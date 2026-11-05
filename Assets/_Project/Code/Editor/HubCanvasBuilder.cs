using System.IO;
using Assets._Project.Code.Runtime.Hub;
using Assets._Project.Code.Runtime.Priest;
using Assets._Project.Code.Runtime.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LastWick.Player;

namespace Assets._Project.Code.Editor
{
    // Monta o Canvas do hub dentro da cena aberta pelo HubSceneBuilder: a barra de recursos no topo
    // e o painel de expedição. Usa as molduras, os botões e a fonte da tela de resultado.
    public static class HubCanvasBuilder
    {
        private const string ArtPath = "Assets/_Project/Art/Sprites/UI/Victory";
        private const string FontPath = "Assets/_Project/Art/Fonts/PixelifySans.ttf";
        private const string AttributeRowPath = "Assets/_Project/Prefabs/UI/Priest/AttributeRow.prefab";
        private const string PlayerStatsPath = "Assets/_Project/Data/Progression/PlayerStats.asset";

        private static readonly Color Dim = new Color(0.02f, 0.02f, 0.02f, 0.78f);
        private static readonly Color Cream = new Color32(232, 220, 192, 255);
        private static readonly Color Muted = new Color32(172, 194, 195, 255);
        private static readonly Color ShadowColor = new Color32(19, 20, 20, 255);
        private static readonly Color Warning = new Color32(201, 109, 69, 255);

        private static Font font;

        // Devolve o painel de expedição e o menu do padre, para o builder ligar o clique do portão e da igreja
        public static ExpeditionPanel Build(out PriestMenu priestMenu)
        {
            font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);

            // Screen Space - Overlay: o raycast da UI vem antes do Physics2DRaycaster da câmera,
            // então o fundo do painel bloqueia o clique e o contorno no mundo (HUB-14)
            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            canvasGo.layer = LayerMask.NameToLayer("UI");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            BuildResourceBar(canvasGo.transform);
            priestMenu = BuildPriestMenu(canvasGo.transform, BuildAttributeRowPrefab());
            return BuildExpeditionPanel(canvasGo.transform);
        }

        // Faixa no topo com os 5 recursos do save (HUB-08)
        private static void BuildResourceBar(Transform canvas)
        {
            var bar = Node("Recursos", canvas);
            Img(bar, Art("SubPanelFrame"), Color.white, tiled: true).raycastTarget = false;
            var rect = bar.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(1008, 120);
            rect.anchoredPosition = new Vector2(0, -16);
            var row = bar.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(48, 48, 24, 24);
            row.childAlignment = TextAnchor.MiddleCenter;
            SetControl(row, forceWidth: true, forceHeight: true);

            var fields = new SerializedObject(bar.AddComponent<ResourceBar>());
            fields.FindProperty("gold").objectReferenceValue = Counter(bar.transform, "Ouro", "IconGold");
            fields.FindProperty("oil").objectReferenceValue = Counter(bar.transform, "Oleo", "IconOil");
            fields.FindProperty("bones").objectReferenceValue = Counter(bar.transform, "Osso", "IconBone");
            fields.FindProperty("iron").objectReferenceValue = Counter(bar.transform, "Ferro", "IconIron");
            fields.FindProperty("wax").objectReferenceValue = Counter(bar.transform, "Cera", "IconWax");
            fields.ApplyModifiedPropertiesWithoutUndo();
        }

        private static ResourceCounterView Counter(Transform parent, string name, string icon)
        {
            var item = Node(name, parent);
            var row = item.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 12;
            row.childAlignment = TextAnchor.MiddleCenter;
            SetControl(row, forceWidth: false, forceHeight: false);

            var image = Node("Icone", item.transform);
            Img(image, Art(icon), Color.white).raycastTarget = false;
            Size(image, 36, 36); // 9 px x 4
            var value = Label("Valor", item.transform, "0", 36, Cream, TextAnchor.MiddleLeft);
            value.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            Size(value, height: 48);

            var view = new SerializedObject(item.AddComponent<ResourceCounterView>());
            view.FindProperty("value").objectReferenceValue = value.GetComponent<Text>();
            view.ApplyModifiedPropertiesWithoutUndo();
            return item.GetComponent<ResourceCounterView>();
        }

        // "Partir para a expedição?": o HubPanel fica num objeto sempre ativo e liga o Conteudo (fundo + painel)
        private static ExpeditionPanel BuildExpeditionPanel(Transform canvas)
        {
            var host = Node("PainelExpedicao", canvas);
            Stretch(host);
            var hubPanel = host.AddComponent<HubPanel>();
            var expedition = host.AddComponent<ExpeditionPanel>();

            // Fundo em tela cheia: escurece o hub e segura os cliques
            var root = Node("Conteudo", host.transform);
            Stretch(root);
            Img(root, null, Dim);

            var panel = Node("Painel", root.transform);
            Img(panel, Art("PanelFrame"), Color.white, tiled: true);
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(1134, 546); // múltiplos do ornamento de 63 unidades
            var column = panel.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(80, 80, 76, 64);
            column.spacing = 10;
            column.childAlignment = TextAnchor.MiddleCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            Size(Label("Titulo", panel.transform, "Partir para a expedição?", 48, Cream, TextAnchor.MiddleCenter), height: 60);
            Ornament(panel.transform, "Arabesco", Art("TitleFlourish"));
            var map = Label("Mapa", panel.transform, "Cemitério - Entrada", 32, Cream, TextAnchor.MiddleCenter);
            Size(map, height: 44);
            var deck = Label("Deck", panel.transform, "Deck: 15 cartas", 28, Muted, TextAnchor.MiddleCenter);
            Size(deck, height: 38);
            var warning = Label("AvisoDeck", panel.transform, "Monte um deck de 10 a 15 cartas na mesa de tarot.", 24, new Color32(201, 109, 69, 255), TextAnchor.MiddleCenter);
            Size(warning, height: 34);

            var buttons = Node("Botoes", panel.transform);
            var row = buttons.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 24;
            row.childAlignment = TextAnchor.MiddleCenter;
            SetControl(row, forceWidth: false, forceHeight: true);
            Size(buttons, height: 72); // altura nativa da barra (24 px x 3)
            var depart = Button(buttons.transform, "Partir", expedition.Depart);
            Button(buttons.transform, "Ficar", expedition.Stay);

            var hubFields = new SerializedObject(hubPanel);
            hubFields.FindProperty("root").objectReferenceValue = root;
            hubFields.ApplyModifiedPropertiesWithoutUndo();

            var fields = new SerializedObject(expedition);
            fields.FindProperty("panel").objectReferenceValue = hubPanel;
            fields.FindProperty("mapLabel").objectReferenceValue = map.GetComponent<Text>();
            fields.FindProperty("deckLabel").objectReferenceValue = deck.GetComponent<Text>();
            fields.FindProperty("invalidDeckLabel").objectReferenceValue = warning;
            fields.FindProperty("departButton").objectReferenceValue = depart;
            fields.ApplyModifiedPropertiesWithoutUndo();

            root.SetActive(false);
            return expedition;
        }

        // Menu do padre: nível, custo do próximo ponto e uma linha por atributo, criadas em runtime pelo PriestMenu
        private static PriestMenu BuildPriestMenu(Transform canvas, AttributeRowView rowPrefab)
        {
            var host = Node("PainelPadre", canvas);
            Stretch(host);
            var hubPanel = host.AddComponent<HubPanel>();
            var menu = host.AddComponent<PriestMenu>();

            var root = Node("Conteudo", host.transform);
            Stretch(root);
            Img(root, null, Dim); // fundo em tela cheia: escurece o hub e segura os cliques

            var panel = Node("Painel", root.transform);
            Img(panel, Art("PanelFrame"), Color.white, tiled: true);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(1386, 819); // múltiplos do ornamento de 63 unidades
            panelRect.anchoredPosition = new Vector2(0, -50); // abaixo da barra de recursos
            var column = panel.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(84, 84, 72, 60);
            column.spacing = 8;
            column.childAlignment = TextAnchor.UpperCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            Size(Label("Titulo", panel.transform, "O Padre", 48, Cream, TextAnchor.MiddleCenter), height: 60);
            Ornament(panel.transform, "Arabesco", Art("TitleFlourish"));

            var header = Node("Cabecalho", panel.transform);
            var headerRow = header.AddComponent<HorizontalLayoutGroup>();
            headerRow.spacing = 60;
            headerRow.childAlignment = TextAnchor.MiddleCenter;
            SetControl(headerRow, forceWidth: false, forceHeight: true);
            Size(header, height: 48);
            var level = Label("Nivel", header.transform, "Nível 0", 32, Cream, TextAnchor.MiddleCenter);
            level.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            var cost = Label("Custo", header.transform, "Próximo ponto: 5 de ouro", 28, Cream, TextAnchor.MiddleCenter);
            cost.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;

            var rows = Node("Atributos", panel.transform);
            var rowsLayout = rows.AddComponent<VerticalLayoutGroup>();
            rowsLayout.spacing = 6;
            rowsLayout.childAlignment = TextAnchor.UpperCenter;
            SetControl(rowsLayout, forceWidth: true, forceHeight: false);
            rows.AddComponent<LayoutElement>().flexibleHeight = 1;

            var footer = Node("Rodape", panel.transform);
            var footerRow = footer.AddComponent<HorizontalLayoutGroup>();
            footerRow.childAlignment = TextAnchor.MiddleCenter;
            SetControl(footerRow, forceWidth: false, forceHeight: true);
            Size(footer, height: 72);
            Button(footer.transform, "Fechar", menu.Close);

            var hubFields = new SerializedObject(hubPanel);
            hubFields.FindProperty("root").objectReferenceValue = root;
            hubFields.ApplyModifiedPropertiesWithoutUndo();

            var fields = new SerializedObject(menu);
            fields.FindProperty("panel").objectReferenceValue = hubPanel;
            fields.FindProperty("baseStats").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PlayerStats>(PlayerStatsPath);
            fields.FindProperty("levelLabel").objectReferenceValue = level.GetComponent<Text>();
            fields.FindProperty("costLabel").objectReferenceValue = cost.GetComponent<Text>();
            fields.FindProperty("rowsContainer").objectReferenceValue = rows.transform;
            fields.FindProperty("rowPrefab").objectReferenceValue = rowPrefab;
            fields.FindProperty("costColor").colorValue = Cream;
            fields.FindProperty("missingGoldColor").colorValue = Warning;
            fields.ApplyModifiedPropertiesWithoutUndo();

            root.SetActive(false);
            return menu;
        }

        // Prefab de uma linha: ícone, nome e nível, os efeitos "atual → próximo" e o botão "Abençoar"
        private static AttributeRowView BuildAttributeRowPrefab()
        {
            var row = new GameObject("AttributeRow", typeof(RectTransform));
            row.layer = LayerMask.NameToLayer("UI");
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 6, 6);
            layout.spacing = 24;
            layout.childAlignment = TextAnchor.MiddleLeft;
            SetControl(layout, forceWidth: false, forceHeight: false);
            row.AddComponent<LayoutElement>().minHeight = 84;

            var icon = Node("Icone", row.transform);
            var iconImage = Img(icon, null, Color.white);
            iconImage.raycastTarget = false;
            Size(icon, 45, 45); // 9 px x 5

            var title = Node("Nome", row.transform);
            var titleColumn = title.AddComponent<VerticalLayoutGroup>();
            titleColumn.childAlignment = TextAnchor.MiddleLeft;
            SetControl(titleColumn, forceWidth: true, forceHeight: false);
            Size(title, width: 240);
            title.GetComponent<LayoutElement>().flexibleWidth = 0; // o espaço livre vai todo para os efeitos
            var name = Label("Atributo", title.transform, "Vitalidade", 32, Cream, TextAnchor.MiddleLeft);
            Size(name, height: 40);
            var level = Label("Nivel", title.transform, "Nv 0", 24, Muted, TextAnchor.MiddleLeft);
            Size(level, height: 30);

            var effects = Label("Efeitos", row.transform, "Vida máxima  100 → 110", 22, Cream, TextAnchor.MiddleLeft);
            effects.GetComponent<Text>().lineSpacing = 1.1f;
            effects.AddComponent<LayoutElement>().flexibleWidth = 1;

            var buy = Button(row.transform, "Abençoar", null);
            Size(buy.gameObject, height: 72);

            var view = row.AddComponent<AttributeRowView>();
            var fields = new SerializedObject(view);
            fields.FindProperty("icon").objectReferenceValue = iconImage;
            fields.FindProperty("nameLabel").objectReferenceValue = name.GetComponent<Text>();
            fields.FindProperty("levelLabel").objectReferenceValue = level.GetComponent<Text>();
            fields.FindProperty("effectsLabel").objectReferenceValue = effects.GetComponent<Text>();
            fields.FindProperty("buyButton").objectReferenceValue = buy;
            fields.ApplyModifiedPropertiesWithoutUndo();

            Directory.CreateDirectory(Path.GetDirectoryName(AttributeRowPath));
            var prefab = PrefabUtility.SaveAsPrefabAsset(row, AttributeRowPath);
            Object.DestroyImmediate(row);
            return prefab.GetComponent<AttributeRowView>();
        }

        // Botão da barra gótica; escurece quando desligado (o "Partir" com deck inválido)
        private static Button Button(Transform parent, string text, UnityAction onClick)
        {
            var go = Node(text, parent);
            var image = Img(go, Art("ButtonNormal"), Color.white); // já desenhado na largura final (150 px x 3)
            Size(go, width: 450);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = new Color(0.85f, 0.85f, 0.85f);
            colors.highlightedColor = Color.white;
            colors.selectedColor = new Color(0.85f, 0.85f, 0.85f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
            colors.disabledColor = new Color(0.35f, 0.35f, 0.35f, 0.6f);
            button.colors = colors;
            if (onClick != null) // a linha de atributo liga o clique em runtime
                UnityEventTools.AddPersistentListener(button.onClick, onClick);
            var label = Label("Texto", go.transform, text, 28, Cream, TextAnchor.MiddleCenter);
            Stretch(label);
            return button;
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

        private static Image Img(GameObject go, Sprite sprite, Color color, bool tiled = false)
        {
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = tiled ? Image.Type.Tiled : Image.Type.Simple;
            return image;
        }

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
