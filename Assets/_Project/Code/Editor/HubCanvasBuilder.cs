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
        private const string UnlockRowPath = "Assets/_Project/Prefabs/UI/Tarot/UnlockRow.prefab";
        private const string TarotCardSlotPath = "Assets/_Project/Prefabs/UI/Tarot/TarotCardSlot.prefab";
        private const string PlayerStatsPath = "Assets/_Project/Data/Progression/PlayerStats.asset";

        private static readonly Color Dim = new Color(0.02f, 0.02f, 0.02f, 0.78f);
        private static readonly Color Cream = new Color32(232, 220, 192, 255);
        private static readonly Color Muted = new Color32(172, 194, 195, 255);
        private static readonly Color ShadowColor = new Color32(19, 20, 20, 255);
        private static readonly Color Warning = new Color32(201, 109, 69, 255);

        private static Font font;

        // Devolve o painel de expedição e os menus do padre, da fogueira e da mesa de tarot, para o builder ligar os cliques do mundo
        public static ExpeditionPanel Build(out PriestMenu priestMenu, out CampfireMenu campfireMenu, out TarotMenu tarotMenu)
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
            campfireMenu = BuildCampfireMenu(canvasGo.transform);
            tarotMenu = BuildTarotMenu(canvasGo.transform, BuildUnlockRowPrefab(), BuildTarotCardSlotPrefab());
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

        // Menu da fogueira: nível de luz, raio atual → próximo, custo em óleo e "Alimentar a chama"
        private static CampfireMenu BuildCampfireMenu(Transform canvas)
        {
            var host = Node("PainelFogueira", canvas);
            Stretch(host);
            var hubPanel = host.AddComponent<HubPanel>();
            var menu = host.AddComponent<CampfireMenu>();

            var root = Node("Conteudo", host.transform);
            Stretch(root);
            Img(root, null, Dim); // fundo em tela cheia: escurece o hub e segura os cliques

            var panel = Node("Painel", root.transform);
            Img(panel, Art("PanelFrame"), Color.white, tiled: true);
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(1134, 546); // múltiplos do ornamento de 63 unidades
            var column = panel.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(80, 80, 76, 64);
            column.spacing = 10;
            column.childAlignment = TextAnchor.MiddleCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            Size(Label("Titulo", panel.transform, "A Fogueira", 48, Cream, TextAnchor.MiddleCenter), height: 60);
            Ornament(panel.transform, "Arabesco", Art("TitleFlourish"));
            var level = Label("Nivel", panel.transform, "Nível 0 de 10", 32, Cream, TextAnchor.MiddleCenter);
            Size(level, height: 44);
            var radius = Label("Raio", panel.transform, "Raio da luz  3,0 → 3,5", 28, Muted, TextAnchor.MiddleCenter);
            Size(radius, height: 38);
            var cost = Label("Custo", panel.transform, "Custo: 40 de óleo", 28, Cream, TextAnchor.MiddleCenter);
            Size(cost, height: 38);

            var buttons = Node("Botoes", panel.transform);
            var row = buttons.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 24;
            row.childAlignment = TextAnchor.MiddleCenter;
            SetControl(row, forceWidth: false, forceHeight: true);
            Size(buttons, height: 72);
            var buy = Button(buttons.transform, "Alimentar a chama", menu.Buy);
            Button(buttons.transform, "Fechar", menu.Close);

            var hubFields = new SerializedObject(hubPanel);
            hubFields.FindProperty("root").objectReferenceValue = root;
            hubFields.ApplyModifiedPropertiesWithoutUndo();

            var fields = new SerializedObject(menu);
            fields.FindProperty("panel").objectReferenceValue = hubPanel;
            fields.FindProperty("levelLabel").objectReferenceValue = level.GetComponent<Text>();
            fields.FindProperty("radiusLabel").objectReferenceValue = radius.GetComponent<Text>();
            fields.FindProperty("costLabel").objectReferenceValue = cost.GetComponent<Text>();
            fields.FindProperty("buyButton").objectReferenceValue = buy;
            fields.FindProperty("costColor").colorValue = Cream;
            fields.FindProperty("missingOilColor").colorValue = Warning; // cor de aviso, como o padre (LIGHT-17)
            fields.ApplyModifiedPropertiesWithoutUndo();

            root.SetActive(false);
            return menu;
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

        // "A Mesa de Tarot": botões das abas Desbloquear, Coleção e Deck, a aba aberta e "Fechar".
        // As linhas e as células de carta são criadas em runtime pelas abas.
        private static TarotMenu BuildTarotMenu(Transform canvas, UnlockRowView rowPrefab, CardSlotView slotPrefab)
        {
            var host = Node("PainelMesaTarot", canvas);
            Stretch(host);
            var hubPanel = host.AddComponent<HubPanel>();
            var menu = host.AddComponent<TarotMenu>();

            var root = Node("Conteudo", host.transform);
            Stretch(root);
            Img(root, null, Dim); // fundo em tela cheia: escurece o hub e segura os cliques

            var panel = Node("Painel", root.transform);
            Img(panel, Art("PanelFrame"), Color.white, tiled: true);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(1638, 882); // múltiplos do ornamento de 63 unidades
            panelRect.anchoredPosition = new Vector2(0, -55); // abaixo da barra de recursos
            var column = panel.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(84, 84, 72, 56);
            column.spacing = 8;
            column.childAlignment = TextAnchor.UpperCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            Size(Label("Titulo", panel.transform, "A Mesa de Tarot", 48, Cream, TextAnchor.MiddleCenter), height: 60);
            Ornament(panel.transform, "Arabesco", Art("TitleFlourish"));

            var tabs = Node("Abas", panel.transform);
            var tabsRow = tabs.AddComponent<HorizontalLayoutGroup>();
            tabsRow.spacing = 24;
            tabsRow.childAlignment = TextAnchor.MiddleCenter;
            SetControl(tabsRow, forceWidth: false, forceHeight: true);
            Size(tabs, height: 72);
            tabs.GetComponent<LayoutElement>().flexibleHeight = 0; // a altura que sobra vai toda para a aba
            string[] tabNames = { "Desbloquear", "Coleção", "Deck" };
            var tabLabels = new Text[tabNames.Length];
            for (int i = 0; i < tabNames.Length; i++)
            {
                var tabButton = Button(tabs.transform, tabNames[i], null);
                UnityEventTools.AddIntPersistentListener(tabButton.onClick, menu.ShowTab, i);
                tabLabels[i] = tabButton.transform.Find("Texto").GetComponent<Text>();
            }

            var content = Node("Aba", panel.transform);
            content.AddComponent<LayoutElement>().flexibleHeight = 1;
            var unlockTab = BuildUnlockTab(content.transform, rowPrefab);
            var collectionTab = BuildCollectionTab(content.transform, slotPrefab);
            var deckTab = BuildDeckTab(content.transform, slotPrefab);
            collectionTab.gameObject.SetActive(false);
            deckTab.gameObject.SetActive(false);

            var footer = Node("Rodape", panel.transform);
            var footerRow = footer.AddComponent<HorizontalLayoutGroup>();
            footerRow.childAlignment = TextAnchor.MiddleCenter;
            SetControl(footerRow, forceWidth: false, forceHeight: true);
            Size(footer, height: 72);
            footer.GetComponent<LayoutElement>().flexibleHeight = 0;
            Button(footer.transform, "Fechar", menu.Close);

            var hubFields = new SerializedObject(hubPanel);
            hubFields.FindProperty("root").objectReferenceValue = root;
            hubFields.ApplyModifiedPropertiesWithoutUndo();

            var fields = new SerializedObject(menu);
            fields.FindProperty("panel").objectReferenceValue = hubPanel;
            fields.FindProperty("unlockTab").objectReferenceValue = unlockTab;
            fields.FindProperty("collectionTab").objectReferenceValue = collectionTab;
            fields.FindProperty("deckTab").objectReferenceValue = deckTab;
            var labels = fields.FindProperty("tabLabels");
            labels.arraySize = tabLabels.Length;
            for (int i = 0; i < tabLabels.Length; i++)
                labels.GetArrayElementAtIndex(i).objectReferenceValue = tabLabels[i];
            fields.FindProperty("selectedTabColor").colorValue = Cream;
            fields.FindProperty("idleTabColor").colorValue = new Color32(128, 128, 120, 255); // abas fechadas apagadas
            fields.ApplyModifiedPropertiesWithoutUndo();

            root.SetActive(false);
            return menu;
        }

        // Aba Desbloquear: lista das cartas bloqueadas, ou o aviso de que todas foram liberadas (TAROT-21)
        private static TarotUnlockTab BuildUnlockTab(Transform parent, UnlockRowView rowPrefab)
        {
            var tab = Node("Desbloquear", parent);
            Stretch(tab);
            var view = tab.AddComponent<TarotUnlockTab>();

            var scroll = Scroll(tab.transform, "Lista", out var rows);
            Stretch(scroll);
            var rowsLayout = rows.gameObject.AddComponent<VerticalLayoutGroup>();
            rowsLayout.spacing = 6;
            rowsLayout.childAlignment = TextAnchor.UpperCenter;
            SetControl(rowsLayout, forceWidth: true, forceHeight: false);

            var empty = Label("TodasLiberadas", tab.transform, "Todas as cartas foram liberadas.", 32, Muted, TextAnchor.MiddleCenter);
            Stretch(empty);
            empty.SetActive(false);

            var fields = new SerializedObject(view);
            fields.FindProperty("rowsContainer").objectReferenceValue = rows;
            fields.FindProperty("rowPrefab").objectReferenceValue = rowPrefab;
            fields.FindProperty("allUnlockedLabel").objectReferenceValue = empty;
            fields.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        // Aba Coleção: "Liberadas: N/M" e a grade de todas as cartas à esquerda, o detalhe à direita
        private static TarotCollectionTab BuildCollectionTab(Transform parent, CardSlotView slotPrefab)
        {
            var tab = Node("Colecao", parent);
            Stretch(tab);
            var view = tab.AddComponent<TarotCollectionTab>();
            var row = tab.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 30;
            SetControl(row, forceWidth: false, forceHeight: true);

            var left = Node("Cartas", tab.transform);
            var leftColumn = left.AddComponent<VerticalLayoutGroup>();
            leftColumn.spacing = 8;
            SetControl(leftColumn, forceWidth: true, forceHeight: false);
            Size(left, width: 900);
            left.GetComponent<LayoutElement>().minWidth = 900; // 7 células de 120 com o espaço entre elas
            var unlocked = Label("Liberadas", left.transform, "Liberadas: 15/20", 28, Cream, TextAnchor.MiddleLeft);
            Size(unlocked, height: 40);
            var grid = CardGrid(left.transform, columns: 7);

            var detail = BuildCardDetail(tab.transform);

            var fields = new SerializedObject(view);
            fields.FindProperty("gridContainer").objectReferenceValue = grid;
            fields.FindProperty("slotPrefab").objectReferenceValue = slotPrefab;
            fields.FindProperty("detail").objectReferenceValue = detail;
            fields.FindProperty("unlockedLabel").objectReferenceValue = unlocked.GetComponent<Text>();
            fields.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        // Detalhe da carta selecionada: desenho, nome, descrição, efeitos e, se bloqueada, "Bloqueada" e o custo
        private static CardDetailView BuildCardDetail(Transform parent)
        {
            var detail = Node("Detalhe", parent);
            Img(detail, Art("SubPanelFrame"), Color.white, tiled: true).raycastTarget = false;
            // Largura fixa: sem ela, uma descrição longa empurra a grade de cartas
            var detailSize = detail.AddComponent<LayoutElement>();
            detailSize.minWidth = detailSize.preferredWidth = 540;
            detailSize.flexibleWidth = 1;
            var column = detail.AddComponent<VerticalLayoutGroup>();
            column.padding = new RectOffset(48, 48, 36, 36);
            column.spacing = 8;
            column.childAlignment = TextAnchor.UpperCenter;
            SetControl(column, forceWidth: true, forceHeight: false);

            var frameHolder = Node("Carta", detail.transform);
            Size(frameHolder, height: 80);
            var frame = Node("Moldura", frameHolder.transform);
            Img(frame, Art("CardFrame"), Color.white, sliced: true).raycastTarget = false;
            frame.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 80); // 12x16 px x 5
            var icon = Node("Icone", frame.transform);
            var iconImage = Img(icon, Art("CardIconDefault"), Color.white);
            iconImage.raycastTarget = false;
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 30); // 9x6 px x 5

            var name = Label("Nome", detail.transform, "Carta", 36, Cream, TextAnchor.MiddleCenter);
            Size(name, height: 44);
            var description = Label("Descricao", detail.transform, "Descrição da carta.", 22, Muted, TextAnchor.UpperCenter);
            var effects = Label("Efeitos", detail.transform, "+20% dano", 24, Cream, TextAnchor.UpperCenter);
            effects.GetComponent<Text>().lineSpacing = 1.1f;

            var locked = Node("Bloqueada", detail.transform);
            var lockedColumn = locked.AddComponent<VerticalLayoutGroup>();
            lockedColumn.spacing = 4;
            SetControl(lockedColumn, forceWidth: true, forceHeight: false);
            Size(Label("Marca", locked.transform, "Bloqueada", 28, Warning, TextAnchor.MiddleCenter), height: 36);
            var cost = Label("Custo", locked.transform, "Custo: 5 osso  5 ferro  5 cera", 22, Cream, TextAnchor.MiddleCenter);
            Size(cost, height: 30);

            var view = detail.AddComponent<CardDetailView>();
            var fields = new SerializedObject(view);
            fields.FindProperty("icon").objectReferenceValue = iconImage;
            fields.FindProperty("defaultIcon").objectReferenceValue = Art("CardIconDefault");
            fields.FindProperty("nameLabel").objectReferenceValue = name.GetComponent<Text>();
            fields.FindProperty("descriptionLabel").objectReferenceValue = description.GetComponent<Text>();
            fields.FindProperty("effectsLabel").objectReferenceValue = effects.GetComponent<Text>();
            fields.FindProperty("lockedGroup").objectReferenceValue = locked;
            fields.FindProperty("costLabel").objectReferenceValue = cost.GetComponent<Text>();
            fields.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        // Aba Deck: "Deck: N/15" e o aviso de mínimo no topo, as grades "No deck" e "Disponíveis" lado a lado
        private static TarotDeckTab BuildDeckTab(Transform parent, CardSlotView slotPrefab)
        {
            var tab = Node("Deck", parent);
            Stretch(tab);
            var view = tab.AddComponent<TarotDeckTab>();
            var column = tab.AddComponent<VerticalLayoutGroup>();
            column.spacing = 8;
            SetControl(column, forceWidth: true, forceHeight: false);

            var header = Node("Cabecalho", tab.transform);
            var headerRow = header.AddComponent<HorizontalLayoutGroup>();
            headerRow.spacing = 40;
            headerRow.childAlignment = TextAnchor.MiddleCenter;
            SetControl(headerRow, forceWidth: false, forceHeight: true);
            Size(header, height: 40);
            header.GetComponent<LayoutElement>().flexibleHeight = 0; // a altura que sobra vai para as grades
            var count = Label("Contador", header.transform, "Deck: 15/15", 32, Cream, TextAnchor.MiddleCenter);
            count.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            var warning = Label("AvisoMinimo", header.transform, "O deck precisa de pelo menos 10 cartas.", 24, Warning, TextAnchor.MiddleCenter);
            warning.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;

            var columns = Node("Grades", tab.transform);
            var columnsRow = columns.AddComponent<HorizontalLayoutGroup>();
            columnsRow.spacing = 40;
            SetControl(columnsRow, forceWidth: true, forceHeight: true);
            columns.AddComponent<LayoutElement>().flexibleHeight = 1;
            var deck = DeckColumn(columns.transform, "NoDeck", "No deck");
            var available = DeckColumn(columns.transform, "Disponiveis", "Disponíveis");

            var fields = new SerializedObject(view);
            fields.FindProperty("slotPrefab").objectReferenceValue = slotPrefab;
            fields.FindProperty("deckContainer").objectReferenceValue = deck;
            fields.FindProperty("availableContainer").objectReferenceValue = available;
            fields.FindProperty("countLabel").objectReferenceValue = count.GetComponent<Text>();
            fields.FindProperty("minimumWarning").objectReferenceValue = warning.GetComponent<Text>();
            fields.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static Transform DeckColumn(Transform parent, string name, string title)
        {
            var column = Node(name, parent);
            var layout = column.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8;
            SetControl(layout, forceWidth: true, forceHeight: false);
            Size(Label("Titulo", column.transform, title, 28, Muted, TextAnchor.MiddleCenter), height: 36);
            return CardGrid(column.transform, columns: 5);
        }

        // Grade de células de carta numa área com rolagem, para quando as cartas não couberem
        private static Transform CardGrid(Transform parent, int columns)
        {
            var scroll = Scroll(parent, "Grade", out var content);
            scroll.AddComponent<LayoutElement>().flexibleHeight = 1;
            var grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(120, 124);
            grid.spacing = new Vector2(10, 8);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.childAlignment = TextAnchor.UpperCenter;
            return content;
        }

        // Área com rolagem vertical: o conteúdo cresce com os itens e a máscara corta o que passa da área
        private static GameObject Scroll(Transform parent, string name, out Transform content)
        {
            var scroll = Node(name, parent);
            Img(scroll, null, Color.clear); // recebe a roda do mouse entre as células
            var scrollRect = scroll.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 40;

            var viewport = Node("Viewport", scroll.transform);
            Stretch(viewport);
            viewport.AddComponent<RectMask2D>();

            var items = Node("Itens", viewport.transform);
            var itemsRect = items.GetComponent<RectTransform>();
            itemsRect.anchorMin = new Vector2(0, 1);
            itemsRect.anchorMax = new Vector2(1, 1);
            itemsRect.pivot = new Vector2(0.5f, 1);
            itemsRect.sizeDelta = Vector2.zero;
            items.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.content = itemsRect;
            content = items.transform;
            return scroll;
        }

        // Prefab de uma linha da aba Desbloquear: carta, nome, custo em osso, ferro e cera e "Desbloquear"
        private static UnlockRowView BuildUnlockRowPrefab()
        {
            var row = new GameObject("UnlockRow", typeof(RectTransform));
            row.layer = LayerMask.NameToLayer("UI");
            try
            {
                var layout = row.AddComponent<HorizontalLayoutGroup>();
                layout.padding = new RectOffset(16, 16, 6, 6);
                layout.spacing = 24;
                layout.childAlignment = TextAnchor.MiddleLeft;
                SetControl(layout, forceWidth: false, forceHeight: false);
                row.AddComponent<LayoutElement>().minHeight = 84;

                var frame = Node("Moldura", row.transform);
                Img(frame, Art("CardFrame"), Color.white, sliced: true).raycastTarget = false;
                Size(frame, 48, 64); // 12x16 px x 4
                var icon = Node("Icone", frame.transform);
                var iconImage = Img(icon, Art("CardIconDefault"), Color.white);
                iconImage.raycastTarget = false;
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(27, 18); // 9x6 px x 3

                var name = Label("Nome", row.transform, "Carta", 32, Cream, TextAnchor.MiddleLeft);
                Size(name, height: 44);
                name.GetComponent<LayoutElement>().flexibleWidth = 1;

                var bones = Cost(row.transform, "Osso", "IconBone");
                var iron = Cost(row.transform, "Ferro", "IconIron");
                var wax = Cost(row.transform, "Cera", "IconWax");

                var unlock = Button(row.transform, "Desbloquear", null); // a linha liga o clique em runtime
                Size(unlock.gameObject, height: 72);

                var view = row.AddComponent<UnlockRowView>();
                var fields = new SerializedObject(view);
                fields.FindProperty("icon").objectReferenceValue = iconImage;
                fields.FindProperty("defaultIcon").objectReferenceValue = Art("CardIconDefault");
                fields.FindProperty("nameLabel").objectReferenceValue = name.GetComponent<Text>();
                fields.FindProperty("bonesLabel").objectReferenceValue = bones;
                fields.FindProperty("ironLabel").objectReferenceValue = iron;
                fields.FindProperty("waxLabel").objectReferenceValue = wax;
                fields.FindProperty("costColor").colorValue = Cream;
                fields.FindProperty("missingColor").colorValue = Warning; // TAROT-17
                fields.FindProperty("unlockButton").objectReferenceValue = unlock;
                fields.ApplyModifiedPropertiesWithoutUndo();

                Directory.CreateDirectory(Path.GetDirectoryName(UnlockRowPath));
                return PrefabUtility.SaveAsPrefabAsset(row, UnlockRowPath).GetComponent<UnlockRowView>();
            }
            finally
            {
                Object.DestroyImmediate(row);
            }
        }

        // Ícone do material e o número do custo
        private static Text Cost(Transform parent, string name, string icon)
        {
            var item = Node(name, parent);
            var row = item.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 8;
            row.childAlignment = TextAnchor.MiddleLeft;
            SetControl(row, forceWidth: false, forceHeight: false);

            var image = Node("Icone", item.transform);
            Img(image, Art(icon), Color.white).raycastTarget = false;
            Size(image, 36, 36); // 9 px x 4
            var value = Label("Valor", item.transform, "5", 32, Cream, TextAnchor.MiddleLeft);
            Size(value, 48, 44);
            return value.GetComponent<Text>();
        }

        // Prefab da célula de carta da mesa: moldura com o desenho, "Bloqueada" por cima e o nome embaixo;
        // a célula inteira é o botão
        private static CardSlotView BuildTarotCardSlotPrefab()
        {
            var slot = new GameObject("TarotCardSlot", typeof(RectTransform));
            slot.layer = LayerMask.NameToLayer("UI");
            try
            {
                slot.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 124);
                Img(slot, null, Color.clear); // área do clique

                var frame = Node("Moldura", slot.transform);
                var frameImage = Img(frame, Art("CardFrame"), Color.white, sliced: true);
                frameImage.raycastTarget = false;
                var frameRect = frame.GetComponent<RectTransform>();
                frameRect.anchorMin = frameRect.anchorMax = frameRect.pivot = new Vector2(0.5f, 1f);
                frameRect.anchoredPosition = new Vector2(0, -4);
                frameRect.sizeDelta = new Vector2(48, 64); // 12x16 px x 4

                var icon = Node("Icone", frame.transform);
                var iconImage = Img(icon, Art("CardIconDefault"), Color.white);
                iconImage.raycastTarget = false;
                iconImage.SetNativeSize();

                var locked = Label("Bloqueada", frame.transform, "Bloqueada", 18, Warning, TextAnchor.MiddleCenter);
                locked.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                var lockedRect = locked.GetComponent<RectTransform>();
                lockedRect.anchorMin = lockedRect.anchorMax = lockedRect.pivot = new Vector2(0.5f, 0f);
                lockedRect.anchoredPosition = new Vector2(0, 2);
                lockedRect.sizeDelta = new Vector2(48, 22);
                locked.SetActive(false);

                var name = Label("Nome", slot.transform, "Carta", 18, Muted, TextAnchor.UpperCenter);
                var nameRect = name.GetComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0, 0);
                nameRect.anchorMax = new Vector2(1, 0);
                nameRect.pivot = new Vector2(0.5f, 0);
                nameRect.sizeDelta = new Vector2(0, 50);

                var button = slot.AddComponent<Button>();
                button.targetGraphic = frameImage;
                button.transition = Selectable.Transition.ColorTint;
                var colors = button.colors;
                colors.normalColor = new Color(0.85f, 0.85f, 0.85f);
                colors.highlightedColor = Color.white;
                colors.selectedColor = new Color(0.85f, 0.85f, 0.85f);
                colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
                colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.8f); // deck cheio: a disponível não entra
                button.colors = colors;

                var view = new SerializedObject(slot.AddComponent<CardSlotView>());
                view.FindProperty("icon").objectReferenceValue = iconImage;
                view.FindProperty("cardName").objectReferenceValue = name.GetComponent<Text>();
                view.FindProperty("defaultIcon").objectReferenceValue = Art("CardIconDefault");
                view.FindProperty("button").objectReferenceValue = button;
                view.FindProperty("lockedMark").objectReferenceValue = locked;
                view.ApplyModifiedPropertiesWithoutUndo();

                Directory.CreateDirectory(Path.GetDirectoryName(TarotCardSlotPath));
                return PrefabUtility.SaveAsPrefabAsset(slot, TarotCardSlotPath).GetComponent<CardSlotView>();
            }
            finally
            {
                Object.DestroyImmediate(slot);
            }
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

        private static Image Img(GameObject go, Sprite sprite, Color color, bool tiled = false, bool sliced = false)
        {
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = tiled ? Image.Type.Tiled : sliced ? Image.Type.Sliced : Image.Type.Simple;
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
