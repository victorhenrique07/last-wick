# Ferreiro: melhoria da arma Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

O usuário pediu que o Claude faça todas as tasks desta feature, código incluído ("Claude escreve tudo", 2026-11-06), no lugar da AD-001.

---

**Design**: `.specs/features/blacksmith-weapon/design.md`
**Status**: Approved
**Approved**: 2026-11-06 - user: "Aprovo, sub-agentes"
**Revisão**: 2026-11-06 - ajustes da revisão contra o código atual (usuário: "Aplica os 7 ajustes"). Primeira aprovação: user: "Aprovo todas".
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

### Phase 1: Armas como dados

A arma sai do prefab e vira asset.

```
T1 → T2 → T3 → T4 → T5 → T6 → T7
```

### Phase 2: Arma na run

A run usa a arma do save.

```
T7 → T8 → T9 → T10
```

### Phase 3: Menu do ferreiro

A melhoria no hub.

```
T10 → T11 → T12 → T13 → T14
```

---

## Task Breakdown

### T1: Criar `WeaponLevel` e `WeaponData` (Claude)

**What**: Criar o asset de arma com ID, nome, ícone, projétil, base e níveis.
**Where**: `Assets/_Project/Code/Runtime/Combat/WeaponData.cs`
**Depends on**: None
**Reuses**: formato do `CardData`
**Requirement**: SMITH-11, SMITH-12

**Done when**:

- [x] `MaxLevel` é o tamanho de `Levels`
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(weapons): adiciona armas como dados`

---

### T2: Criar `WeaponCatalog` (Claude)

**What**: Criar a lista de armas com `StartingWeapon` e `Find`, que ignora entradas nulas e sem Id, e o `OnValidate` que avisa Id vazio ou repetido.
**Where**: `Assets/_Project/Code/Runtime/Combat/WeaponCatalog.cs`
**Depends on**: T1
**Reuses**: `CardCollection`
**Requirement**: SMITH-13

**Done when**:

- [x] `Find` devolve `null` para ID desconhecido e para ID vazio
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(weapons): adiciona catálogo de armas`

---

### T3: Criar `WeaponStats` (Claude)

**What**: Somar a base e os níveis 1 até o nível, preso em `0..MaxLevel`.
**Where**: `Assets/_Project/Code/Runtime/Combat/WeaponStats.cs`
**Depends on**: T2
**Reuses**: nada
**Requirement**: SMITH-09, SMITH-17

**Done when**:

- [x] Nível 2 da arma inicial dá 12 de dano e 4,4 de cadência
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(weapons): calcula os status da arma por nível`

---

### T4: `GameDatabase` com `Weapons` (Claude)

**What**: Adicionar `Weapons`, `FindWeapon` e o erro de arma inicial vazia.
**Where**: `Assets/_Project/Code/Runtime/Save/GameDatabase.cs`
**Depends on**: T3
**Reuses**: `FindCard`
**Requirement**: SMITH-13

**Done when**:

- [ ] Sem `StartingWeapon`, o `OnValidate` registra erro
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): liga o catálogo de armas no GameDatabase`

---

### T5: Save com armas (Claude)

**What**: Equipar a arma inicial no save novo, tirar de `WeaponLevels` os IDs vazios, repetidos e desconhecidos, e trocar arma equipada desconhecida pela inicial.
**Where**: `Assets/_Project/Code/Runtime/Save/SaveSystem.cs`
**Depends on**: T4
**Reuses**: limpeza de IDs de carta
**Requirement**: SAVE-10, SAVE-19, SAVE-22

**Done when**:

- [ ] `EquippedWeapon` desconhecido carrega como a arma inicial
- [ ] `WeaponLevels` com o mesmo ID duas vezes carrega com uma entrada só
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): valida as armas do save`

---

### T6: Criar `StartingWeapon.asset` (Claude)

**What**: Copiar projétil, 10 de dano e 4 de cadência do `Player.prefab` e criar os 5 níveis com custos 10 a 50.
**Where**: `Assets/_Project/Data/Weapons/StartingWeapon.asset`
**Depends on**: T5
**Reuses**: valores do `Weapon` no `Player.prefab`
**Requirement**: SMITH-11, SMITH-12

**Done when**:

- [ ] 5 níveis alternando +2 de dano e +10% de cadência
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(weapons): cria a arma inicial`

---

### T7: Criar `WeaponCatalog.asset` (Claude)

**What**: Criar o catálogo com a arma inicial e ligar no `GameDatabase`.
**Where**: `Assets/_Project/Data/Weapons/WeaponCatalog.asset`
**Depends on**: T6
**Reuses**: asset da T6
**Requirement**: SMITH-13

**Done when**:

- [ ] Sem erro de arma inicial no Console
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(weapons): cria o catálogo de armas`

---

### T8: `Weapon.Equip` e `TryFire` com dados (Claude)

**What**: Tirar os números serializados, criar `Equip` e usar `WeaponStats` no disparo, multiplicando pelos bônus das cartas.
**Where**: `Assets/_Project/Code/Runtime/Combat/Weapon.cs`
**Depends on**: T7
**Reuses**: `TryFire` atual
**Requirement**: SMITH-08, SMITH-09, SMITH-10

**Done when**:

- [ ] Sem arma equipada, `TryFire` devolve falso com um erro
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(weapons): a arma atira com os dados do asset`

---

### T9: `PlayerAttack` equipa a arma do save (Claude)

**What**: No `Start`, achar a arma equipada e o nível e chamar `Equip`.
**Where**: `Assets/_Project/Code/Runtime/Player/PlayerAttack.cs`
**Depends on**: T8
**Reuses**: `SaveSystem`, `GameDatabase`
**Requirement**: SMITH-08, SMITH-14

**Done when**:

- [ ] A run atira como antes num save novo
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): equipa a arma do save`

---

### T10: Ícone da arma inicial (Claude)

**What**: Desenhar o ícone e ligar no asset.
**Where**: `Assets/_Project/Art/Sprites/UI/Weapons/StartingWeapon.png`
**Depends on**: T9
**Reuses**: paleta do tileset
**Requirement**: SMITH-02

**Done when**:

- [ ] Ícone ligado ao `StartingWeapon.asset`
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `art(weapons): adiciona ícone da arma inicial`

---

### T11: Criar `SmithShop` (Claude)

**What**: Criar `Equipped`, `Level`, `IsMaxed`, `NextCost` e `TryUpgrade`.
**Where**: `Assets/_Project/Code/Runtime/Combat/SmithShop.cs`
**Depends on**: T10
**Reuses**: `SaveSystem.TrySpend`
**Requirement**: SMITH-05, SMITH-06, SMITH-15, SMITH-18

**Done when**:

- [ ] `TryUpgrade` no último nível devolve falso
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(weapons): adiciona a melhoria da arma`

---

### T12: Criar `WeaponStatRowView` (Claude)

**What**: Criar a linha "status: atual → próximo".
**Where**: `Assets/_Project/Code/Runtime/UI/WeaponStatRowView.cs`
**Depends on**: T11
**Reuses**: `StatEntryView`
**Requirement**: SMITH-03

**Done when**:

- [ ] Mostra os dois valores
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona a linha de status da arma`

---

### T13: Criar `SmithMenu` (Claude)

**What**: Mostrar arma, nível, status que mudam, custo e "Forjar", e melhorar pelo `SmithShop`.
**Where**: `Assets/_Project/Code/Runtime/Hub/SmithMenu.cs`
**Depends on**: T12
**Reuses**: `HubPanel`, `WeaponStats`
**Requirement**: SMITH-01 a SMITH-04, SMITH-07, SMITH-15, SMITH-16

**Done when**:

- [ ] No máximo, mostra "Arma no nível máximo"
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): adiciona o menu do ferreiro`

---

### T14: Painel do ferreiro no builder (Claude)

**What**: Montar o painel e ligar o `onClick` da bigorna ao `SmithMenu.Open`.
**Where**: `Assets/_Project/Code/Editor/HubCanvasBuilder.cs` (`BuildSmithMenu`, `Build` com `out SmithMenu`) e `Assets/_Project/Code/Editor/HubSceneBuilder.cs` (clique da bigorna); a cena `Hub` regerada
**Depends on**: T13
**Reuses**: painel do padre
**Requirement**: SMITH-01, SMITH-07

**Done when**:

- [ ] Clicar na bigorna abre o painel
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(hub): monta o painel do ferreiro`

---

## Checks

**C1** - Clicar na bigorna abre o menu do ferreiro (SMITH-01, AC 1)
Proof: walk - Clicar na bigorna no hub.

**C2** - O menu mostra nome, ícone e "Nível N/M" (SMITH-02, AC 2)
Proof: walk - Num save novo, ler "Nível 0/5".

**C3** - O menu mostra os status atuais e o que muda (SMITH-03, AC 3)
Proof: walk - Ler "Dano 10 → 12".

**C4** - O menu mostra o custo do próximo nível em osso, ferro e cera (SMITH-04, AC 4)
Proof: walk - Ler os três valores do nível 1.

**C5** - Melhorar tira o custo dos materiais (SMITH-05, AC 5)
Proof: walk - Com os materiais exatos, forjar e ver 0.

**C6** - Melhorar sobe o nível em 1 no save (SMITH-06, AC 6)
Proof: walk - Ler `WeaponLevels` com nível 1 no `save.json`.

**C7** - "Fechar" e Esc fecham o menu (SMITH-07, AC 7)
Proof: walk - Fechar uma vez por botão e outra por Esc.

**C8** - A run usa a arma equipada com a base do asset (SMITH-08, AC 1)
Proof: walk - Num save novo, o dano por tiro é 10.

**C9** - A run soma os bônus dos níveis 1 até o atual (SMITH-09, AC 2)
Proof: walk - Com nível 2, ler dano 12 e cadência 4,4 no Inspector do `Weapon` em modo Debug.

**C10** - As cartas se aplicam sobre a arma melhorada (SMITH-10, AC 3)
Proof: walk - Num save com a Força do padre em 0, nível 2 e "+20% dano", o tiro dá 14,4; com a Força em 1, o tiro soma o dano do padre antes dos 20%.

**C11** - A arma é um asset com ID, nome, ícone, projétil, base e níveis (SMITH-11, AC 1)
Proof: walk - Abrir o `StartingWeapon.asset` no Inspector.

**C12** - Cada nível tem bônus e custo (SMITH-12, AC 2)
Proof: walk - Ler os 5 níveis no Inspector.

**C13** - Existe uma lista de armas com uma inicial (SMITH-13, AC 3)
Proof: walk - Abrir o `WeaponCatalog.asset`.

**C14** - Uma arma nova roda sem mudar código (SMITH-14, AC 4)
Proof: walk - Duplicar a arma com outro projétil e ID, pôr no catálogo, mudar `EquippedWeapon` no save e jogar.

**C15** - No último nível, aparece "Arma no nível máximo" e a melhoria desliga (SMITH-15, edge case)
Proof: walk - Com nível 4 e os materiais do nível 5, forjar e ver "Arma no nível máximo" com o botão desligado; e pôr nível 5 no `save.json` e abrir o menu.

**C16** - Sem materiais, a melhoria desliga e o que falta fica na cor de aviso (SMITH-16, edge case)
Proof: walk - Com 0 materiais, abrir o menu e ver os três custos em #C96D45 e o botão desligado; com só um material faltando, ver só ele em #C96D45.

**C17** - Nível salvo acima do máximo usa o último (SMITH-17, edge case)
Proof: walk - Pôr nível 9 no `save.json` e ler o dano do nível 5 na run.

**C18** - Cliques rápidos sobem um nível por clique, sem material negativo (SMITH-18, edge case)
Proof: walk - Com custo de 2 níveis, clicar 5 vezes rápido: sobe 2 e nada fica negativo.

**C19** - Arma equipada desconhecida no save vira a arma inicial (design, Error Handling; SAVE-22)
Proof: walk - Pôr `EquippedWeapon` "nao-existe" no `save.json`, carregar e ver a arma inicial no ferreiro e na run.

**C20** - Catálogo sem arma inicial dá erro no Editor (design, Error Handling)
Proof: walk - Esvaziar o `StartingWeapon` do `WeaponCatalog` em memória, chamar o `OnValidate` do `GameDatabase`, ver o erro no Console e restaurar.

**C21** - `WeaponLevels` com Id vazio, repetido ou desconhecido é limpo ao carregar (design, Error Handling)
Proof: walk - Pôr no `save.json` a arma inicial duas vezes (níveis 1 e 3), um Id vazio e um desconhecido, carregar e ver uma entrada só da arma inicial, com nível 1.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** 2026-11-06 - o Claude faz todas as tasks, código incluído (usuário: "Claude escreve tudo"); execução em sub-agentes por lote (usuário: "Aprovo, sub-agentes").
**Dispatched:** lote 1 (Fase 1: T1 a T7) a partir de `e352ec8`, em 2026-11-06
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2 → Phase 3

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5 ------→ T6 ------→ T7
Phase 2:  T8 ------→ T9 ------→ T10
Phase 3:  T11 ------→ T12 ------→ T13 ------→ T14
```

Execution is strictly sequential. A feature tem 14 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
