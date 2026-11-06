# Mesa de tarot: cartas e deck Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/tarot-table/design.md`
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

### Phase 1: Regras de carta

Custo, desbloqueio, deck e o level up.

```
T1 → T2 → T3 → T4 → T5 → T6
```

### Phase 2: Views

As peças de tela da mesa.

```
T6 → T7 → T8 → T9
```

### Phase 3: Abas e painel

As três abas e o painel no hub.

```
T9 → T10 → T11 → T12 → T13 → T14
```

---

## Task Breakdown

### T1: `UnlockCost` no `CardData` (usuário)

**What**: Adicionar o custo de desbloqueio como `ResourceBag`.
**Where**: `Assets/_Project/Code/Runtime/Cards/CardData.cs`
**Depends on**: None
**Reuses**: `ResourceBag`
**Requirement**: TAROT-02

**Done when**:

- [x] O Inspector de uma carta mostra o custo
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): adiciona custo de desbloqueio`

---

### T2: Custos nas 20 cartas (Claude)

**What**: Dar 15 materiais a cada carta, divididos entre osso, ferro e cera.
**Where**: `Assets/_Project/Data/Cards/*.asset`
**Depends on**: T1
**Reuses**: valor provisório da spec
**Requirement**: TAROT-02

**Done when**:

- [x] Todas as cartas somam 15 materiais
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(cards): preenche os custos das cartas`

---

### T3: Criar `CardLibrary` (usuário)

**What**: Criar as consultas e as mudanças de desbloqueio e de deck do design.
**Where**: `Assets/_Project/Code/Runtime/Cards/CardLibrary.cs`
**Depends on**: T2
**Reuses**: `SaveSystem`, `GameDatabase`, `DeckData.MinCards/MaxCards`
**Requirement**: TAROT-03, TAROT-04, TAROT-10, TAROT-12, TAROT-13, TAROT-15, TAROT-18, TAROT-20

**Done when**:

- [x] `TryAddToDeck` recusa carta repetida, bloqueada ou deck cheio
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): adiciona regras de desbloqueio e deck`

---

### T4: Criar `CardDescriber` (usuário)

**What**: Gerar uma linha por bônus diferente de zero, com redução escrita como "-N%".
**Where**: `Assets/_Project/Code/Runtime/Cards/CardDescriber.cs`
**Depends on**: T3
**Reuses**: nomes dos status da tela de resultado
**Requirement**: TAROT-08, TAROT-22

**Done when**:

- [x] Uma carta de esquiva mostra "-20% recarga da esquiva"
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): descreve os efeitos das cartas`

---

### T5: `LevelUpPanel` sorteia do deck salvo (usuário)

**What**: Trocar `deck.Cards` por `CardLibrary.DeckCards()` e completar com o `Deck.asset` se faltar carta.
**Where**: `Assets/_Project/Code/Runtime/UI/LevelUpPanel.cs`
**Depends on**: T4
**Reuses**: `ShowOptions`
**Requirement**: TAROT-16

**Done when**:

- [x] Só cartas do deck salvo aparecem
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): o level up sorteia do deck do jogador`

---

### T6: Portão usa `CardLibrary.IsDeckValid` (usuário)

**What**: Trocar a conta de validade do `ExpeditionPanel` pela da `CardLibrary`.
**Where**: `Assets/_Project/Code/Runtime/Hub/ExpeditionPanel.cs`
**Depends on**: T5
**Reuses**: `CardLibrary`
**Requirement**: TAROT-19

**Done when**:

- [x] Portão e mesa concordam sobre o deck válido
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `refactor(hub): portão usa a regra de deck da mesa`

---

### T7: `CardSlotView` com bloqueio e clique (Claude)

**What**: Adicionar `SetLocked(bool)` e o clique opcional, sem mudar a tela de resultado.
**Where**: `Assets/_Project/Code/Runtime/UI/CardSlotView.cs`
**Depends on**: T6
**Reuses**: `CardSlotView` atual
**Requirement**: TAROT-09, TAROT-11

**Done when**:

- [x] A tela de resultado continua igual
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): célula de carta com estado bloqueado`

---

### T8: Criar `UnlockRowView` (Claude)

**What**: Criar a linha com ícone, nome, custo e botão "Desbloquear".
**Where**: `Assets/_Project/Code/Runtime/UI/UnlockRowView.cs`
**Depends on**: T7
**Reuses**: `PriestStatRowView`
**Requirement**: TAROT-02, TAROT-17

**Done when**:

- [x] Material que falta fica vermelho
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona a linha de desbloqueio`

---

### T9: Criar `CardDetailView` (Claude)

**What**: Criar o detalhe com nome, ícone, descrição, efeitos e custo.
**Where**: `Assets/_Project/Code/Runtime/UI/CardDetailView.cs`
**Depends on**: T8
**Reuses**: `CardDescriber`
**Requirement**: TAROT-08, TAROT-09

**Done when**:

- [ ] Carta bloqueada mostra "Bloqueada" e o custo
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona o detalhe da carta`

---

### T10: Criar `TarotUnlockTab` (usuário)

**What**: Listar as bloqueadas e desbloquear pela `CardLibrary`.
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotUnlockTab.cs`
**Depends on**: T9
**Reuses**: `UnlockRowView`
**Requirement**: TAROT-02, TAROT-05, TAROT-17, TAROT-21

**Done when**:

- [ ] Desbloquear tira a carta da lista
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(tarot): adiciona a aba Desbloquear`

---

### T11: Criar `TarotCollectionTab` (usuário)

**What**: Mostrar as 20 cartas, o detalhe da selecionada e "Liberadas: N/M".
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotCollectionTab.cs`
**Depends on**: T10
**Reuses**: `CardSlotView`, `CardDetailView`
**Requirement**: TAROT-07, TAROT-08, TAROT-09, TAROT-10

**Done when**:

- [ ] Selecionar uma carta muda o detalhe
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(tarot): adiciona a aba Coleção`

---

### T12: Criar `TarotDeckTab` (usuário)

**What**: Separar "No deck" e "Disponíveis", mover por clique e mostrar "Deck: N/15" e o aviso.
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotDeckTab.cs`
**Depends on**: T11
**Reuses**: `CardSlotView`, `CardLibrary`
**Requirement**: TAROT-11, TAROT-12, TAROT-13, TAROT-14, TAROT-18, TAROT-19

**Done when**:

- [ ] Clicar move a carta e grava o save
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(tarot): adiciona a aba Deck`

---

### T13: Criar `TarotMenu` (usuário)

**What**: Abrir na aba Desbloquear, trocar de aba e redesenhar no `Changed`.
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotMenu.cs`
**Depends on**: T12
**Reuses**: `HubPanel`
**Requirement**: TAROT-01, TAROT-06

**Done when**:

- [ ] Abrir sempre começa em Desbloquear
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(tarot): adiciona o menu da mesa`

---

### T14: Painel da mesa no builder (Claude)

**What**: Montar o painel com as três abas e ligar o `onClick` da mesa ao `TarotMenu.Open`.
**Where**: `Assets/_Project/Code/Editor/HubSceneBuilder.cs`
**Depends on**: T13
**Reuses**: painel do padre
**Requirement**: TAROT-01, TAROT-06

**Done when**:

- [ ] Clicar na mesa abre o painel
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta o painel da mesa de tarot`

---

## Checks

**C1** - Clicar na mesa abre o menu na aba Desbloquear (TAROT-01, AC 1)
Proof: walk - Clicar na mesa de tarot no hub.

**C2** - A aba lista as bloqueadas com nome, ícone e custo (TAROT-02, AC 2)
Proof: walk - Num save novo, ver as 5 cartas bloqueadas com custo.

**C3** - Desbloquear tira o custo dos materiais (TAROT-03, AC 3)
Proof: walk - Com os materiais exatos, desbloquear e ver os materiais caírem pelo custo.

**C4** - Desbloquear grava o ID em `UnlockedCards` (TAROT-04, AC 4)
Proof: walk - Ler o ID novo no `save.json`.

**C5** - A carta desbloqueada sai da aba (TAROT-05, AC 5)
Proof: walk - Ver 4 cartas na aba depois do desbloqueio.

**C6** - "Fechar" e Esc fecham o menu (TAROT-06, AC 6)
Proof: walk - Fechar uma vez por botão e outra por Esc.

**C7** - A Coleção lista as 20 cartas (TAROT-07, AC 1)
Proof: walk - Contar 20 cartas na aba Coleção.

**C8** - Selecionar mostra nome, ícone, descrição e os efeitos diferentes de zero (TAROT-08, AC 2)
Proof: walk - Selecionar uma carta de dano e ver só as linhas de dano.

**C9** - As bloqueadas mostram "Bloqueada" e o custo (TAROT-09, AC 3)
Proof: walk - Selecionar uma carta bloqueada.

**C10** - A Coleção mostra "Liberadas: N/M" (TAROT-10, AC 4)
Proof: walk - Num save novo, ler "Liberadas: 15/20".

**C11** - A aba Deck separa "No deck" e "Disponíveis" (TAROT-11, AC 1)
Proof: walk - Abrir a aba Deck depois de um desbloqueio: 15 no deck e 1 disponível.

**C12** - Clicar numa disponível com menos de 15 a põe no deck (TAROT-12, AC 2)
Proof: walk - Tirar uma carta e clicar numa disponível.

**C13** - Clicar numa carta do deck a tira (TAROT-13, AC 3)
Proof: walk - Clicar numa carta do deck.

**C14** - O contador mostra "Deck: N/15" (TAROT-14, AC 4)
Proof: walk - Ler "Deck: 14/15" depois de tirar uma.

**C15** - Mudar o deck grava a lista no save (TAROT-15, AC 5)
Proof: walk - Ler a lista `Deck` do `save.json` depois de cada clique.

**C16** - O level up só oferece cartas do deck salvo (TAROT-16, AC 6)
Proof: walk - Tirar 3 cartas, jogar até 5 level ups e nunca ver as 3.

**C17** - Sem materiais, o botão fica desligado e o custo vermelho (TAROT-17, edge case)
Proof: walk - Com 0 materiais, abrir a aba Desbloquear.

**C18** - Com 15 no deck, as disponíveis não entram (TAROT-18, edge case)
Proof: walk - Com o deck cheio, clicar numa disponível e ver o deck igual.

**C19** - Abaixo de 10, aparece o aviso (TAROT-19, edge case)
Proof: walk - Deixar 9 cartas e ler "O deck precisa de pelo menos 10 cartas."

**C20** - O deck nunca tem a mesma carta duas vezes (TAROT-20, edge case)
Proof: walk - Clicar duas vezes rápido numa disponível e ver o ID uma vez no `save.json`.

**C21** - Sem bloqueadas, a aba avisa (TAROT-21, edge case)
Proof: walk - Desbloquear as 5 e ler "Todas as cartas foram liberadas."

**C22** - Redução de recarga aparece como "-N%" (TAROT-22, edge case)
Proof: walk - Selecionar uma carta de esquiva e ler "-20% recarga da esquiva".

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** 2026-11-06 - o Claude faz as tasks de código a partir da T4, e corrige a `CardLibrary` da T3 (usuário: "Faça a T4 e T5 para mim. Pode fazer o resto também." e "Claude corrige"). Execução em sub-agentes por lote (usuário: "Em sub-agentes por lote").
**Dispatched:** lote 1 (Fases 1 e 2: T2 a T9, com a correção da T3) a partir de `425fb3e`, em 2026-11-06
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2 → Phase 3

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5 ------→ T6
Phase 2:  T7 ------→ T8 ------→ T9
Phase 3:  T10 ------→ T11 ------→ T12 ------→ T13 ------→ T14
```

Execution is strictly sequential. A feature tem 14 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
