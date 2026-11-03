# Hub funcional Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/hub-flow/design.md`
**Status**: Approved
**Approved**: 2026-11-03 - user: "Aprovo todas"
**Build**: none - o projeto não tem comando de build; a compilação é conferida no Console do Unity Editor

---

## Test Coverage Matrix

> Generated from codebase, project guidelines, and spec - confirm before Execute. Guidelines found: none. A spec escolheu conferência manual em Play Mode; o projeto não tem `.asmdef` nem testes.

| Code Layer | Required Test Type | Coverage Expectation | Location Pattern | Run Command |
| ---------- | ------------------ | -------------------- | ---------------- | ----------- |
| Regras puras e componentes de runtime | none | Cada critério conferido no walk em Play Mode | - | Play Mode no Unity Editor |
| Telas, assets e cenas | none | Cada critério conferido no walk em Play Mode | - | Play Mode no Unity Editor |

## Gate Check Commands

> Generated from codebase - confirm before Execute.

| Gate Level | When to Use | Command |
| ---------- | ----------- | ------- |
| Build | Depois de cada task | Sem comando: o Console do Unity Editor sem erros de compilação |

---

## Execution Plan

Phases are ordered and run sequentially - each phase completes before the next begins, and tasks within a phase execute in order.

### Phase 1: Painéis

O código dos painéis do hub.

```
T1 → T2
```

### Phase 2: Cena e build

Portão, barra de recursos e ordem das cenas.

```
T2 → T3 → T4 → T5 → T6 → T7
```

---

## Task Breakdown

### T1: Criar `HubPanel` (usuário)

**What**: Criar o componente com `Open`, `Close`, `Current`, `Opened`, `Closed` e Esc.
**Where**: `Assets/_Project/Code/Runtime/Hub/HubPanel.cs`
**Depends on**: None
**Reuses**: Input System
**Requirement**: HUB-05, HUB-06, HUB-14

**Done when**:

- [ ] Abrir um painel fecha o anterior
- [ ] Esc fecha o painel aberto
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): adiciona HubPanel`

---

### T2: Criar `ExpeditionPanel` (usuário)

**What**: Preencher mapa e deck, ligar ou desligar "Partir", carregar a run uma vez com `timeScale` 1.
**Where**: `Assets/_Project/Code/Runtime/Hub/ExpeditionPanel.cs`
**Depends on**: T1
**Reuses**: `HubPanel`, `SaveSystem`, `DeckData.MinCards/MaxCards`
**Requirement**: HUB-03, HUB-04, HUB-05, HUB-07, HUB-13, HUB-15

**Done when**:

- [ ] Deck fora de 10 a 15 desliga "Partir" e mostra o aviso
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): adiciona o painel de expedição`

---

### T3: Arte do portão (Claude)

**What**: Montar `Gate.png` com pilares e grade na paleta do hub, com 2 px de margem.
**Where**: `Assets/_Project/Art/Sprites/Hub/Gate.png`
**Depends on**: T2
**Reuses**: tileset do cemitério e Graveyard Set
**Requirement**: HUB-01

**Done when**:

- [ ] Importado com PPU 16, Point e pivô na base
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `art(hub): adiciona o portão do cemitério`

---

### T4: Criar `ResourceCounterView` (Claude)

**What**: Criar a view de ícone e número.
**Where**: `Assets/_Project/Code/Runtime/UI/ResourceCounterView.cs`
**Depends on**: T3
**Reuses**: `StatEntryView`
**Requirement**: HUB-08

**Done when**:

- [ ] `SetValue(int)` muda o número
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona contador de recurso`

---

### T5: Criar `ResourceBar` (Claude)

**What**: Assinar `SaveSystem.Changed` e redesenhar os 5 contadores.
**Where**: `Assets/_Project/Code/Runtime/Hub/ResourceBar.cs`
**Depends on**: T4
**Reuses**: `ResourceCounterView`, ícones de `run-rewards`
**Requirement**: HUB-08, HUB-09, HUB-10

**Done when**:

- [ ] Mudar o save redesenha a barra
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): adiciona a barra de recursos`

---

### T6: Portão, Canvas e painéis no builder (Claude)

**What**: Pôr o portão com `HubInteractable` no fim da trilha, o Canvas overlay, a barra no topo e o painel de expedição com fundo.
**Where**: `Assets/_Project/Code/Editor/HubSceneBuilder.cs`
**Depends on**: T5
**Reuses**: `PlaceInteractable`, arte gótica
**Requirement**: HUB-01, HUB-02, HUB-03, HUB-08, HUB-14

**Done when**:

- [ ] A prévia `Logs/hub_preview.png` mostra o portão e a barra
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta portão, barra de recursos e painel de expedição`

---

### T7: Ordem das cenas no build (Claude)

**What**: Deixar `Hub` e `FirstOne`, nessa ordem, e tirar a `SampleScene`.
**Where**: `ProjectSettings/EditorBuildSettings.asset`
**Depends on**: T6
**Reuses**: lista atual
**Requirement**: HUB-11, HUB-12

**Done when**:

- [ ] Build Settings mostra `Hub` no índice 0 e não mostra `SampleScene`
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(build): abre o jogo no hub`

---

## Checks

**C1** - O hub tem um portão clicável no fim da trilha de baixo (HUB-01, AC 1)
Proof: walk - Entrar no hub e ver o portão no fim da trilha.

**C2** - O portão ganha o contorno dourado com o mouse em cima (HUB-02, AC 2)
Proof: walk - Passar o mouse sobre o portão.

**C3** - O clique abre "Partir para a expedição?" com o mapa e "Deck: 15 cartas" (HUB-03, AC 3)
Proof: walk - Clicar no portão num save novo.

**C4** - "Partir" com deck válido carrega a `FirstOne` (HUB-04, AC 4)
Proof: walk - Clicar em "Partir" e cair na run.

**C5** - "Ficar" fecha o painel (HUB-05, AC 5)
Proof: walk - Clicar em "Ficar" e continuar no hub.

**C6** - Esc fecha o painel (HUB-06, AC 6)
Proof: walk - Abrir o painel e apertar Esc.

**C7** - A run começa com `timeScale` 1 (HUB-07, AC 7)
Proof: walk - Morrer, voltar ao hub, partir de novo e ver os inimigos se mexendo.

**C8** - A barra mostra os 5 recursos com ícone (HUB-08, AC 1)
Proof: walk - Entrar no hub e ver ouro, óleo, osso, ferro e cera.

**C9** - A barra muda sem recarregar a cena (HUB-09, AC 2)
Proof: walk - Comprar um ponto no padre e ver o ouro cair na hora.

**C10** - Voltar da run mostra as recompensas somadas (HUB-10, AC 3)
Proof: walk - Vencer com 70 de ouro e ver a barra subir 70 no hub.

**C11** - O build abre no hub (HUB-11, AC 1)
Proof: walk - Gerar um build e ver o acampamento como primeira tela.

**C12** - A `SampleScene` não está no build (HUB-12, AC 2)
Proof: walk - Abrir Build Settings e não achar a `SampleScene`.

**C13** - Deck inválido desliga "Partir" e mostra o aviso (HUB-13, edge case)
Proof: walk - Deixar 9 cartas no deck e abrir o painel.

**C14** - Com o painel aberto, o mundo não reage (HUB-14, edge case)
Proof: walk - Com o painel aberto, passar o mouse e clicar na igreja: nada acontece.

**C15** - Vários cliques em "Partir" carregam a run uma vez (HUB-15, edge case)
Proof: walk - Clicar 3 vezes rápido em "Partir" e ver uma única troca de cena, sem erro no Console.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** none
**Dispatched:** none
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2

Phase 1:  T1 ------→ T2
Phase 2:  T3 ------→ T4 ------→ T5 ------→ T6 ------→ T7
```

Execution is strictly sequential. A feature tem 7 tasks e cabe em um único lote.
