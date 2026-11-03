using System.IO;
using Assets._Project.Code.Runtime.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets._Project.Code.Editor
{
    // Monta a tela de vitória dentro do prefab do HUD, seguindo o mockup em Docs/Mockups/TelaVitoria_Meshy.png.
    // Pode ser rodado de novo: apaga o VictoryPanel antigo do HUD e recria do zero.
    public static class VictoryScreenBuilder
    {
        private const string HudPath = "Assets/_Project/Prefabs/UI/HUD.prefab";
        private const string CardSlotPath = "Assets/_Project/Prefabs/UI/Victory/CardSlot.prefab";
        private const string ArtPath = "Assets/_Project/Art/Sprites/UI/Victory";
        private const string FontPath = "Assets/_Project/Art/Fonts/PixelifySans.ttf";
        private const float SpritePixelsPerUnit = 100f / 3f; // 1 pixel da arte = 3 unidades do Canvas de 1920x1080

        // Cores da paleta do jogo
        private static readonly Color Dim = new Color(0.02f, 0.02f, 0.02f, 0.78f);
        private static readonly Color Cream = new Color32(232, 220, 192, 255);
        private static readonly Color Muted = new Color32(172, 194, 195, 255);
        private static readonly Color Gold = new Color32(209, 202, 128, 255);
        private static readonly Color ShadowColor = new Color32(19, 20, 20, 255);

        private static Font font;

        [MenuItem("Last Wick/Montar tela de vitória")]
        public static void Build()
        {
            ConfigureArt();
            ConfigureFont();
            font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
            var cardSlot = BuildCardSlotPrefab();

            var hud = PrefabUtility.LoadPrefabContents(HudPath);
            try
            {
                var old = hud.transform.Find("VictoryPanel");
                int siblingIndex = old.GetSiblingIndex();
                Object.DestroyImmediate(old.gameObject);

                var controller = hud.GetComponent<VictoryPanel>();
                var screen = BuildScreen(hud.transform, controller, cardSlot);
                screen.transform.SetSiblingIndex(siblingIndex);
                screen.SetActive(false);

                PrefabUtility.SaveAsPrefabAsset(hud, HudPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(hud);
            }
            Debug.Log("[VictoryScreenBuilder] Tela de vitória montada no HUD");
        }

        // Usado no modo batch: monta a tela e salva uma imagem dela preenchida com dados de exemplo.
        // Fica em Logs/ porque o Unity apaga a pasta Temp/ ao fechar.
        public static void BuildAndPreview()
        {
            Build();
            try
            {
                Preview(Path.GetFullPath("Logs/victory_preview.png"));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static GameObject BuildScreen(Transform hud, VictoryPanel controller, CardSlotView cardSlotPrefab)
        {
            var fields = new SerializedObject(controller);

            // Fundo escurecido por cima do jogo pausado
            var screen = Node("VictoryPanel", hud);
            Stretch(screen);
            Img(screen, null, Dim);
            fields.FindProperty("victoryPanel").objectReferenceValue = screen;

            var panel = Node("Painel", screen.transform);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(1300, 1000);
            panelRect.anchoredPosition = new Vector2(0, -26);
            Img(panel, Art("PanelFrame"), Color.white, tiled: true);
            var column = panel.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(76, 76, 78, 66);
            column.spacing = 14;
            column.childAlignment = TextAnchor.UpperCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            // Vela em cima da moldura
            Decoration(panel, "Vela", Art("TopCandle"), new Vector2(0.5f, 1f), new Vector2(0.5f, 0f), new Vector2(0, -12));

            var title = Label("Titulo", panel.transform, "A CHAMA RESISTE", 64, Cream, TextAnchor.MiddleCenter);
            Size(title, height: 76);
            Ornament(panel.transform, "Arabesco", Art("TitleFlourish"));

            // Resumo da run
            var summary = Node("Resumo", panel.transform);
            Img(summary, Art("SummaryFrame"), Color.white, tiled: true);
            var summaryRow = summary.AddComponent<HorizontalLayoutGroup>();
            summaryRow.padding = new RectOffset(58, 58, 30, 30);
            summaryRow.spacing = 4;
            SetControl(summaryRow, forceWidth: true, forceHeight: true);
            Size(summary, height: 140);
            Link(fields, "runTime", SummaryItem(summary.transform, "IconClock", "Tempo da run", "8:42"));
            Link(fields, "remainingTime", SummaryItem(summary.transform, "IconHourglass", "Tempo restante", "1:18"));
            Link(fields, "bossesDefeated", SummaryItem(summary.transform, "IconSkull", "Bosses derrotados", "3"));
            Link(fields, "kills", SummaryItem(summary.transform, "IconGrave", "Inimigos mortos", "912"));
            Link(fields, "finalLevel", SummaryItem(summary.transform, "IconCrown", "Nível final", "24"));

            // Status e cartas lado a lado
            var row = Node("Blocos", panel.transform);
            var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 28;
            SetControl(rowLayout, forceWidth: true, forceHeight: true);
            row.AddComponent<LayoutElement>().flexibleHeight = 1;

            var statsGrid = Block(row.transform, "Status finais", out _);
            var grid = statsGrid.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(212, 46);
            grid.spacing = new Vector2(16, 4);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            Link(fields, "maxHealth", StatRow(statsGrid.transform, "IconHeart", "Vida máxima"));
            Link(fields, "dodgeDistance", StatRow(statsGrid.transform, "IconDodge", "Distância da esquiva"));
            Link(fields, "moveSpeed", StatRow(statsGrid.transform, "IconBoot", "Velocidade"));
            Link(fields, "dodgeCooldown", StatRow(statsGrid.transform, "IconCooldown", "Recarga da esquiva"));
            Link(fields, "lightRadius", StatRow(statsGrid.transform, "IconLight", "Raio da luz"));
            Link(fields, "fireRate", StatRow(statsGrid.transform, "IconFireRate", "Cadência"));
            Link(fields, "xpPickupRadius", StatRow(statsGrid.transform, "IconXp", "Coleta de XP"));
            Link(fields, "projectileSpeed", StatRow(statsGrid.transform, "IconProjectile", "Velocidade do projétil"));
            Link(fields, "bonusPierce", StatRow(statsGrid.transform, "IconPierce", "Perfuração extra"));
            Link(fields, "projectileRange", StatRow(statsGrid.transform, "IconRange", "Alcance do projétil"));

            var cardsGrid = Block(row.transform, "Cartas", out var cardsBlock);
            var cards = cardsGrid.AddComponent<GridLayoutGroup>();
            cards.cellSize = new Vector2(80, 112);
            cards.spacing = new Vector2(10, 8);
            cards.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cards.constraintCount = 5;
            cards.childAlignment = TextAnchor.UpperCenter;
            var noCards = Label("SemCartas", cardsBlock.transform, "Nenhuma carta", 26, Muted, TextAnchor.MiddleCenter);
            Size(noCards, height: 40);
            fields.FindProperty("cardsContainer").objectReferenceValue = cardsGrid.transform;
            fields.FindProperty("cardSlotPrefab").objectReferenceValue = cardSlotPrefab;
            fields.FindProperty("noCardsLabel").objectReferenceValue = noCards;

            // Botão
            var buttonRow = Node("Rodape", panel.transform);
            var buttonLayout = buttonRow.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;
            SetControl(buttonLayout, forceWidth: false, forceHeight: true);
            Size(buttonRow, height: 72); // altura nativa da barra (24 px x 3), para não esticar os ornamentos
            var button = Node("BotaoVoltar", buttonRow.transform);
            Img(button, Art("ButtonNormal"), Color.white); // já desenhado na largura final (150 px x 3)
            Size(button, width: 450);
            var buttonComponent = button.AddComponent<Button>();
            buttonComponent.transition = Selectable.Transition.SpriteSwap;
            buttonComponent.spriteState = new SpriteState
            {
                highlightedSprite = Art("ButtonHover"),
                pressedSprite = Art("ButtonHover"),
                selectedSprite = Art("ButtonHover"),
            };
            UnityEventTools.AddPersistentListener(buttonComponent.onClick, new UnityAction(controller.ReturnToHub));
            var buttonLabel = Label("Texto", button.transform, "Voltar ao acampamento", 24, Cream, TextAnchor.MiddleCenter);
            Stretch(buttonLabel);

            fields.ApplyModifiedPropertiesWithoutUndo();
            return screen;
        }

        // Bloco com moldura, título e divisória; devolve o objeto onde vai o conteúdo
        private static GameObject Block(Transform parent, string title, out GameObject block)
        {
            block = Node(title, parent);
            Img(block, Art("SubPanelFrame"), Color.white, tiled: true);
            // Os dois blocos dividem a largura por igual, sem depender do tamanho do conteúdo
            var element = block.AddComponent<LayoutElement>();
            element.minWidth = 0;
            element.preferredWidth = 0;
            element.flexibleWidth = 1;
            var layout = block.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(56, 56, 40, 48);
            layout.spacing = 6;
            layout.childAlignment = TextAnchor.UpperCenter;
            SetControl(layout, forceWidth: true, forceHeight: false);

            Size(Label("Titulo", block.transform, title, 34, Cream, TextAnchor.MiddleCenter), height: 40);
            Ornament(block.transform, "Divisoria", Art("Divider"));

            var content = Node("Conteudo", block.transform);
            content.AddComponent<LayoutElement>().flexibleHeight = 1;
            return content;
        }

        // Rótulo em cima e, embaixo, ícone + valor, tudo centralizado
        private static StatEntryView SummaryItem(Transform parent, string icon, string label, string sample)
        {
            var item = Node(label, parent);
            var column = item.AddComponent<VerticalLayoutGroup>();
            column.childAlignment = TextAnchor.MiddleCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            var name = Label("Nome", item.transform, label, 20, Muted, TextAnchor.MiddleCenter);
            name.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow; // rótulo numa linha só
            Size(name, height: 26);

            var line = Node("Linha", item.transform);
            var row = line.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 10;
            row.childAlignment = TextAnchor.MiddleCenter;
            SetControl(row, forceWidth: false, forceHeight: false);
            Size(line, height: 48);

            var image = Node("Icone", line.transform);
            Img(image, Art(icon), Color.white);
            Size(image, 27, 27);
            var value = Label("Valor", line.transform, sample, 40, Cream, TextAnchor.MiddleLeft);
            value.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            Size(value, width: 90, height: 48);

            return Entry(item, value);
        }

        private static StatEntryView StatRow(Transform parent, string icon, string label)
        {
            var row = Node(label, parent);

            var image = Node("Icone", row.transform);
            Img(image, Art(icon), Color.white);
            var iconRect = image.GetComponent<RectTransform>();
            iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.sizeDelta = new Vector2(27, 27);

            var name = Label("Nome", row.transform, label, 20, Muted, TextAnchor.MiddleLeft);
            var nameRect = name.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(36, 0);
            nameRect.offsetMax = new Vector2(-48, 0);

            var value = Label("Valor", row.transform, "0", 26, Cream, TextAnchor.MiddleRight);
            var valueRect = value.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(1, 0);
            valueRect.anchorMax = new Vector2(1, 1);
            valueRect.pivot = new Vector2(1, 0.5f);
            valueRect.sizeDelta = new Vector2(56, 0);

            return Entry(row, value);
        }

        private static CardSlotView BuildCardSlotPrefab()
        {
            var slot = new GameObject("CardSlot", typeof(RectTransform));
            try
            {
                slot.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 112);

                var frame = Node("Moldura", slot.transform);
                Img(frame, Art("CardFrame"), Color.white, sliced: true);
                var frameRect = frame.GetComponent<RectTransform>();
                frameRect.anchorMin = frameRect.anchorMax = frameRect.pivot = new Vector2(0.5f, 1f);
                frameRect.sizeDelta = new Vector2(48, 64);

                var icon = Node("Icone", frame.transform);
                var iconImage = Img(icon, Art("CardIconDefault"), Color.white);
                iconImage.SetNativeSize();

                var count = Label("Quantidade", frame.transform, "x1", 20, Gold, TextAnchor.LowerRight);
                var countRect = count.GetComponent<RectTransform>();
                countRect.anchorMin = countRect.anchorMax = countRect.pivot = new Vector2(1, 0);
                countRect.anchoredPosition = new Vector2(10, -6);
                countRect.sizeDelta = new Vector2(40, 22);

                var name = Label("Nome", slot.transform, "Carta", 20, Muted, TextAnchor.UpperCenter);
                var nameRect = name.GetComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0, 0);
                nameRect.anchorMax = new Vector2(1, 0);
                nameRect.pivot = new Vector2(0.5f, 0);
                nameRect.sizeDelta = new Vector2(0, 48);

                var view = new SerializedObject(slot.AddComponent<CardSlotView>());
                view.FindProperty("icon").objectReferenceValue = iconImage;
                view.FindProperty("cardName").objectReferenceValue = name.GetComponent<Text>();
                view.FindProperty("count").objectReferenceValue = count.GetComponent<Text>();
                view.FindProperty("defaultIcon").objectReferenceValue = Art("CardIconDefault");
                view.ApplyModifiedPropertiesWithoutUndo();

                Directory.CreateDirectory(Path.GetDirectoryName(CardSlotPath));
                return PrefabUtility.SaveAsPrefabAsset(slot, CardSlotPath).GetComponent<CardSlotView>();
            }
            finally
            {
                Object.DestroyImmediate(slot);
            }
        }

        private static void Preview(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Camera").AddComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(34, 58, 48, 255);

            var hud = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(HudPath), scene);
            foreach (Transform child in hud.transform)
                child.gameObject.SetActive(child.name == "VictoryPanel");
            var canvas = hud.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;

            // Algumas cartas de exemplo
            var grid = hud.transform.Find("VictoryPanel/Painel/Blocos/Cartas/Conteudo");
            var slotPrefab = AssetDatabase.LoadAssetAtPath<CardSlotView>(CardSlotPath);
            string[] names = { "Chama Dupla", "Pavio Curto", "Cálice", "Vela Benta", "Sopro Frio", "Cera Quente", "Cinzas" };
            for (int i = 0; i < names.Length; i++)
            {
                var card = ScriptableObject.CreateInstance<CardData>();
                card.Name = names[i];
                Object.Instantiate(slotPrefab, grid).Show(card, i % 3 + 1);
            }
            hud.transform.Find("VictoryPanel/Painel/Blocos/Cartas/SemCartas").gameObject.SetActive(false);

            var target = new RenderTexture(1920, 1080, 24);
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            var image = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            File.WriteAllBytes(path, image.EncodeToPNG());
            RenderTexture.active = null;
            camera.targetTexture = null;
            Debug.Log($"[VictoryScreenBuilder] Preview salvo em {path}");
        }

        private static StatEntryView Entry(GameObject go, GameObject valueText)
        {
            var view = new SerializedObject(go.AddComponent<StatEntryView>());
            view.FindProperty("value").objectReferenceValue = valueText.GetComponent<Text>();
            view.ApplyModifiedPropertiesWithoutUndo();
            return go.GetComponent<StatEntryView>();
        }

        private static void Link(SerializedObject fields, string name, Object value) =>
            fields.FindProperty(name).objectReferenceValue = value;

        private static GameObject Node(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.layer = LayerMask.NameToLayer("UI");
            return go;
        }

        private static void Stretch(GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        // tiled: repete os ornamentos das bordas e o miolo em vez de esticar (molduras do Gothic Pixel UI)
        private static Image Img(GameObject go, Sprite sprite, Color color, bool sliced = false, bool tiled = false)
        {
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = tiled ? Image.Type.Tiled : sliced ? Image.Type.Sliced : Image.Type.Simple;
            return image;
        }

        // Enfeite centralizado no tamanho nativo dentro de um layout vertical (arabescos e divisórias)
        private static void Ornament(Transform parent, string name, Sprite sprite)
        {
            var holder = Node(name, parent);
            var layout = holder.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = layout.childControlHeight = false;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;

            var image = Node("Imagem", holder.transform);
            Img(image, sprite, Color.white).SetNativeSize();
            Size(holder, height: image.GetComponent<RectTransform>().sizeDelta.y);
        }

        private static GameObject Label(string name, Transform parent, string text, int size, Color color, TextAnchor alignment)
        {
            var go = Node(name, parent);
            var label = go.AddComponent<Text>();
            label.font = font;
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            var shadow = go.AddComponent<Shadow>(); // sombra dura de 1 pixel da arte
            shadow.effectColor = ShadowColor;
            shadow.effectDistance = new Vector2(3, -3);
            return go;
        }

        private static void Decoration(GameObject panel, string name, Sprite sprite, Vector2 anchor, Vector2 pivot, Vector2 position)
        {
            var go = Node(name, panel.transform);
            Img(go, sprite, Color.white).SetNativeSize();
            go.AddComponent<LayoutElement>().ignoreLayout = true;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
        }

        private static void Size(GameObject go, float width = -1, float height = -1)
        {
            if (!go.TryGetComponent<LayoutElement>(out var element))
                element = go.AddComponent<LayoutElement>();
            if (width >= 0) element.preferredWidth = width;
            if (height >= 0) element.preferredHeight = height;
        }

        private static void SetControl(HorizontalOrVerticalLayoutGroup layout, bool forceWidth, bool forceHeight)
        {
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = forceWidth;
            layout.childForceExpandHeight = forceHeight;
        }

        private static Sprite Art(string name) => AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/{name}.png");

        // Pixel art: sem filtro, sem compressão, e bordas de 9-slice nas molduras
        private static void ConfigureArt()
        {
            var borders = new System.Collections.Generic.Dictionary<string, Vector4>
            {
                ["PanelFrame"] = new Vector4(21, 21, 21, 21),
                ["SummaryFrame"] = new Vector4(18, 18, 18, 18),
                ["SubPanelFrame"] = new Vector4(18, 18, 18, 18),
                ["CardFrame"] = new Vector4(4, 4, 4, 4),
            };

            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { ArtPath }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = SpritePixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.spriteBorder = borders.TryGetValue(Path.GetFileNameWithoutExtension(path), out var border) ? border : Vector4.zero;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect; // o modo Tiled precisa da malha retangular
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
        }

        private static void ConfigureFont()
        {
            var importer = (TrueTypeFontImporter)AssetImporter.GetAtPath(FontPath);
            // HintedRaster deixaria as letras mais duras, mas deforma o "c" e o "ç" nesses tamanhos
            importer.fontRenderingMode = FontRenderingMode.Smooth;
            importer.SaveAndReimport();
        }
    }
}
