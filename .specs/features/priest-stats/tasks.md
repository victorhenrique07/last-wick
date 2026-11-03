# Padre: status permanentes Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/priest-stats/design.md`
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

### Phase 1: Regras

Status como dados e a base da run.

```
T1 → T2 → T3 → T4 → T5
```

### Phase 2: Menu

Assets, arte e o menu do padre.

```
T5 → T6 → T7 → T8 → T9 → T10
```

---

## Task Breakdown

### T1: Criar `PlayerStatType` e `PriestStatData` (usuário)

**What**: Criar o enum dos status e o asset de status com ID, nome, ícone, ganho e custo.
**Where**: `Assets/_Project/Code/Runtime/Priest/PriestStatData.cs`
**Depends on**: None
**Reuses**: formato do `CardData`
**Requirement**: PRST-06, PRST-11

**Done when**:

- [ ] `CostAt(level)` devolve 5 + 5 × nível
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(priest): adiciona status do padre como dados`

---

### T2: Criar `StatBlock` e `PermanentStats` (usuário)

**What**: Montar a base da run a partir do `PlayerStats` e dos níveis do padre, com piso de 0,2 s na esquiva.
**Where**: `Assets/_Project/Code/Runtime/Player/PermanentStats.cs`
**Depends on**: T1
**Reuses**: `PlayerStats`
**Requirement**: PRST-08, PRST-16

**Done when**:

- [ ] 3 níveis de Vida dão 130 de vida base
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): monta a base permanente da run`

---

### T3: `PlayerRunStats` usa o `StatBlock` (usuário)

**What**: Trocar `baseStats.X` por `block.X` no `Refresh` e manter o piso depois das cartas.
**Where**: `Assets/_Project/Code/Runtime/Player/PlayerRunStats.cs`
**Depends on**: T2
**Reuses**: `Refresh` atual
**Requirement**: PRST-08, PRST-09, PRST-10

**Done when**:

- [ ] Cartas multiplicam o valor que já inclui o padre
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): aplica o padre antes das cartas`

---

### T4: `GameDatabase` com `PriestStats` (usuário)

**What**: Adicionar a lista e `FindPriestStat`.
**Where**: `Assets/_Project/Code/Runtime/Save/GameDatabase.cs`
**Depends on**: T3
**Reuses**: `FindCard`
**Requirement**: PRST-12

**Done when**:

- [ ] A lista aparece no Inspector do `GameDatabase`
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): lista os status do padre no GameDatabase`

---

### T5: Criar `PriestShop` (usuário)

**What**: Criar `LevelOf`, `NextCost`, `CanBuy` e `TryBuy`.
**Where**: `Assets/_Project/Code/Runtime/Priest/PriestShop.cs`
**Depends on**: T4
**Reuses**: `SaveSystem.TrySpend`
**Requirement**: PRST-03, PRST-04, PRST-06, PRST-15

**Done when**:

- [ ] `TryBuy` sem ouro devolve falso e não muda nada
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(priest): adiciona a compra de pontos`

---

### T6: Os 5 assets de status (Claude)

**What**: Criar Vida, Dano, Velocidade, Recarga da esquiva e Coleta de XP com os valores da spec e pôr na lista.
**Where**: `Assets/_Project/Data/Priest/`
**Depends on**: T5
**Reuses**: valores da spec
**Requirement**: PRST-11

**Done when**:

- [ ] Os 5 assets estão no `GameDatabase`, sem aviso de ID
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(priest): cria os status do padre`

---

### T7: Ícones dos 5 status (Claude)

**What**: Desenhar os ícones em pixel art na paleta do jogo.
**Where**: `Assets/_Project/Art/Sprites/UI/Priest/`
**Depends on**: T6
**Reuses**: paleta do tileset
**Requirement**: PRST-02

**Done when**:

- [ ] 5 ícones ligados aos assets
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `art(priest): adiciona ícones dos status`

---

### T8: Criar `PriestStatRowView` (Claude)

**What**: Criar a linha com ícone, nome, nível, valores, custo e botão.
**Where**: `Assets/_Project/Code/Runtime/UI/PriestStatRowView.cs`
**Depends on**: T7
**Reuses**: `StatEntryView`
**Requirement**: PRST-02, PRST-13, PRST-14

**Done when**:

- [ ] Custo em vermelho e botão desligado quando `canBuy` é falso
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona a linha de status do padre`

---

### T9: Criar `PriestMenu` (usuário)

**What**: Criar uma linha por status, redesenhar no `Changed` e comprar pelo `PriestShop`.
**Where**: `Assets/_Project/Code/Runtime/Priest/PriestMenu.cs`
**Depends on**: T8
**Reuses**: `HubPanel`, `PriestStatRowView`
**Requirement**: PRST-01, PRST-02, PRST-05, PRST-07, PRST-12

**Done when**:

- [ ] Comprar atualiza nível, valor e custo da linha
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(priest): adiciona o menu do padre`

---

### T10: Painel do padre no builder (Claude)

**What**: Montar o painel com fundo e ligar o `onClick` da igreja ao `PriestMenu.Open`.
**Where**: `Assets/_Project/Code/Editor/HubSceneBuilder.cs`
**Depends on**: T9
**Reuses**: painel de expedição
**Requirement**: PRST-01, PRST-07

**Done when**:

- [ ] Clicar na igreja abre o painel
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta o painel do padre`

---

## Checks

**C1** - Clicar na igreja abre o menu do padre (PRST-01, AC 1)
Proof: walk - Clicar na igreja no hub.

**C2** - Cada linha mostra nome, nível, valor atual, próximo valor e custo (PRST-02, AC 2)
Proof: walk - Abrir o menu e ler "Vida Nv 0 100 → 110 5".

**C3** - Comprar tira o custo do ouro (PRST-03, AC 3)
Proof: walk - Com 20 de ouro, comprar Vida e ver 15.

**C4** - Comprar sobe o nível em 1 no save (PRST-04, AC 4)
Proof: walk - Ler `PriestLevels` no `save.json` com Vida 1.

**C5** - A linha mostra o novo nível, valor e custo (PRST-05, AC 5)
Proof: walk - Depois da compra, ler "Nv 1 110 → 120 10".

**C6** - O custo é 5 + 5 × nível (PRST-06, AC 6)
Proof: walk - Comprar Vida duas vezes com 20 de ouro: sobra 5 e o próximo custa 15.

**C7** - "Fechar" e Esc fecham o menu (PRST-07, AC 7)
Proof: walk - Fechar uma vez por botão e outra por Esc.

**C8** - Os pontos somam ao `PlayerStats` na run (PRST-08, AC 1)
Proof: walk - Com 3 pontos de Vida, a run começa com 130 de vida máxima.

**C9** - As cartas multiplicam o valor com o padre (PRST-09, AC 2)
Proof: walk - Com 3 pontos de Vida, escolher "+20% vida" e ver 156.

**C10** - A tela de resultado mostra os status com o padre (PRST-10, AC 3)
Proof: walk - Terminar a run do teste anterior e ver a vida final com os pontos.

**C11** - Cada status é um asset com ID, nome, ícone, ganho e custo (PRST-11, AC 1)
Proof: walk - Abrir os 5 assets no Inspector.

**C12** - Um asset novo na lista aparece no menu (PRST-12, AC 2)
Proof: walk - Duplicar o asset de Vida, mudar o ID, pôr na lista e ver 6 linhas.

**C13** - Sem ouro, o botão fica desligado (PRST-13, edge case)
Proof: walk - Com 0 de ouro, abrir o menu.

**C14** - Sem ouro, o custo fica vermelho (PRST-14, edge case)
Proof: walk - No mesmo teste, ver os custos em vermelho.

**C15** - Cliques rápidos compram um ponto por clique, sem ouro negativo (PRST-15, edge case)
Proof: walk - Com 15 de ouro, clicar 5 vezes rápido em Vida: compra 2 pontos e sobra 0.

**C16** - A esquiva nunca fica abaixo de 0,2 s (PRST-16, edge case)
Proof: walk - Pôr 100 no nível da esquiva no `save.json` e ler 0,2 na tela de resultado.

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

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5
Phase 2:  T6 ------→ T7 ------→ T8 ------→ T9 ------→ T10
```

Execution is strictly sequential. A feature tem 10 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
