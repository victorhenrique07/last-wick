# Mesa de tarot: cartas e deck Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/tarot-table/design.md`
**Status**: Approved
**Approved**: 2026-11-06 - user: "Aprovo as tasks"
**Revisão**: 2026-11-06 - Phase 4 (T15 a T19): desbloqueio na Coleção. Primeira aprovação: 2026-11-03 - user: "Aprovo todas".
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

### Phase 4: Revisão de 2026-11-06 (desbloqueio na Coleção)

A aba Desbloquear sai e cada carta da Coleção mostra nome, descrição, "Desbloquear" e o custo.

```
T14 → T15 → T16 → T17 → T18 → T19
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

- [x] Carta bloqueada mostra "Bloqueada" e o custo
- [x] Console sem erros de compilação

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

- [x] Desbloquear tira a carta da lista
- [x] Console sem erros de compilação

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

- [x] Selecionar uma carta muda o detalhe
- [x] Console sem erros de compilação

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

- [x] Clicar move a carta e grava o save
- [x] Console sem erros de compilação

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

- [x] Abrir sempre começa em Desbloquear
- [x] Console sem erros de compilação

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

- [x] Clicar na mesa abre o painel
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta o painel da mesa de tarot`

---

### T15: Criar `CollectionCardView` (Claude)

**What**: Criar a carta da Coleção com ícone, nome, descrição, botão "Desbloquear" e custo.
**Where**: `Assets/_Project/Code/Runtime/UI/CollectionCardView.cs`
**Depends on**: T14
**Reuses**: estilo de `UnlockRowView` (custo e cor de aviso) e de `CardSlotView` (ícone padrão)
**Requirement**: TAROT-02, TAROT-05, TAROT-08, TAROT-17

**Done when**:

- [x] Uma carta bloqueada sem materiais mostra o botão desligado e o custo na cor de aviso
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona a carta da Coleção com desbloqueio`

---

### T16: `TarotCollectionTab` com as cartas da Coleção (Claude)

**What**: Desenhar uma `CollectionCardView` por carta e desbloquear pelo botão de cada uma, sem o painel de detalhe.
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotCollectionTab.cs`
**Depends on**: T15
**Reuses**: `CardLibrary`
**Requirement**: TAROT-02, TAROT-03, TAROT-04, TAROT-05, TAROT-07, TAROT-08, TAROT-10

**Done when**:

- [x] Desbloquear uma carta pelo botão dela tira o botão e o custo
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(tarot): desbloqueio direto na Coleção`

---

### T17: `TarotMenu` com Coleção e Deck (Claude)

**What**: Deixar só as abas Coleção e Deck e abrir na Coleção.
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotMenu.cs`
**Depends on**: T16
**Reuses**: `HubPanel`
**Requirement**: TAROT-01, TAROT-09

**Done when**:

- [x] Abrir sempre começa na Coleção
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(tarot): menu com Coleção e Deck`

---

### T18: Painel da mesa no builder com a nova Coleção (Claude)

**What**: Montar a grade de cartas da Coleção com rolagem, tirar a aba Desbloquear e o painel de detalhe, regerar a cena `Hub` e conferir o layout por capturas.
**Where**: `Assets/_Project/Code/Editor/HubCanvasBuilder.cs`
**Depends on**: T17
**Reuses**: `BuildTarotMenu`
**Requirement**: TAROT-01, TAROT-02, TAROT-08, TAROT-09

**Done when**:

- [x] Clicar na mesa abre a Coleção com nome e descrição em cada carta
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta a Coleção com desbloqueio`

---

### T19: Remover o que ficou sem uso (Claude)

**What**: Apagar `TarotUnlockTab`, `UnlockRowView`, `CardDetailView`, `CardDescriber` e o `UnlockRow.prefab`.
**Where**: `Assets/_Project/Code/Runtime/Hub/TarotUnlockTab.cs`
**Depends on**: T18
**Reuses**: nada
**Requirement**: TAROT-09

**Done when**:

- [x] Nenhum arquivo do projeto cita os quatro tipos removidos
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `refactor(tarot): remove a aba Desbloquear e o detalhe`

---

## Checks

**C1** - Clicar na mesa abre o menu na aba Coleção (TAROT-01, AC 1)
Proof: walk - Clicar na mesa de tarot no hub e ver a Coleção; fechar no Deck e reabrir na Coleção.

**C2** - Cada carta bloqueada mostra, embaixo da descrição, "Desbloquear" e o custo (TAROT-02, AC 2)
Proof: walk - Num save novo, ver as 5 cartas bloqueadas com o botão e o custo em osso, ferro e cera, e as 15 liberadas sem eles.

**C3** - Desbloquear tira o custo dos materiais (TAROT-03, AC 3)
Proof: walk - Com os materiais exatos, clicar em "Desbloquear" embaixo da carta na Coleção e ver os materiais caírem pelo custo.

**C4** - Desbloquear grava o ID em `UnlockedCards` (TAROT-04, AC 4)
Proof: walk - Ler o ID novo no `save.json`.

**C5** - A carta desbloqueada deixa de mostrar o botão e o custo (TAROT-05, AC 5)
Proof: walk - Depois do desbloqueio, ver a carta sem o botão e sem o custo, e 4 cartas ainda com eles.

**C6** - "Fechar" e Esc fecham o menu (TAROT-06, AC 6)
Proof: walk - Fechar uma vez por botão e outra por Esc.

**C7** - A Coleção lista as 20 cartas (TAROT-07, AC 1)
Proof: walk - Contar 20 cartas na aba Coleção.

**C8** - Cada carta mostra nome, ícone e a descrição do asset (TAROT-08, AC 2)
Proof: walk - Comparar o nome e a descrição de cada uma das 20 cartas da Coleção com o asset dela, e ver o ícone em todas.

**C9** - O menu tem só as abas Coleção e Deck (TAROT-09, AC 3)
Proof: walk - Ler os botões de aba do menu e ver só "Coleção" e "Deck".

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

**C17** - Sem materiais, o botão fica desligado e o que falta fica na cor de aviso (TAROT-17, edge case)
Proof: walk - Com 0 materiais, ver os botões desligados e os custos em #C96D45; com só um material faltando, ver só ele em #C96D45.

**C18** - Com 15 no deck, as disponíveis não entram (TAROT-18, edge case)
Proof: walk - Com o deck cheio, clicar numa disponível e ver o deck igual.

**C19** - Abaixo de 10, aparece o aviso (TAROT-19, edge case)
Proof: walk - Deixar 9 cartas e ler "O deck precisa de pelo menos 10 cartas."

**C20** - O deck nunca tem a mesma carta duas vezes (TAROT-20, edge case)
Proof: walk - Clicar duas vezes rápido numa disponível e ver o ID uma vez no `save.json`.

**C23** - Deck salvo com menos de 3 cartas na run: o level up completa com o deck inicial e registra um aviso (design, Error Handling linha 4)
Proof: walk - Play no `FirstOne` com o deck salvo de 1 e de 0 cartas, subir de nível e ver 3 opções e o aviso no Console.

**C24** - Carta com `UnlockCost` zerado: desbloqueio grátis (design, Error Handling linha 5)
Proof: walk - Desbloquear uma carta de custo zero e ver os materiais iguais e a carta liberada.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** 2026-11-06 - o Claude faz as tasks de código a partir da T4, e corrige a `CardLibrary` da T3 (usuário: "Faça a T4 e T5 para mim. Pode fazer o resto também." e "Claude corrige"). Execução em sub-agentes por lote (usuário: "Em sub-agentes por lote").
**Dispatched:** lote 1 (Fases 1 e 2: T2 a T9, com a correção da T3) a partir de `425fb3e`, em 2026-11-06 - returned (`4d9d709`..`e8ed85a`); lote 2 (Fase 3: T10 a T14) a partir de `e8ed85a`, em 2026-11-06 - returned (`734982d`..`f6b04c9`); Verifier em 2026-11-06 sobre `9c2139f` (diff `8e73d73..9c2139f`) - returned (FAIL, 5 findings; 19 de 22 checks yes), todos fechados em `6846585`, `d24f688`, `a87e788` e na rodada 2 do walk
**Walked:** 2026-11-06, walk conduzido pelo agente no Editor em Play Mode via `unity command` (Run In Background ligado só durante o walk), com backup e restauração do `save.json` (md5 igual ao original no fim); save novo por `SaveSystem.StartNew()` e materiais postos por `SaveSystem.Add`. Cliques pelo raycast do `EventSystem` no centro de cada controle (`RaycastAll` + `pointerClickHandler`). Hub, save novo e 0 materiais: clicar na mesa acertou "Mesa de Tarot (Mago)" e abriu o menu na aba Desbloquear (C1), com 5 linhas, cada uma com ícone, nome e custo em osso, ferro e cera (C2), todos os custos na cor de aviso #C96D45 e os 5 botões desligados (C17). Com 10/3/1: Lança de Osso mostrou 10 e 3 em creme, 2 em aviso e o botão desligado; com 10/3/2 o botão ligou, o clique deixou 0/0/0 (C3), gravou "lanca-de-osso" em `UnlockedCards` no `save.json` (C4) e a aba ficou com 4 linhas (C5). Coleção: 20 células (C7), 4 com "Bloqueada" e "Liberadas: 16/20"; Chama Certeira (dano 0,1 e velocidade do projétil 0,15) mostrou só "+10% dano" e "+15% velocidade do projétil" (C8); Passo de Sombra mostrou "Bloqueada" e "Custo: 7 osso  5 ferro  3 cera" (C9); Fuga pela Névoa mostrou "-10% recarga da esquiva" (C22). Deck: "Deck: 15/15", 15 em "No deck" e Lança de Osso em "Disponíveis" (C11), sem clique e o deck igual depois de clicar (C18); tirar Pavio Longo deu "Deck: 14/15" (C13, C14) e pôr Lança deu 15/15 (C12), com a lista `Deck` do `save.json` conferida depois de cada clique (C15). Dois cliques no mesmo frame numa disponível nunca repetiram o Id no deck (C20). Tirando 6 cartas, "Deck: 9/15" e "O deck precisa de pelo menos 10 cartas." (C19). Com materiais de sobra, desbloquear as 4 restantes deixou "Todas as cartas foram liberadas." (C21). "Fechar" fechou o menu (C6, botão). Run pelo portão (`ExpeditionPanel.Open` + `Depart`) com deck de 11 cartas: 5 level ups (`GameEvents.RaiseLevelUp`), 15 opções, todas do deck salvo (C16). Save novo de volta no hub: "Liberadas: 15/20" (C10); fechar na aba Coleção e reabrir pela mesa voltou na aba Desbloquear (C1). Console sem erros. Capturas do layout: `Logs/tarot_unlock.png`, `Logs/tarot_collection.png`, `Logs/tarot_deck.png`, `Logs/tarot_deck_full.png`, `Logs/tarot_deck_min.png`. C6 com Esc: tecla simulada não chega ao Input System; o usuário abriu o menu da mesa no Play Mode, apertou Esc no Game view e relatou: "Esc fechou". Para reproduzir: abrir `Hub`, dar Play e clicar na mesa de tarot à direita da fogueira. Rodada 2 (2026-11-06, depois dos findings do Verifier, com o mesmo backup e restauração do `save.json`): um `save.json` editado com `Deck` = [pavio-longo, pavio-longo, + 9] e `UnlockedCards` com "" e pavio-longo repetido carregou com o deck de 10 cartas sem repetição e sem o Id vazio; a aba Deck mostrou uma célula de Pavio Longo e "Deck: 10/15" sem o aviso (C19 no limite, C20 pelo save); tirar Pavio deu "Deck: 9/15" com o aviso e o `save.json` sem pavio-longo, e pôr de volta deu "Deck: 10/15" sem o aviso (C19). Coleção: as 20 cartas clicadas uma a uma, cada uma com tantas linhas quanto bônus diferentes de zero e no formato de cada campo, cobrindo 12 dos 13 campos; o 13º (dano base, que nenhuma carta usa) numa carta de teste em memória mostrou "+2 dano base" (C8). Carta em memória sem Id: `IsUnlocked`, `TryUnlock` e `TryAddToDeck` falsos e nada gravado. Carta em memória de custo zero: `TryUnlock` verdadeiro, materiais iguais (22/31/24) e a carta liberada (C24). `FirstOne` com deck salvo de 1 e de 0 cartas: 3 opções em cada level up e o aviso "LevelUpPanel: o deck salvo tem N cartas; completando com o deck inicial." no Console (C23).

---

## Phase Execution Map

```
Phase 1 → Phase 2 → Phase 3 → Phase 4

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5 ------→ T6
Phase 2:  T7 ------→ T8 ------→ T9
Phase 3:  T10 ------→ T11 ------→ T12 ------→ T13 ------→ T14
Phase 4:  T15 ------→ T16 ------→ T17 ------→ T18 ------→ T19
```

Execution is strictly sequential. A feature tem 14 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
