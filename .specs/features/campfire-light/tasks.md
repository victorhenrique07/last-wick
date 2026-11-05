# Fogueira e óleo Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/campfire-light/design.md`
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

### Phase 1: Luz da run

A luz da run passa a vir da fogueira.

```
T1 → T2 → T3 → T4 → T5 → T6
```

### Phase 2: Fogueira no hub

A compra de luz no hub.

```
T6 → T7 → T8 → T9
```

---

## Task Breakdown

### T1: Criar `LightConfig` (usuário)

**What**: Criar o ScriptableObject com os números de luz e custo.
**Where**: `Assets/_Project/Code/Runtime/Light/LightConfig.cs`
**Depends on**: None
**Reuses**: formato do `RunConfig`
**Requirement**: LIGHT-04, LIGHT-08

**Done when**:

- [x] O Inspector mostra os 5 campos
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(light): adiciona LightConfig`

---

### T2: Criar `LightRules` (usuário)

**What**: Criar `Radius` e `CostOf`.
**Where**: `Assets/_Project/Code/Runtime/Light/LightRules.cs`
**Depends on**: T1
**Reuses**: nada
**Requirement**: LIGHT-04, LIGHT-08

**Done when**:

- [x] `Radius(4)` é 5,0 e `CostOf(2)` é 60
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(light): adiciona regras de luz`

---

### T3: `GameDatabase` com `Light` (usuário)

**What**: Adicionar o campo `Light` (`LightConfig`).
**Where**: `Assets/_Project/Code/Runtime/Save/GameDatabase.cs`
**Depends on**: T2
**Reuses**: campos atuais
**Requirement**: LIGHT-08

**Done when**:

- [x] O campo aparece no Inspector
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): liga a configuração de luz no GameDatabase`

---

### T4: `PermanentStats` com a luz da fogueira (usuário)

**What**: Na versão de `Build` que lê o save, preencher `LightRadius` com `LightRules.Radius(db.Light, save.LightLevel)`; na genérica, deixar `LightRadius` em 0 em vez de copiar o `PlayerStats`.
**Where**: `Assets/_Project/Code/Runtime/Player/PermanentStats.cs`
**Depends on**: T3
**Reuses**: `Build` do padre
**Requirement**: LIGHT-08, LIGHT-09

**Done when**:

- [x] Nível 0 dá raio 3,0 sem cartas
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): a luz base vem da fogueira`

---

### T5: Tirar `LightRadius` do `PlayerStats` (usuário)

**What**: Remover o campo que deixou de ser usado.
**Where**: `Assets/_Project/Code/Runtime/Player/PlayerStats.cs`
**Depends on**: T4
**Reuses**: nada
**Requirement**: LIGHT-08

**Done when**:

- [x] Nenhum código lê `PlayerStats.LightRadius`
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `refactor(player): remove o raio de luz do PlayerStats`

---

### T6: Criar `LightConfig.asset` (Claude)

**What**: Preencher os valores da spec e ligar no `GameDatabase`.
**Where**: `Assets/_Project/Data/Progression/LightConfig.asset`
**Depends on**: T5
**Reuses**: valores da spec
**Requirement**: LIGHT-04, LIGHT-08

**Done when**:

- [x] O asset tem raio 3 + 0,5 por nível até 10 e custo 40 + 20
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(light): cria a configuração de luz`

---

### T7: Criar `CampfireShop` (usuário)

**What**: Criar `IsMaxed`, `NextCost`, `CanBuy` e `TryBuy`.
**Where**: `Assets/_Project/Code/Runtime/Light/CampfireShop.cs`
**Depends on**: T6
**Reuses**: `SaveSystem.TrySpend`
**Requirement**: LIGHT-04, LIGHT-05, LIGHT-06, LIGHT-16

**Done when**:

- [ ] `TryBuy` no nível 10 devolve falso
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(light): adiciona a compra de luz`

---

### T8: Criar `CampfireMenu` (usuário)

**What**: Mostrar nível, raios, custo e o botão, e comprar pelo `CampfireShop`.
**Where**: `Assets/_Project/Code/Runtime/Hub/CampfireMenu.cs`
**Depends on**: T7
**Reuses**: `HubPanel`
**Requirement**: LIGHT-02, LIGHT-03, LIGHT-07, LIGHT-16, LIGHT-17

**Done when**:

- [ ] Sem óleo, o custo fica na cor de aviso e o botão desligado
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): adiciona o menu da fogueira`

---

### T9: Velas clicáveis e painel no builder (Claude)

**What**: Importar o `HubFlame` com a margem de 2 px do contorno, pôr `HubInteractable` nas velas e montar o painel da fogueira.
**Where**: `Assets/_Project/Code/Editor/HubSceneBuilder.cs`
**Depends on**: T8
**Reuses**: `PlaceInteractable`, painel de expedição
**Requirement**: LIGHT-01, LIGHT-02

**Done when**:

- [ ] Passar o mouse nas velas mostra o contorno
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta a fogueira clicável e seu painel`

---

## Checks

**C1** - As velas ganham o contorno dourado com o mouse em cima (LIGHT-01, AC 1)
Proof: walk - Passar o mouse sobre as velas no hub.

**C2** - Clicar nas velas abre o menu da fogueira (LIGHT-02, AC 2)
Proof: walk - Clicar nas velas.

**C3** - O menu mostra nível, raio atual, próximo raio e custo (LIGHT-03, AC 3)
Proof: walk - Num save novo, ler "Nível 0 3,0 → 3,5 40".

**C4** - O nível n custa 40 + 20 × (n − 1) (LIGHT-04, AC 4)
Proof: walk - Comprar dois níveis e ver os custos 40 e 60, e o próximo 80.

**C5** - Comprar tira o custo do óleo (LIGHT-05, AC 5)
Proof: walk - Com 100 de óleo, comprar os níveis 1 e 2 e ver 0.

**C6** - Comprar sobe `LightLevel` em 1 no save (LIGHT-06, AC 6)
Proof: walk - Ler `LightLevel` 2 no `save.json`.

**C7** - "Fechar" e Esc fecham o menu (LIGHT-07, AC 7)
Proof: walk - Fechar uma vez por botão e outra por Esc.

**C8** - A run usa 3,0 + 0,5 × nível como raio base (LIGHT-08, AC 1)
Proof: walk - Com nível 4, ler raio 5,0 no Inspector do `PlayerLight`, sem cartas.

**C9** - As cartas de luz multiplicam o raio da fogueira (LIGHT-09, AC 2)
Proof: walk - Com nível 4 e "+20% luz", ler 6,0.

**C10** - No nível 10, o menu mostra "Luz no máximo" e desliga a compra (LIGHT-16, edge case)
Proof: walk - Pôr `LightLevel` 10 no `save.json` e abrir o menu.

**C11** - Sem óleo, a compra fica desligada e o custo na cor de aviso (LIGHT-17, edge case)
Proof: walk - Com 0 de óleo, abrir o menu.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** T3 em diante - o Claude faz o resto da fogueira. Usuário: "pode fazer a fogueira." | T2 - o Claude escreveu o `LightRules` a pedido do usuário ("pode fazer isso também.") | T1 - o Claude terminou o `LightConfig` a pedido do usuário ("Faça o LightConfig e termine o RunRewards.") | antes da T1 - os orbes de óleo saíram do jogo: LIGHT-10 a LIGHT-15, LIGHT-18 e LIGHT-19, as tasks do orbe (antigas T7 a T10) e as checks C10 a C15, C18 e C19 foram removidas, e o `LightConfig` fica só com luz e custo. Usuário, em 2026-11-04: "Acredito que isso possa ser removido do gdd. nao vamos spawnar o oleo no mapa. será apenas de bosses."
**Dispatched:** none
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5 ------→ T6
Phase 2:  T7 ------→ T8 ------→ T9
```

Execution is strictly sequential. A feature tem 9 tasks.
