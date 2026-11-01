# Spawn por ondas Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pelo `CLAUDE.md` do projeto, o usuário escreve o código, salvo pedido explícito para que o agente implemente.

---

**Design**: `.specs/features/spawn-waves/design.md`
**Status**: Approved
**Approved**: 2026-11-01 - user: "Aprovo"
**Build**: none - o projeto não tem comando de build; a compilação é conferida no Console do Unity Editor

---

## Test Coverage Matrix

> Generated from codebase, project guidelines, and spec - confirm before Execute. Guidelines found: none. O usuário escolheu "Só teste manual".

| Code Layer | Required Test Type | Coverage Expectation | Location Pattern | Run Command |
| ---------- | ------------------ | -------------------- | ---------------- | ----------- |
| Runtime (`Spawner`, `SpawnEntry`) | none | Cada critério conferido no walk em Play Mode | - | Play Mode no Unity Editor |
| Dados (cena, `RunConfig.asset`) | none | Cada critério conferido no walk em Play Mode | - | Play Mode no Unity Editor |

## Gate Check Commands

> Generated from codebase - confirm before Execute.

| Gate Level | When to Use | Command |
| ---------- | ----------- | ------- |
| Build | Depois de cada task | Sem comando: o Console do Unity Editor sem erros de compilação |

---

## Execution Plan

Phases are ordered and run sequentially - each phase completes before the next begins, and tasks within a phase execute in order.

### Phase 1: Dados das ondas

As ondas entram nos dados antes de o `Spawner` usá-las, e o jogo segue funcionando.

```
T1 → T2
```

### Phase 2: Lógica do Spawner

```
T2 → T3 → T4 → T5
```

### Phase 3: Limpeza e marcos

```
T5 → T6 → T7
```

---

## Task Breakdown

### T1: Adicionar `SpawnWave` e `waves` aos dados

**What**: Criar a classe `SpawnWave` e o array `waves` em `SpawnStage`, mantendo `spawnInterval` por enquanto.
**Where**: `Assets/_Project/Code/Runtime/Run/SpawnEntry.cs`
**Depends on**: None
**Reuses**: formato de `SpawnEntry`
**Requirement**: WAVE-15, WAVE-16

**Done when**:

- [ ] O Inspector mostra `duration`, `minAlive` e `spawnInterval` em cada onda
- [ ] `SpawnStage` mostra `waves`, `enemies` e `maxAlive`
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(spawn): adiciona os dados das ondas`

---

### T2: Preencher as ondas da cena

**What**: Preencher as ondas dos 3 stages com os valores provisórios do design.
**Where**: `Assets/_Project/Scenes/Maps/FirstOne.unity`
**Depends on**: T1
**Reuses**: `enemies` e `maxAlive` atuais da cena
**Requirement**: WAVE-18

**Done when**:

- [ ] Cada um dos 3 stages tem 4 ondas
- [ ] A cena foi salva
- [ ] Console sem erros

**Tests**: none
**Gate**: build

**Commit**: `feat(spawn): configura as ondas da cena FirstOne`

---

### T3: Escolher a onda ativa

**What**: Criar `stageTime` e `CurrentWave`, somar o tempo durante o spawn e zerá-lo em `OnBossDefeated`.
**Where**: `Assets/_Project/Code/Runtime/Run/Spawner.cs`
**Depends on**: T2
**Reuses**: padrão de subtração do `PickEnemy`
**Requirement**: WAVE-01, WAVE-05, WAVE-11, WAVE-21

**Done when**:

- [ ] `CurrentWave` segue as 6 regras do design
- [ ] `stageTime` para durante os bosses e volta a zero quando o boss morre
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(spawn): escolhe a onda ativa pelo tempo do stage`

---

### T4: Tratar stage sem ondas

**What**: Sair do `FixedUpdate` em stage sem ondas e registrar um erro uma vez por stage.
**Where**: `Assets/_Project/Code/Runtime/Run/Spawner.cs`
**Depends on**: T3
**Reuses**: `Debug.LogError`
**Requirement**: WAVE-19, WAVE-20

**Done when**:

- [ ] Um stage sem ondas não cria inimigos
- [ ] O erro aparece uma vez, mesmo com o jogo rodando
- [ ] `OnBossDefeated` libera um novo aviso para o stage seguinte
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(spawn): avisa quando um stage nao tem ondas`

---

### T5: Aplicar o ritmo das ondas

**What**: Adicionar `fillInterval`, escolher o intervalo pelos vivos, trocar `==` por `>=` em `maxAlive` e remover `using NUnit.Framework`.
**Where**: `Assets/_Project/Code/Runtime/Run/Spawner.cs`
**Depends on**: T4
**Reuses**: `timer`, `PickEnemy` e `GetSpawnPosition`
**Requirement**: WAVE-02, WAVE-03, WAVE-04, WAVE-06, WAVE-07, WAVE-08, WAVE-09, WAVE-10, WAVE-12, WAVE-13, WAVE-17, WAVE-22, WAVE-23

**Done when**:

- [ ] O `Spawner` usa `fillInterval` abaixo do `minAlive`
- [ ] O `Spawner` usa `wave.spawnInterval` a partir do `minAlive`
- [ ] Nada de `stage.spawnInterval` restou no `Spawner`
- [ ] O fluxo de `OnMilestoneReached`, `OnStageCleared` e `OnPlayerDied` continua igual
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(spawn): aplica o ritmo das ondas`

---

### T6: Remover o intervalo antigo do stage

**What**: Remover `spawnInterval` de `SpawnStage`.
**Where**: `Assets/_Project/Code/Runtime/Run/SpawnEntry.cs`
**Depends on**: T5
**Reuses**: nenhum
**Requirement**: WAVE-16

**Done when**:

- [ ] `SpawnStage` tem apenas `enemies`, `waves` e `maxAlive`
- [ ] A cena foi salva novamente
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `refactor(spawn): remove o intervalo fixo do stage`

---

### T7: Voltar os marcos ao one-pager

**What**: Definir os marcos 3000, 7500 e 14000 no `RunConfig.asset`, depois de conferir stages e bosses.
**Where**: `Assets/_Project/Data/Run/RunConfig.asset`
**Depends on**: T6
**Reuses**: valores de `Docs/Last Wick - One-Pager.md`
**Requirement**: WAVE-14

**Done when**:

- [ ] O Inspector mostra 3000, 7500 e 14000
- [ ] Uma run completa chega à tela de vitória
- [ ] Console sem erros

**Tests**: none
**Gate**: build

**Commit**: `chore(run): volta os marcos aos valores do one-pager`

---

## Checks

Para ver os valores privados `currentAlive`, `stageTime` e `timer`, use o Inspector do `Spawner` em modo Debug.

**C1** - A onda ativa muda quando `stageTime` passa da soma das `duration` anteriores (WAVE-01, AC 1)
Proof: walk - Com ondas de 5s e `minAlive` diferentes, a meta de vivos muda aos 5s e aos 10s.

**C2** - Abaixo do `minAlive`, nasce um inimigo a cada `fillInterval` (WAVE-02, AC 2)
Proof: walk - Com `fillInterval` em 0,5 e sem atirar, `currentAlive` sobe 1 a cada 0,5s até o mínimo.

**C3** - Do `minAlive` até `maxAlive`, nasce um inimigo a cada `spawnInterval` (WAVE-03, AC 3)
Proof: walk - Com `spawnInterval` em 2 e sem atirar, `currentAlive` sobe 1 a cada 2s depois do mínimo.

**C4** - Em `maxAlive`, nenhum inimigo nasce (WAVE-04, AC 4)
Proof: walk - Com `maxAlive` em 5 e sem atirar, `currentAlive` para em 5.

**C5** - A última onda fica ativa depois da soma das `duration` (WAVE-05, AC 5)
Proof: walk - Com `stageTime` além do total, os valores da última onda continuam valendo.

**C6** - Os inimigos seguem os pesos de `enemies` do stage (WAVE-06, AC 6)
Proof: walk - Com peso 1 em um tipo e 0 nos demais, só esse tipo nasce em todas as ondas.

**C7** - Nasce no máximo um inimigo por `FixedUpdate` (WAVE-07, AC 7)
Proof: walk - Com `fillInterval` em 0,02, `currentAlive` sobe de um em um no modo Pause com Step.

**C8** - Ao alcançar um marco, os spawns param (WAVE-08, AC 1)
Proof: walk - Depois do marco, nenhum inimigo comum nasce durante o boss.

**C9** - Ao alcançar um marco, os inimigos vivos somem (WAVE-09, AC 2)
Proof: walk - `currentAlive` vai a 0 quando o marco é alcançado.

**C10** - Ao derrotar um boss comum, o spawn volta no stage seguinte (WAVE-10, AC 3)
Proof: walk - Depois do `Boss1`, inimigos voltam a nascer com a mistura do stage 2.

**C11** - Um stage novo começa na primeira onda (WAVE-11, AC 4)
Proof: walk - `stageTime` volta a 0 e a meta de vivos é a da primeira onda.

**C12** - Depois do último boss, nenhum inimigo nasce (WAVE-12, AC 5)
Proof: walk - Com a tela de vitória aberta, `currentAlive` não sobe.

**C13** - Quando o player morre, os spawns param (WAVE-13, AC 6)
Proof: walk - Depois da morte, `currentAlive` não sobe.

**C14** - O `RunConfig.asset` define os marcos 3000, 7500 e 14000 (WAVE-14, AC 7)
Proof: walk - O Inspector do `RunConfig` mostra os três valores.

**C15** - `SpawnWave` mostra `duration`, `minAlive` e `spawnInterval` (WAVE-15, AC 1)
Proof: walk - Os três campos aparecem em cada onda no Inspector.

**C16** - `SpawnStage` mostra `waves`, `enemies` e `maxAlive` (WAVE-16, AC 2)
Proof: walk - Os três campos aparecem em cada stage, sem `spawnInterval`.

**C17** - O `Spawner` mostra `fillInterval` (WAVE-17, AC 3)
Proof: walk - O campo aparece no Inspector com 0,1.

**C18** - A cena `FirstOne` tem ondas nos 3 stages (WAVE-18, AC 4)
Proof: walk - Os 3 stages mostram pelo menos uma onda no Inspector.

**C19** - Um stage sem ondas não cria inimigos (WAVE-19, Edge 1)
Proof: walk - Com `waves` vazio no Play Mode, `currentAlive` não sobe.

**C20** - Um stage sem ondas registra um erro uma vez (WAVE-20, Edge 2)
Proof: walk - O Console mostra uma única ocorrência do erro.

**C21** - Uma onda com `duration` igual ou menor que 0 é pulada (WAVE-21, Edge 3)
Proof: walk - Com a primeira onda em `duration` 0, o stage começa na segunda.

**C22** - O `maxAlive` tem prioridade sobre o `minAlive` (WAVE-22, Edge 4)
Proof: walk - Com `minAlive` 20 e `maxAlive` 5, `currentAlive` para em 5.

**C23** - Reduzir o mínimo na troca de onda não remove inimigos (WAVE-23, Edge 5)
Proof: walk - Ao trocar de `minAlive` 30 para 5, `currentAlive` não cai sem abates.

---

## Handoff

**Settled mid-build:** 2026-11-01 - usuário escreveu T1, T3, T4, T5 e o código da T6; agente fez T2, salvou a cena da T6 e fez T7 a pedido ("faça isso tudo para mim e verifique se o projeto esta compilando"). `using NUnit.Framework` segue em `Spawner.cs`: a compilação de player para StandaloneWindows64 passou com ele, então fica como limpeza pendente da T5.
**Dispatched:** none
**Walked:** 2026-11-01 - Conferência automatizada em Play Mode: C1-C7, C19-C23 por chamadas diretas ao `Spawner`, com `timeScale` 0; C8-C13 por runs com script, em tempo real. Run com marcos 3000/7500/14000: 914 abates, vitória em 141,2s e Console sem erros. Player invulnerável e sem atirar, abates e bosses resolvidos por `Health.Kill`. C14-C18 lidos dos dados. Ninguém jogou a run manualmente, e sensação e dificuldade não foram avaliadas.

---

## Phase Execution Map

```
Phase 1 → Phase 2 → Phase 3

Phase 1:  T1 ------→ T2
Phase 2:  T3 ------→ T4 ------→ T5
Phase 3:  T6 ------→ T7
```

Execution is strictly sequential. A feature tem 7 tasks e cabe em um único lote.
