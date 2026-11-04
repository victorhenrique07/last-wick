using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets._Project.Code.Runtime.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Assets._Project.Code.Editor
{
    // Monta a cena do menu principal (AD-013) com as peças originais do Gothic Pixel UI.
    // Pode ser rodado de novo: reconfigura os sprites e recria a cena do zero.
    public static class MainMenuSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Menu/MainMenu.unity";
        private const string ArtPath = "Assets/_Project/Art/Sprites/UI/Menu";
        private const string FontPath = "Assets/_Project/Art/Fonts/PixelifySans.ttf";
        private const float SpritePixelsPerUnit = 100f / 3f; // 1 pixel da arte = 3 unidades do Canvas de 1920x1080
        private const int FrameBorder = 16;                  // cantos das molduras de 48 px do pacote

        // Cores tiradas do pacote
        private static readonly Color Background = new Color32(20, 15, 11, 255);
        private static readonly Color Gold = new Color32(194, 176, 72, 255);
        private static readonly Color Cream = new Color32(232, 220, 192, 255);
        private static readonly Color Disabled = new Color32(103, 92, 80, 255);
        private static readonly Color Outline = new Color32(49, 30, 18, 255);
        private static readonly Color Dim = new Color(0.03f, 0.02f, 0.01f, 0.8f);

        private static Font font;

        [MenuItem("Last Wick/Montar menu principal")]
        public static void Build()
        {
            ConfigureArt();
            font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            CreateEventSystem();

            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            canvasGo.layer = LayerMask.NameToLayer("UI");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var menu = canvasGo.AddComponent<MainMenu>();
            var fields = new SerializedObject(menu);

            // Título e arabesco
            var title = Label("Titulo", canvasGo.transform, "LAST WICK", 140, Gold, TextAnchor.MiddleCenter);
            Place(title, new Vector2(1200, 170), new Vector2(0, 330));
            var flourish = Node("Arabesco", canvasGo.transform);
            Img(flourish, Art("MenuFlourish"), Color.white).SetNativeSize();
            flourish.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 225);

            // Moldura com as opções
            var frame = Node("Opcoes", canvasGo.transform);
            Img(frame, Art("MenuFrame"), Color.white, tiled: true);
            Place(frame, new Vector2(624, 528), new Vector2(0, -130));
            var column = frame.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(60, 60, 60, 60);
            column.spacing = 8;
            column.childAlignment = TextAnchor.MiddleCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            var newGame = Option(frame.transform, "NovoJogo", "Novo jogo", menu.NewGame);
            var continueGame = Option(frame.transform, "Continuar", "Continuar", menu.Continue);
            var options = Option(frame.transform, "Opcoes", "Opções", null);
            Option(frame.transform, "Sair", "Sair do jogo", menu.Quit);
            fields.FindProperty("continueOption").objectReferenceValue = continueGame;
            fields.FindProperty("optionsOption").objectReferenceValue = options;

            // Confirmação do novo jogo, por cima de tudo
            var confirm = ConfirmPanel(canvasGo.transform, menu);
            fields.FindProperty("confirmNewGame").objectReferenceValue = confirm;
            fields.ApplyModifiedPropertiesWithoutUndo();
            confirm.SetActive(false);

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            PutFirstInBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MainMenuSceneBuilder] Cena salva em {ScenePath}");
        }

        // Usado para conferir: monta a cena e salva imagens do menu e da confirmação em Logs/
        public static void BuildAndPreview()
        {
            Build();
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var confirm = canvas.transform.Find("ConfirmarNovoJogo").gameObject;
            Preview(canvas, Path.GetFullPath("Logs/main_menu_preview.png"));
            confirm.SetActive(true);
            Preview(canvas, Path.GetFullPath("Logs/main_menu_confirm_preview.png"));
            confirm.SetActive(false);
        }

        private static void Preview(Canvas canvas, string path)
        {
            var camera = Camera.main;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 5;

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
            Object.DestroyImmediate(target);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Debug.Log($"[MainMenuSceneBuilder] Preview salvo em {path}");
        }

        private static GameObject ConfirmPanel(Transform canvas, MainMenu menu)
        {
            // Fundo escurecido em tela cheia: também bloqueia os cliques no menu atrás
            var screen = Node("ConfirmarNovoJogo", canvas);
            Stretch(screen);
            Img(screen, null, Dim);

            var panel = Node("Painel", screen.transform);
            Img(panel, Art("DialogFrame"), Color.white, tiled: true);
            Place(panel, new Vector2(816, 384), Vector2.zero);
            var column = panel.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(72, 72, 64, 56);
            column.spacing = 12;
            column.childAlignment = TextAnchor.MiddleCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            Size(Label("Titulo", panel.transform, "Começar de novo?", 52, Cream, TextAnchor.MiddleCenter), height: 64);
            Size(Label("Aviso", panel.transform, "O progresso atual será perdido.", 32, Cream, TextAnchor.MiddleCenter), height: 48);

            var row = Node("Botoes", panel.transform);
            var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 24;
            rowLayout.childAlignment = TextAnchor.MiddleCenter;
            SetControl(rowLayout, forceWidth: true, forceHeight: false);
            Size(row, height: 88);
            Option(row.transform, "Confirmar", "Confirmar", menu.StartNewGame, size: 40);
            Option(row.transform, "Cancelar", "Cancelar", menu.CancelNewGame, size: 40);
            return screen;
        }

        // Texto clicável com um colchete de cada lado, que só aparecem com o mouse em cima
        private static MenuOption Option(Transform parent, string name, string text, UnityAction onClick, int size = 52)
        {
            var go = Node(name, parent);
            Img(go, null, Color.clear); // área de clique da linha inteira
            Size(go, height: 88);
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 18;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;

            var left = Bracket(go.transform, "ColcheteEsquerdo", "BracketLeft");
            var label = Label("Texto", go.transform, text, size, Cream, TextAnchor.MiddleCenter);
            label.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            var right = Bracket(go.transform, "ColcheteDireito", "BracketRight");

            var button = go.AddComponent<Button>();
            button.transition = Selectable.Transition.None; // o MenuOption cuida do visual
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.Vertical;
            button.navigation = navigation;
            if (onClick != null)
                UnityEventTools.AddPersistentListener(button.onClick, onClick);

            var option = go.AddComponent<MenuOption>();
            var fields = new SerializedObject(option);
            fields.FindProperty("label").objectReferenceValue = label.GetComponent<Text>();
            var brackets = fields.FindProperty("brackets");
            brackets.arraySize = 2;
            brackets.GetArrayElementAtIndex(0).objectReferenceValue = left;
            brackets.GetArrayElementAtIndex(1).objectReferenceValue = right;
            fields.FindProperty("normalColor").colorValue = Cream;
            fields.FindProperty("highlightColor").colorValue = Gold;
            fields.FindProperty("disabledColor").colorValue = Disabled;
            fields.ApplyModifiedPropertiesWithoutUndo();
            return option;
        }

        private static Image Bracket(Transform parent, string name, string sprite)
        {
            var go = Node(name, parent);
            var image = Img(go, Art(sprite), Color.white);
            image.enabled = false;
            image.raycastTarget = false;
            var element = go.AddComponent<LayoutElement>();
            element.preferredWidth = 30;  // 10 px x 3
            element.preferredHeight = 72; // 24 px x 3
            return image;
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            go.transform.position = new Vector3(0f, 0f, -10f);
            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Background;
            go.AddComponent<UniversalAdditionalCameraData>();
        }

        private static void CreateEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
        }

        // O menu é a primeira cena do jogo
        private static void PutFirstInBuildSettings(string path)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes.Where(s => s.path != path));
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        // Pixel art: sem filtro, sem compressão, e bordas de 9-slice nas molduras
        private static void ConfigureArt()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { ArtPath }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                bool frame = Path.GetFileNameWithoutExtension(path).EndsWith("Frame");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = SpritePixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.spriteBorder = frame ? Vector4.one * FrameBorder : Vector4.zero;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect; // o modo Tiled precisa da malha retangular
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
        }

        private static Sprite Art(string name) => AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/{name}.png");

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

        private static void Place(GameObject go, Vector2 size, Vector2 position)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        private static Image Img(GameObject go, Sprite sprite, Color color, bool tiled = false)
        {
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = tiled ? Image.Type.Tiled : Image.Type.Simple;
            return image;
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
            shadow.effectColor = Outline;
            shadow.effectDistance = new Vector2(3, -3);
            return go;
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
    }
}
