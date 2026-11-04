# Padre: atributos permanentes Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/priest-stats/design.md`
**Status**: Approved
**Approved**: 2026-11-04 - user: "Aprovo todas"
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

Atributos como dados, a defesa e a base da run.

```
T1 → T2 → T3 → T4 → T5 → T6
```

### Phase 2: Menu

Assets, arte e o menu do padre.

```
T6 → T7 → T8 → T9 → T10 → T11
```

---

## Task Breakdown

### T1: Criar `PlayerStatType` e `AttributeData` (usuário)

**What**: Criar o enum dos status, o efeito (status + ganho por ponto) e o asset de atributo com ID, nome, ícone e lista de efeitos.
**Where**: `Assets/_Project/Code/Runtime/Priest/AttributeData.cs`
**Depends on**: None
**Reuses**: formato de `CardData`
**Requirement**: PRST-20, PRST-22

**Done when**:

- [x] O Inspector de um `AttributeData` mostra ID, nome, ícone e uma lista de efeitos com status e ganho
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(priest): adiciona AttributeData`

---

### T2: Defesa no `PlayerStats` e no `Health` (usuário)

**What**: Adicionar `Defense` ao `PlayerStats` e `DamageTakenMultiplier` ao `Health`, usado no `TakeDamage`.
**Where**: `Assets/_Project/Code/Runtime/Player/PlayerStats.cs`, `Assets/_Project/Code/Runtime/Combat/Health.cs`
**Depends on**: T1
**Reuses**: `Health.TakeDamage`
**Requirement**: PRST-17, PRST-18

**Done when**:

- [x] Com `DamageTakenMultiplier` 0,8, um golpe de 10 tira 8 de vida
- [x] Inimigos continuam recebendo o dano cheio
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(combat): adiciona defesa ao dano recebido`

---

### T3: Criar `StatBlock` e `PermanentStats` (usuário)

**What**: Montar a base da run a partir do `PlayerStats` e dos efeitos dos atributos, ler um status do bloco e somar o nível do personagem.
**Where**: `Assets/_Project/Code/Runtime/Player/PermanentStats.cs`
**Depends on**: T2
**Reuses**: `SaveData.GetLevel`
**Requirement**: PRST-06, PRST-10, PRST-22, PRST-28

**Done when**:

- [ ] Com Vitalidade 3 (+10 por ponto), `Build` dá 130 de vida a partir de 100
- [ ] `CharacterLevel` soma todos os níveis do save, inclusive de IDs sem atributo
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): monta a base da run com os atributos`

---

### T4: `PlayerRunStats` usa o `StatBlock` (usuário)

**What**: Trocar `baseStats.X` por `block.X`, multiplicar a cadência pelo bloco, prender a esquiva em 0,2 s e passar a defesa ao `Health`.
**Where**: `Assets/_Project/Code/Runtime/Player/PlayerRunStats.cs`
**Depends on**: T3
**Reuses**: `PermanentStats.Build`
**Requirement**: PRST-10 a PRST-17, PRST-26, PRST-27

**Done when**:

- [ ] Sem pontos no padre, os status da run são os mesmos de antes
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): aplica os atributos do padre na run`

---

### T5: `GameDatabase` com os atributos e o custo (usuário)

**What**: Adicionar a lista `Attributes`, `FindAttribute`, `PriestBaseCost` e `PriestCostStep`, com aviso para ID vazio.
**Where**: `Assets/_Project/Code/Runtime/Save/GameDatabase.cs`
**Depends on**: T4
**Reuses**: `FindCard`
**Requirement**: PRST-07, PRST-21

**Done when**:

- [ ] O Inspector do `GameDatabase` mostra a lista de atributos e os dois números do custo
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): lista os atributos do padre no GameDatabase`

---

### T6: Criar `PriestShop` (usuário)

**What**: Criar `LevelOf`, `CharacterLevel`, `NextCost`, `CanBuy` e `TryBuy`.
**Where**: `Assets/_Project/Code/Runtime/Priest/PriestShop.cs`
**Depends on**: T5
**Reuses**: `SaveSystem.TrySpend`, `SaveData.SetLevel`
**Requirement**: PRST-04, PRST-05, PRST-06, PRST-07, PRST-25

**Done when**:

- [ ] Com 20 de ouro, `TryBuy` em dois atributos deixa 5 de ouro, nível 2 e `NextCost` 15
- [ ] Sem ouro, `TryBuy` devolve falso e não muda o save
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(priest): adiciona a compra de pontos`

---

### T7: Os 4 assets de atributo (Claude)

**What**: Criar Vitalidade, Força, Agilidade e Resistência com os efeitos e ganhos da spec, pôr na lista do `GameDatabase` e ajustar `Defense` 0 no `PlayerStats.asset`.
**Where**: `Assets/_Project/Data/Priest/`, `Assets/_Project/Resources/GameDatabase.asset`, `Assets/_Project/Data/Progression/PlayerStats.asset`
**Depends on**: T6
**Reuses**: valores da spec
**Requirement**: PRST-11 a PRST-14, PRST-18, PRST-20

**Done when**:

- [ ] Os 4 assets têm IDs próprios e os efeitos da spec, na ordem do menu
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(priest): cria os atributos do padre`

---

### T8: Ícones dos 4 atributos (Claude)

**What**: Desenhar Vitalidade, Força, Agilidade e Resistência em pixel art na paleta da UI.
**Where**: `Assets/_Project/Art/Sprites/UI/Victory/`
**Depends on**: T7
**Reuses**: estilo dos ícones de status da tela de resultado
**Requirement**: PRST-03, PRST-20

**Done when**:

- [ ] 4 ícones importados como os outros ícones da UI e ligados aos assets
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `art(ui): adiciona ícones dos atributos`

---

### T9: Criar `AttributeRowView` (Claude)

**What**: Criar a linha com ícone, nome, nível, linhas de efeito "atual → próximo" e o botão "Abençoar".
**Where**: `Assets/_Project/Code/Runtime/UI/AttributeRowView.cs`
**Depends on**: T8
**Reuses**: `RewardEntryView`, `MenuOption`
**Requirement**: PRST-03, PRST-08, PRST-23

**Done when**:

- [ ] `Show` troca nome, nível, efeitos e liga ou desliga o botão
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona a linha de atributo`

---

### T10: Criar `PriestMenu` (usuário)

**What**: Mostrar nível e custo, criar uma linha por atributo com os efeitos atual → próximo, redesenhar no `Changed` e comprar pelo `PriestShop`.
**Where**: `Assets/_Project/Code/Runtime/Priest/PriestMenu.cs`
**Depends on**: T9
**Reuses**: `HubPanel`, `PermanentStats`, `PriestShop`
**Requirement**: PRST-02, PRST-03, PRST-08, PRST-19, PRST-21, PRST-23, PRST-24

**Done when**:

- [ ] Sem ouro, os botões ficam desligados e o custo fica vermelho
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(priest): adiciona o menu do padre`

---

### T11: Painel do padre no builder (Claude)

**What**: Montar o painel do padre no Canvas do hub e ligar o `onClick` da igreja ao `PriestMenu.Open`.
**Where**: `Assets/_Project/Code/Editor/HubCanvasBuilder.cs`, `Assets/_Project/Code/Editor/HubSceneBuilder.cs`
**Depends on**: T10
**Reuses**: `HubPanel`, arte gótica
**Requirement**: PRST-01, PRST-09

**Done when**:

- [ ] A prévia em `Logs/` mostra o menu do padre aberto
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta o menu do padre`

---

## Checks

Para acelerar os walks, o agente conduz o Play Mode pelo Editor (`unity command`), com backup e restauração do `save.json`; dar ouro é `SaveSystem.Add` em memória.

**C1** - Clicar na igreja abre o menu do padre (PRST-01, AC 1)
Proof: walk - Clicar na igreja no hub.

**C2** - O menu mostra o nível do personagem e o custo do próximo ponto (PRST-02, AC 2)
Proof: walk - Num save novo, ler "Nível 0" e custo 5.

**C3** - Cada atributo mostra nome, nível e cada efeito com atual → próximo (PRST-03, AC 3)
Proof: walk - Ler "Vitalidade Nv 0" e "Vida máxima 100 → 110"; ler as 5 linhas da Agilidade.

**C4** - Comprar tira o custo do ouro (PRST-04, AC 4)
Proof: walk - Com 20 de ouro, comprar Vitalidade e ver 15.

**C5** - Comprar sobe o nível do atributo no save (PRST-05, AC 5)
Proof: walk - Ler `PriestLevels` no `save.json` com o ID da Vitalidade em 1.

**C6** - O nível do personagem é a soma dos atributos (PRST-06, AC 6)
Proof: walk - Comprar Vitalidade e Força e ler "Nível 2".

**C7** - O custo é 5 + 5 × nível e igual em qualquer atributo (PRST-07, AC 7)
Proof: walk - Com 20 de ouro, comprar Vitalidade (5) e Força (10): sobra 5 e o próximo custa 15 em todos.

**C8** - Depois da compra, o menu mostra o novo nível, o novo custo e os novos valores (PRST-08, AC 8)
Proof: walk - Depois de comprar Vitalidade, ler "Vida máxima 110 → 120" e o custo 10.

**C9** - "Fechar" e Esc fecham o menu (PRST-09, AC 9)
Proof: walk - Fechar pelo botão; reabrir e apertar Esc no Game view.

**C10** - A run soma à base o ganho vezes o nível de cada atributo (PRST-10, AC 1)
Proof: walk - Com pontos em cada atributo, começar uma run e ler os status do `PlayerRunStats`.

**C11** - Vitalidade sobe a vida máxima (PRST-11, AC 2)
Proof: walk - Com Vitalidade 3, começar a run com 130 de vida máxima.

**C12** - Força sobe o dano base de qualquer arma (PRST-12, AC 3)
Proof: walk - Com Força 2, ver `CalculateDamage` da arma 2 acima do de antes.

**C13** - Agilidade move os cinco status, e a recarga da esquiva cai (PRST-13, AC 4)
Proof: walk - Com Agilidade 5, ler velocidade +0,25, esquiva +0,25, recarga −0,2 s, cadência ×1,1 e coleta +0,25; a velocidade do projétil não muda.

**C14** - Resistência sobe a defesa (PRST-14, AC 5)
Proof: walk - Com Resistência 10, ler defesa 0,183 (1 − 0,98^10) no `PlayerRunStats`.

**C15** - As cartas se aplicam sobre a base com os pontos (PRST-15, AC 6)
Proof: walk - Com Vitalidade 3 e uma carta de +10% de vida, ler 143.

**C16** - A tela de resultado mostra os status com os pontos (PRST-16, AC 7)
Proof: walk - Com Vitalidade 3, terminar a run e ler 130 em "Vida máxima".

**C17** - O dano recebido cai pela defesa (PRST-17, AC 1)
Proof: walk - Com Resistência 10, um golpe de 10 tira 8,17 de vida.

**C18** - A defesa começa em 0 e combina os pontos multiplicando (PRST-18, AC 2)
Proof: walk - Sem Resistência, um golpe de 10 tira 10; com Resistência 30, a defesa é 0,455 (1 − 0,98^30), e não 0,6.

**C19** - O menu mostra a defesa em porcentagem (PRST-19, AC 3)
Proof: walk - Ler "Defesa 0% → 2%" na Resistência.

**C20** - Cada atributo é um asset com ID, nome, ícone e efeitos (PRST-20, AC 1)
Proof: walk - Abrir os 4 assets no Inspector.

**C21** - Um atributo novo na lista aparece no menu sem código (PRST-21, AC 2)
Proof: walk - Criar um atributo de teste, pôr na lista e vê-lo no menu; depois removê-lo.

**C22** - Um efeito novo com status existente vale sem código (PRST-22, AC 3)
Proof: walk - Pôr um efeito de vida na Força de teste e ver a vida subir na run e no menu; depois desfazer.

**C23** - Sem ouro, os botões de compra ficam desligados (PRST-23, edge case)
Proof: walk - Com 0 de ouro, abrir o menu e ver os 4 botões desligados.

**C24** - Sem ouro, o custo fica vermelho (PRST-24, edge case)
Proof: walk - No mesmo teste, ler a cor do custo.

**C25** - Cliques rápidos compram um ponto por clique, sem ouro negativo (PRST-25, edge case)
Proof: walk - Com 20 de ouro, clicar 5 vezes em Vitalidade: 2 pontos e 5 de ouro.

**C26** - A recarga da esquiva não fica abaixo de 0,2 s (PRST-26, edge case)
Proof: walk - Com Agilidade 50, ler 0,2 s de recarga.

**C27** - Com muitos pontos, a defesa fica abaixo de 100% e cada ponto rende menos (PRST-27, edge case)
Proof: walk - Com Resistência 100, ler defesa 0,867 e um golpe de 10 tirando 1,33; o ganho do ponto 100 é menor que o do ponto 1.

**C28** - Nível de atributo removido fica no save e conta no nível (PRST-28, edge case)
Proof: walk - Pôr no save um nível 3 de um ID inexistente e ler "Nível 3" e custo 20, sem efeito na run.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** T2 - nomes do usuário: `PlayerStats.DamageReduction` (no lugar de `Defense`) e `Health.DamageReductionMultiplier` (no lugar de `DamageTakenMultiplier`); o multiplicador é o quanto do dano passa (1 − redução) | T2 - a defesa multiplica (1 − 0,98^nível), sem teto; PRST-18, PRST-27, C14, C17, C18, C27 e o `DefenseCap` mudaram. Usuário, na T2, depois de comparar com a defesa dos soulslikes: escolheu "Multiplicando (soulslike)". | T1 - `PlayerStatType` e `AttributeEffect` ficam em arquivos próprios em `Runtime/Player/`; sem asset de teste no Inspector (usuário: "Já sabemos que vai funcionar."), conferido na T7 | T1 - a velocidade do projétil sai do padre e fica só nas cartas; PRST-13, o enum e a C13 mudaram. Usuário, na T1: "esse campo nao vai existir. Isso será aumentado através das cartas durante a run."
**Dispatched:** none
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5 ------→ T6
Phase 2:  T7 ------→ T8 ------→ T9 ------→ T10 -----→ T11
```

Execution is strictly sequential. A feature tem 11 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
