using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets._Project.Code.Runtime.Hub;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Assets._Project.Code.Editor
{
    // Monta a cena do hub a partir dos sprites do tileset do cemitério.
    // Pode ser rodado de novo: reconfigura os sprites e recria a cena do zero.
    public static class HubSceneBuilder
    {
        private const int PixelsPerUnit = 16;
        private const string SpritesPath = "Assets/_Project/Art/Sprites";
        private const string TilesetPath = SpritesPath + "/Environment/TilesetGraveyard.png";
        private const string GroundPath = SpritesPath + "/Environment/HubGround.png";
        private const string TilesPath = "Assets/_Project/Art/Tilemaps/Tiles/Hub";
        private const string ScenePath = "Assets/_Project/Scenes/Hub/Hub.unity";
        private const string OutlineMaterialPath = "Assets/_Project/Art/Materials/HubOutline.mat";
        private const int NpcSpritePadding = 2; // margem transparente em volta da igreja, bigorna e mesa, para o contorno caber

        // Centro da clareira (onde fica a chama) e raios da área iluminada, em unidades
        private static readonly Vector2 Center = new Vector2(0.5f, -1.5f);
        private const float ClearingRadiusX = 7f;
        private const float ClearingRadiusY = 4.2f;

        // Base do portão, no fim da trilha de baixo, entre o círculo de pedra e a borda de baixo da câmera
        private static readonly Vector2 GatePosition = new Vector2(0.5f, -5.2f);

        // Recortes do tileset (x, y a partir do canto superior esquerdo, em pixels)
        private static readonly (string name, int x, int y, int w, int h, bool tile)[] TilesetPieces =
        {
            ("Stone_0", 0, 80, 16, 16, true), ("Stone_1", 16, 80, 16, 16, true), ("Stone_2", 32, 80, 16, 16, true),
            ("Stone_3", 0, 96, 16, 16, true), ("Stone_4", 16, 96, 16, 16, true), ("Stone_5", 32, 96, 16, 16, true),
            ("Stone_6", 0, 112, 16, 16, true), ("Stone_7", 16, 112, 16, 16, true), ("Stone_8", 32, 112, 16, 16, true),
            ("Cross_A", 5, 19, 7, 11, false), ("Grave_A", 17, 19, 16, 12, false),
            ("Grave_B", 35, 19, 11, 11, false), ("Grave_C", 51, 18, 13, 12, false),
            ("Cross_B", 5, 35, 7, 11, false), ("Grave_D", 17, 35, 16, 12, false),
            ("Grave_E", 34, 35, 12, 11, false), ("Grave_F", 51, 34, 13, 12, false),
            ("DeadTree", 65, 17, 14, 14, false), ("Bush", 89, 21, 16, 19, false),
            ("Torch", 37, 65, 10, 14, false),
            ("Rock_A", 7, 6, 8, 7, false), ("Rock_B", 21, 4, 8, 7, false), ("Rock_C", 35, 2, 8, 7, false),
            ("Fence_Long", 8, 145, 38, 12, false), ("Fence_Short", 47, 145, 33, 12, false),
        };

        // Folha do chão (grama do Puny World e pedras do tileset, nas cores do tileset): células de 16x16
        private static readonly string[] DirtNames = { "Dirt_HLeft", "Dirt_HMid", "Dirt_HRight", "Dirt_VTop", "Dirt_VMid", "Dirt_VBottom", "Dirt_Dot" };

        private static IEnumerable<(string name, int x, int y, int w, int h, bool tile)> GroundPieces()
        {
            for (int i = 0; i < 9; i++)
            {
                yield return ($"GrassLight_{i}", i * 16, 0, 16, 16, true);
                yield return ($"GrassDark_{i}", i * 16, 16, 16, 16, true);
            }
            for (int i = 0; i < DirtNames.Length; i++)
                yield return (DirtNames[i], i * 16, 32, 16, 16, true);
            for (int i = 0; i < 8; i++)
                yield return ($"StepStone_{i}", i * 16, 48, 16, 16, true);
        }

        [MenuItem("Last Wick/Montar cena do Hub")]
        public static void Build()
        {
            ConfigureSingleSprite(SpritesPath + "/Hub/Church.png", NpcSpritePadding);
            ConfigureSingleSprite(SpritesPath + "/Hub/Anvil.png", NpcSpritePadding);
            ConfigureSingleSprite(SpritesPath + "/Hub/TarotTable.png", NpcSpritePadding);
            ConfigureSingleSprite(SpritesPath + "/Hub/HubFlame.png", 0);
            ConfigureSingleSprite(SpritesPath + "/Hub/Gate.png", NpcSpritePadding);
            var outlineMaterial = CreateOutlineMaterial();
            SliceSheet(TilesetPath, TilesetPieces);
            SliceSheet(GroundPath, GroundPieces().ToArray());

            var sprites = AssetDatabase.LoadAllAssetsAtPath(TilesetPath).OfType<Sprite>()
                .Concat(AssetDatabase.LoadAllAssetsAtPath(GroundPath).OfType<Sprite>())
                .ToDictionary(s => s.name);
            var tiles = sprites.Values.Where(s => s.name.StartsWith("Grass") || s.name.StartsWith("Dirt") || s.name.StartsWith("Stone_"))
                .ToDictionary(s => s.name, s => CreateTile(s.name, s));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var random = new System.Random(7); // semente fixa: o mesmo mapa a cada build

            CreateCamera();
            CreateGround(tiles, random);

            var world = new GameObject("Mundo").transform;

            // NPCs: clicáveis, com contorno quando o mouse passa por cima
            var church = PlaceInteractable("Igreja (Padre)", "Hub/Church.png", new Vector2(0.5f, 1.4f), world, outlineMaterial);
            PlaceInteractable("Bigorna (Ferreiro)", "Hub/Anvil.png", new Vector2(-4.5f, -1.6f), world, outlineMaterial);
            PlaceInteractable("Mesa de Tarot (Mago)", "Hub/TarotTable.png", new Vector2(5.5f, -2f), world, outlineMaterial);

            // Portão no fim da trilha de baixo: a saída para a run (HUB-01), com o painel de expedição no Canvas
            var gate = PlaceInteractable("Portao (Expedicao)", "Hub/Gate.png", GatePosition, world, outlineMaterial);
            var expedition = HubCanvasBuilder.Build(out var priestMenu);
            LinkClick(gate, expedition.Open);
            LinkClick(church, priestMenu.Open); // PRST-01
            CreateEventSystem();

            var flame = Place("Fogueira", LoadSprite("Hub/HubFlame.png"), Center + new Vector2(0f, -0.4f), world);
            AddPointLight(flame.transform, new Vector2(0f, 0.4f), new Color(1f, 0.72f, 0.4f), 1.5f, 6.5f);

            foreach (float x in new[] { -0.9f, 1.9f })
            {
                var torch = Place("Tocha", sprites["Torch"], new Vector2(x, 1.4f), world);
                AddPointLight(torch.transform, new Vector2(0f, 0.7f), new Color(1f, 0.75f, 0.45f), 0.8f, 2.2f);
            }

            CreateStepStoneTrails(sprites, world, random);
            CreateOutskirts(sprites, world, random);

            var globalLight = new GameObject("Luz Global").AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.color = new Color(0.62f, 0.68f, 0.85f);
            globalLight.intensity = 0.4f;
            ApplyToAllSortingLayers(globalLight);

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[HubSceneBuilder] Cena salva em {ScenePath}");
        }

        // Usado para conferir: monta a cena e salva imagens da câmera em Logs/ (hub e painel de expedição aberto)
        // (Logs/ e não Temp/, porque o Unity apaga a pasta Temp/ ao fechar)
        public static void BuildAndPreview()
        {
            Build();
            var cam = Camera.main;
            var canvas = Object.FindFirstObjectByType<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; // o Overlay não aparece no render da câmera
            canvas.worldCamera = cam;
            canvas.planeDistance = 5;
            canvas.sortingOrder = short.MaxValue; // por cima dos sprites, como o Overlay fica no jogo

            SavePreview(cam, "Logs/hub_preview.png");
            var panel = canvas.transform.Find("PainelExpedicao/Conteudo").gameObject;
            panel.SetActive(true);
            SavePreview(cam, "Logs/hub_expedition_preview.png");
            panel.SetActive(false);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        private static void SavePreview(Camera cam, string file)
        {
            var rt = new RenderTexture(1280, 720, 24);
            cam.targetTexture = rt;
            Canvas.ForceUpdateCanvases();
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            string path = Path.GetFullPath(file);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Debug.Log($"[HubSceneBuilder] Preview salvo em {path}");
            cam.targetTexture = null;
            RenderTexture.active = null;
        }

        private static float ClearingDistance(Vector2 p)
        {
            var d = p - Center;
            return d.x * d.x / (ClearingRadiusX * ClearingRadiusX) + d.y * d.y / (ClearingRadiusY * ClearingRadiusY);
        }

        private static void CreateGround(Dictionary<string, Tile> tiles, System.Random random)
        {
            var grid = new GameObject("Grid").AddComponent<Grid>();
            var ground = CreateTilemap("Chao", grid.transform, -1000);
            var dirt = CreateTilemap("Trilha", grid.transform, -999);
            var floor = CreateTilemap("Piso", grid.transform, -998);

            // Grama clara na clareira e escura em volta, com a borda irregular
            for (int x = -10; x < 11; x++)
                for (int y = -7; y < 6; y++)
                {
                    float jitter = (float)random.NextDouble() * 0.5f - 0.25f;
                    bool lit = ClearingDistance(new Vector2(x + 0.5f, y + 0.5f)) + jitter < 1f;
                    int variant = random.NextDouble() < 0.45 ? 0 : random.Next(1, 9); // metade sem tufos, para não poluir
                    ground.SetTile(new Vector3Int(x, y, 0), tiles[(lit ? "GrassLight_" : "GrassDark_") + variant]);
                }

            // Círculo de pedra em volta da chama
            for (int x = -2; x < 3; x++)
                for (int y = -4; y < 1; y++)
                    if (Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), Center) < 1.8f)
                        floor.SetTile(new Vector3Int(x, y, 0), tiles[$"Stone_{random.Next(9)}"]);

            // Trilha de terra da igreja até a pedra e da pedra até a saída de baixo
            dirt.SetTile(new Vector3Int(0, 1, 0), tiles["Dirt_VTop"]);
            dirt.SetTile(new Vector3Int(0, 0, 0), tiles["Dirt_VMid"]);
            for (int y = -4; y > -8; y--)
                dirt.SetTile(new Vector3Int(0, y, 0), tiles["Dirt_VMid"]);

            // Terra batida embaixo da bigorna e da mesa de tarot
            foreach (int start in new[] { -6, 4 })
            {
                dirt.SetTile(new Vector3Int(start, -2, 0), tiles["Dirt_HLeft"]);
                dirt.SetTile(new Vector3Int(start + 1, -2, 0), tiles["Dirt_HMid"]);
                dirt.SetTile(new Vector3Int(start + 2, -2, 0), tiles["Dirt_HRight"]);
            }
        }

        private static void CreateStepStoneTrails(Dictionary<string, Sprite> sprites, Transform world, System.Random random)
        {
            var parent = new GameObject("Trilhas de Pedra").transform;
            parent.SetParent(world);

            // Cada trilha é uma curva (início, controle, fim)
            var trails = new[]
            {
                (new Vector2(-1.3f, -1.4f), new Vector2(-2.2f, -1.7f), new Vector2(-3f, -1.5f)),
                (new Vector2(2.3f, -1.4f), new Vector2(3.2f, -1.7f), new Vector2(4f, -1.5f)),
                (new Vector2(-1.1f, -0.3f), new Vector2(-3.5f, 1.2f), new Vector2(-6.5f, 3f)),
                (new Vector2(2.2f, -0.2f), new Vector2(4.5f, 0.6f), new Vector2(7.8f, 2.2f)),
                (new Vector2(2.4f, -3.2f), new Vector2(5f, -4.4f), new Vector2(8.5f, -4.6f)),
                (new Vector2(-1.4f, -3.3f), new Vector2(-4f, -4.6f), new Vector2(-8f, -4.2f)),
            };

            foreach (var (start, control, end) in trails)
            {
                float length = Vector2.Distance(start, control) + Vector2.Distance(control, end);
                int count = Mathf.CeilToInt(length / 0.6f);
                for (int i = 0; i <= count; i++)
                {
                    float t = i / (float)count;
                    var point = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * control + t * t * end;
                    point += new Vector2((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 0.25f;
                    var stone = Place("Pedra", sprites[$"StepStone_{random.Next(8)}"], point, parent);
                    stone.GetComponent<SpriteRenderer>().sortingOrder = -900; // sempre no chão, embaixo dos objetos
                }
            }
        }

        private static void CreateOutskirts(Dictionary<string, Sprite> sprites, Transform world, System.Random random)
        {
            var decor = new GameObject("Decoracao").transform;
            decor.SetParent(world);

            string[] graves = { "Cross_A", "Grave_A", "Grave_B", "Grave_C", "Cross_B", "Grave_D", "Grave_E", "Grave_F" };
            var used = new List<Vector2> { new Vector2(0.5f, 2.5f), new Vector2(-4.5f, -2f), new Vector2(5.5f, -2f) };

            // Túmulos espalhados na parte escura, sem encostar uns nos outros nem nas trilhas principais
            int placed = 0;
            for (int attempt = 0; attempt < 600 && placed < 34; attempt++)
            {
                var p = new Vector2((float)random.NextDouble() * 18f - 8.5f, (float)random.NextDouble() * 10f - 5.2f);
                if (ClearingDistance(p) < 1.1f || Mathf.Abs(p.x - 0.5f) < 0.9f && p.y < -3f) continue;
                if (Mathf.Abs(p.x - GatePosition.x) < 2.8f && p.y < GatePosition.y + 2.2f) continue; // espaço do portão
                if (used.Any(u => Vector2.Distance(u, p) < 1.15f)) continue;
                used.Add(p);
                Place("Tumulo", sprites[graves[random.Next(graves.Length)]], p, decor);
                placed++;
            }

            foreach (var p in new[] { new Vector2(-6.8f, 2.6f), new Vector2(7.2f, 1.2f), new Vector2(-7.4f, -3.6f), new Vector2(6.6f, -4.2f) })
                Place("Arvore Seca", sprites["DeadTree"], p, decor);
            foreach (var p in new[] { new Vector2(-2.4f, 1.3f), new Vector2(3.6f, 1.6f), new Vector2(-5.6f, -0.4f), new Vector2(7.6f, -1.6f), new Vector2(-2.6f, -4.4f) })
                Place("Arbusto", sprites["Bush"], p, decor);
            foreach (var p in new[] { new Vector2(-3f, 0.1f), new Vector2(3.6f, -0.4f), new Vector2(3.4f, -3.7f), new Vector2(-1.2f, 2.2f) })
                Place("Pedra", sprites[$"Rock_{"ABC"[random.Next(3)]}"], p, decor);

            // Pedaços de cerca velha nas bordas
            foreach (var (p, name) in new[] { (new Vector2(-5.8f, 3.7f), "Fence_Long"), (new Vector2(5.2f, 3.9f), "Fence_Short"),
                         (new Vector2(-7.2f, -1.2f), "Fence_Short"), (new Vector2(7.4f, -3f), "Fence_Long"),
                         (new Vector2(-4.2f, -4.9f), "Fence_Long"), (new Vector2(4.6f, -4.9f), "Fence_Short") })
                Place("Cerca", sprites[name], p, decor);
        }

        private static GameObject Place(string name, Sprite sprite, Vector2 position, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = Mathf.RoundToInt(-position.y * 10f); // mais embaixo na tela = desenhado na frente
            return go;
        }

        private static void LinkClick(GameObject interactable, UnityAction action)
        {
            var onClick = (UnityEvent)typeof(HubInteractable).GetField("onClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(interactable.GetComponent<HubInteractable>());
            UnityEventTools.AddPersistentListener(onClick, action);
        }

        private static GameObject PlaceInteractable(string name, string spritePath, Vector2 position, Transform parent, Material outlineMaterial)
        {
            var go = Place(name, LoadSprite(spritePath), position, parent);
            go.AddComponent<BoxCollider2D>();
            var interactable = new SerializedObject(go.AddComponent<HubInteractable>());
            interactable.FindProperty("outlineMaterial").objectReferenceValue = outlineMaterial;
            interactable.ApplyModifiedPropertiesWithoutUndo();
            return go;
        }

        // O clique nos objetos do mundo passa pelo EventSystem, como na UI
        private static void CreateEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
        }

        private static Material CreateOutlineMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(OutlineMaterialPath);
            if (material != null) return material;

            Directory.CreateDirectory(Path.GetDirectoryName(OutlineMaterialPath));
            material = new Material(Shader.Find("Last Wick/Sprite Outline"));
            AssetDatabase.CreateAsset(material, OutlineMaterialPath);
            return material;
        }

        private static void AddPointLight(Transform parent, Vector2 offset, Color color, float intensity, float radius)
        {
            var go = new GameObject("Luz");
            go.transform.SetParent(parent);
            go.transform.localPosition = offset;
            var light = go.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.pointLightInnerRadius = radius * 0.15f;
            light.pointLightOuterRadius = radius;
            ApplyToAllSortingLayers(light);
        }

        // Luz criada por código não ilumina nenhuma sorting layer até a gente dizer quais
        private static void ApplyToAllSortingLayers(Light2D light)
        {
            var so = new SerializedObject(light);
            var layers = so.FindProperty("m_ApplyToSortingLayers");
            layers.arraySize = SortingLayer.layers.Length;
            for (int i = 0; i < SortingLayer.layers.Length; i++)
                layers.GetArrayElementAtIndex(i).intValue = SortingLayer.layers[i].id;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            go.transform.position = new Vector3(0.5f, -0.3f, -10f);
            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color32(19, 20, 20, 255);
            cam.allowHDR = false;  // pixel art: sem HDR nem MSAA, que borram as bordas dos pixels
            cam.allowMSAA = false;
            go.AddComponent<UniversalAdditionalCameraData>();

            // 320x180 com sprites de 16 px: 11,25 unidades de altura, escala inteira (6x em 1080p)
            var pixelPerfect = go.AddComponent<PixelPerfectCamera>();
            pixelPerfect.assetsPPU = PixelsPerUnit;
            pixelPerfect.refResolutionX = 320;
            pixelPerfect.refResolutionY = 180;
            pixelPerfect.gridSnapping = PixelPerfectCamera.GridSnapping.PixelSnapping;
            pixelPerfect.cropFrame = PixelPerfectCamera.CropFrame.None;
            go.AddComponent<Physics2DRaycaster>(); // deixa o EventSystem enxergar os Collider2D do mundo
        }

        private static Tilemap CreateTilemap(string name, Transform grid, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(grid);
            var tilemap = go.AddComponent<Tilemap>();
            go.AddComponent<TilemapRenderer>().sortingOrder = sortingOrder;
            return tilemap;
        }

        private static Tile CreateTile(string name, Sprite sprite)
        {
            Directory.CreateDirectory(TilesPath);
            string path = $"{TilesPath}/{name}.asset";
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, path);
            }
            tile.sprite = sprite;
            EditorUtility.SetDirty(tile);
            return tile;
        }

        private static Sprite LoadSprite(string relativePath) =>
            AssetDatabase.LoadAssetAtPath<Sprite>($"{SpritesPath}/{relativePath}");

        private static TextureImporter ConfigurePixelArt(string path, SpriteImportMode mode)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = mode;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            return importer;
        }

        // Pivô no pé do desenho, descontando a margem transparente de baixo
        private static void ConfigureSingleSprite(string path, int padding)
        {
            var importer = ConfigurePixelArt(path, SpriteImportMode.Single);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(0.5f, padding / (float)texture.height);
            settings.spriteMeshType = SpriteMeshType.FullRect; // malha "Tight" cortaria o contorno
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        private static void SliceSheet(string path, (string name, int x, int y, int w, int h, bool tile)[] pieces)
        {
            var importer = ConfigurePixelArt(path, SpriteImportMode.Multiple);
            importer.SaveAndReimport();

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();

            // Mantém o ID dos recortes que já existem, para não quebrar referências em outras cenas
            var existingIds = provider.GetSpriteRects().ToDictionary(r => r.name, r => r.spriteID);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            var rects = pieces.Select(p => new SpriteRect
            {
                name = p.name,
                rect = new Rect(p.x, texture.height - p.y - p.h, p.w, p.h), // o Unity conta o Y de baixo para cima
                alignment = p.tile ? SpriteAlignment.Center : SpriteAlignment.BottomCenter,
                pivot = p.tile ? new Vector2(0.5f, 0.5f) : new Vector2(0.5f, 0f),
                spriteID = existingIds.TryGetValue(p.name, out var id) ? id : GUID.Generate(),
            }).ToArray();

            provider.SetSpriteRects(rects);
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>()
                .SetNameFileIdPairs(rects.Select(r => new SpriteNameFileIdPair(r.name, r.spriteID)).ToList());
            provider.Apply();
            importer.SaveAndReimport();
        }

        private static void AddToBuildSettings(string path)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (scenes.Any(s => s.path == path)) return;
            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
