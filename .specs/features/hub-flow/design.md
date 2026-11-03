# Hub funcional Design

**Spec**: `.specs/features/hub-flow/spec.md`
**Status**: Approved
**Approved**: 2026-11-03 - user: "Aprovo todos"

---

## Architecture Overview

O hub ganha três peças:

- **Portão.** Um objeto do mundo com o `HubInteractable` que já existe. O clique abre o painel de expedição.
- **`HubPanel`.** Um componente de painel reutilizável, que também vai servir aos menus do padre, da fogueira, da mesa e do ferreiro. Ele abre, fecha, responde ao Esc e põe um fundo escuro que bloqueia o mundo.
- **`ResourceBar`.** A barra de recursos no topo da tela, que se redesenha quando o save avisa `Changed`.

Os cliques e o contorno do mundo são bloqueados sem código (HUB-14). O painel tem uma `Image` de fundo em tela cheia com `raycastTarget`. O `EventSystem` dá prioridade ao `GraphicRaycaster` do Canvas em modo overlay sobre o `Physics2DRaycaster` da câmera. Assim o mundo não recebe enter, exit nem clique enquanto o fundo estiver ativo.

```mermaid
sequenceDiagram
    actor J as Jogador
    participant Gt as Portão (HubInteractable)
    participant EP as ExpeditionPanel
    participant HP as HubPanel
    participant S as SaveSystem
    participant SM as SceneManager

    J->>Gt: clique
    Gt->>EP: onClick → Open()
    EP->>S: Data.Deck.Count
    EP->>EP: deck válido? liga ou desliga "Partir" (HUB-13)
    EP->>HP: Open() (fundo + painel)
    alt Partir
        J->>EP: Partir
        EP->>EP: loading = true (HUB-15)
        EP->>SM: Time.timeScale = 1; LoadScene("FirstOne")
    else Ficar ou Esc
        J->>HP: Close()
    end
```

---

## Code Reuse Analysis

### Existing Components to Leverage

| Component | Location | How to Use |
| --------- | -------- | ---------- |
| `HubInteractable` | `Assets/_Project/Code/Runtime/Hub/HubInteractable.cs` | O portão usa sem mudança: contorno (HUB-02) e `onClick`. |
| `HubSceneBuilder` | `Assets/_Project/Code/Editor/HubSceneBuilder.cs` | Ganha o portão no fim da trilha de baixo, o Canvas do hub, a `ResourceBar` e o painel. |
| `RunResultPanel.ReturnToHub` | `Assets/_Project/Code/Runtime/UI/RunResultPanel.cs` | Já volta ao `Hub` com `timeScale` 1; o save já foi gravado pelo `RunFinisher` (HUB-10). |
| Arte gótica (`PanelFrame`, `ButtonNormal/Hover`, Pixelify Sans) | `Assets/_Project/Art/Sprites/UI/Victory/` | Mesma moldura e botões no painel de expedição e nos menus do hub. |
| `SaveSystem.Changed` | feature `save-system` | Fonte da `ResourceBar` (HUB-09). |

### Integration Points

| System | Integration Method |
| ------ | ------------------ |
| `save-system` | Lê `Data.Resources` e `Data.Deck`. |
| `tarot-table` | Usa `GameDatabase.StartingDeck.MinCards`/`MaxCards` para a validade do deck; a mesa é onde o deck é corrigido. |
| Menus das outras features | Cada menu é um `HubPanel`. |
| `ProjectSettings/EditorBuildSettings.asset` | Ordem `Hub`, `FirstOne`; `SampleScene` sai (HUB-11, HUB-12). |

---

## Components

| Component | Quem escreve |
| --------- | ------------ |
| `HubPanel`, `ExpeditionPanel` | Usuário |
| `ResourceBar`, `ResourceCounterView`, arte do portão, Canvas do hub, ícones dos recursos, builder, build settings | Claude |

### HubPanel

- **Purpose**: Abrir e fechar um painel do hub, com Esc e fundo que bloqueia o mundo.
- **Location**: `Assets/_Project/Code/Runtime/Hub/HubPanel.cs`
- **Interfaces**:
  - `void Open()` e `void Close()` - liga e desliga o objeto `root` (fundo + painel)
  - `static HubPanel Current` - o painel aberto; abrir outro fecha o atual
  - `event Action Opened` e `event Action Closed` - para cada menu atualizar seu conteúdo ao abrir
  - `Update`: com o painel aberto, `Keyboard.current.escapeKey.wasPressedThisFrame` chama `Close()` (HUB-06)
- **Dependencies**: Input System.
- **Reuses**: nada.

### ExpeditionPanel

- **Purpose**: O conteúdo do painel "Partir para a expedição?" (HUB-03 a HUB-05, HUB-13, HUB-15).
- **Location**: `Assets/_Project/Code/Runtime/Hub/ExpeditionPanel.cs`
- **Interfaces**:
  - `void Open()` - chamado pelo `onClick` do portão; preenche o nome do mapa e "Deck: N cartas", liga ou desliga "Partir" e o aviso, e chama `panel.Open()`
  - `void Depart()` - botão "Partir"
  - `void Stay()` - botão "Ficar", chama `panel.Close()`
  - campos `[SerializeField] string mapName = "Cemitério - Entrada"`, `string runSceneName = "FirstOne"`
- **Dependencies**: `HubPanel`, `SaveSystem`, `SceneManager`.
- **Reuses**: o padrão de `RunResultPanel.ReturnToHub`.

### ResourceBar e ResourceCounterView

- **Purpose**: Mostrar ouro, óleo, osso, ferro e cera o tempo todo (HUB-08, HUB-09).
- **Location**: `Assets/_Project/Code/Runtime/Hub/ResourceBar.cs` e `Assets/_Project/Code/Runtime/UI/ResourceCounterView.cs`
- **Interfaces**: `ResourceBar` assina `SaveSystem.Changed` no `OnEnable` e chama `Refresh()`; `ResourceCounterView.SetValue(int)`.
- **Layout**: faixa no topo, com 5 contadores (ícone + número) numa moldura fina.
- **Reuses**: os ícones de recurso criados em `run-rewards`.

### Portão

- **Purpose**: A saída do acampamento (HUB-01).
- **Location**: sprite em `Assets/_Project/Art/Sprites/Hub/Gate.png`, objeto criado pelo `HubSceneBuilder` no fim da trilha de baixo.
- **Arte**: grade de cemitério com dois pilares de pedra, montada com peças do tileset e do Graveyard Set na paleta do hub, com 2 px de margem para o contorno.

---

## Error Handling Strategy

| Error Scenario | Handling | User Impact |
| -------------- | -------- | ----------- |
| Deck fora de 10 a 15 | "Partir" desligado e aviso "Monte um deck de 10 a 15 cartas na mesa de tarot." (HUB-13) | Precisa ir à mesa de tarot. |
| Clique duplo em "Partir" | `loading` ignora o segundo (HUB-15) | Carrega uma vez. |
| Cena da run fora do build | `Application.CanStreamedLevelBeLoaded` falso → `Debug.LogError` e o botão volta a funcionar | Fica no hub. |
| Esc com dois painéis | Só existe um `Current`; Esc fecha esse | Nenhum. |

---

## Risks & Concerns

| Concern | Location (file:line) | Impact | Mitigation |
| ------- | -------------------- | ------ | ---------- |
| `HubSceneBuilder` remonta a cena inteira | `Assets/_Project/Code/Editor/HubSceneBuilder.cs` | Ajustes feitos à mão na cena do Hub se perdem ao rodar o builder de novo. | Tudo do hub (portão, Canvas, painéis) entra pelo builder; o usuário liga o código pelos componentes que o builder já põe. |
| `Time.timeScale` fica 0 se a cena da run for aberta por outro caminho | `Assets/_Project/Code/Runtime/UI/RunResultPanel.cs` | A run começaria parada. | `ExpeditionPanel.Depart` também põe 1 antes de carregar (HUB-07). |
| Ordem do raycast UI contra mundo depende do Canvas estar em Screen Space - Overlay | Canvas do hub (novo) | Com Canvas em Camera, o mundo poderia receber o clique. | O builder cria o Canvas em Screen Space - Overlay. |

---

## Tech Decisions (only non-obvious ones)

| Decision | Choice | Rationale |
| -------- | ------ | --------- |
| Bloquear o mundo com o painel aberto | Fundo `Image` com `raycastTarget`, sem flag no `HubInteractable` | Usa a prioridade de raycast que a Unity já tem; nenhum código novo. |
| Um painel aberto por vez | `HubPanel.Current` | Esc fica sem ambiguidade. |
| `HubPanel` serve a todos os menus | Componente único | AD-011: os menus do hub reutilizam o mesmo painel. |
