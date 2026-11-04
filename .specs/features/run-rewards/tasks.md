# Recompensas da run Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/run-rewards/design.md`
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

Os números e as contas, sem cena.

```
T1 → T2 → T3 → T4 → T5
```

### Phase 2: Fim da run

As recompensas chegam ao `RunResult` e ao save.

```
T5 → T6 → T7 → T8
```

### Phase 3: Tela de resultado

O bloco de recompensas na tela.

```
T8 → T9 → T10 → T11 → T12
```

---

## Task Breakdown

### T1: Evento `OilCollected` (usuário)

**What**: Adicionar o evento e o `RaiseOilCollected(int)`.
**Where**: `Assets/_Project/Code/Runtime/Core/GameEvents.cs`
**Depends on**: None
**Reuses**: padrão dos eventos atuais
**Requirement**: RWD-06

**Done when**:

- [x] `GameEvents.RaiseOilCollected` existe
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(run): adiciona evento de óleo coletado`

---

### T2: Criar `RewardConfig` (usuário)

**What**: Criar o ScriptableObject com `MaterialChance` e o array `BossReward`.
**Where**: `Assets/_Project/Code/Runtime/Run/RewardConfig.cs`
**Depends on**: T1
**Reuses**: formato do `RunConfig`
**Requirement**: RWD-07

**Done when**:

- [x] O Inspector mostra a chance e uma linha por boss
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(run): adiciona RewardConfig`

---

### T3: Criar `RewardConfig.asset` (Claude)

**What**: Preencher 0,2 de chance e os bosses 10/30/10, 20/30/15, 40/50/20 (ouro/óleo/materiais).
**Where**: `Assets/_Project/Data/Run/RewardConfig.asset`
**Depends on**: T2
**Reuses**: valores da spec
**Requirement**: RWD-03, RWD-04, RWD-05

**Done when**:

- [x] O asset tem 3 bosses com os valores da spec
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(run): cria a configuração de recompensas`

---

### T4: Criar `RewardRules` (usuário)

**What**: Criar `Deliver` e `RandomMaterials` como funções puras.
**Where**: `Assets/_Project/Code/Runtime/Run/RewardRules.cs`
**Depends on**: T3
**Reuses**: `ResourceBag.Halved`
**Requirement**: RWD-02, RWD-05, RWD-09, RWD-10, RWD-18

**Done when**:

- [x] `Deliver` em vitória devolve tudo; nos outros motivos, a metade para baixo
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(run): adiciona regras de recompensa`

---

### T5: Criar `RunRewards` (usuário)

**What**: Somar materiais por abate, recompensas de boss e óleo, e parar de somar no `RunFinished`.
**Where**: `Assets/_Project/Code/Runtime/Run/RunRewards.cs`
**Depends on**: T4
**Reuses**: `RunProgress`, `RewardRules`
**Requirement**: RWD-01 a RWD-06, RWD-12, RWD-19, RWD-20

**Done when**:

- [x] `Collected` cresce durante a run e não muda depois do fim
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(run): soma as recompensas coletadas na run`

---

### T6: `RunResult` com `Collected` e `Delivered` (usuário)

**What**: Trocar os cinco `int` por dois `ResourceBag`.
**Where**: `Assets/_Project/Code/Runtime/Run/RunResult.cs`
**Depends on**: T5
**Reuses**: `ResourceBag`
**Requirement**: RWD-08

**Done when**:

- [x] `RunResult` tem `Collected` e `Delivered`
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `refactor(run): guarda recompensas como ResourceBag`

---

### T7: Entregar no `RunFinisher` (usuário)

**What**: Montar coletado e entregue, somar ao save, gravar e só então levantar `RunFinished`, com trava contra fim duplo.
**Where**: `Assets/_Project/Code/Runtime/Run/RunFinisher.cs`
**Depends on**: T6
**Reuses**: `BuildResult`, `SaveSystem`
**Requirement**: RWD-09, RWD-10, RWD-11

**Done when**:

- [x] O save é gravado antes de `RaiseRunFinished`
- [x] Um segundo fim na mesma run é ignorado
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(run): entrega as recompensas ao save no fim da run`

---

### T8: Ligar `RunRewards` na `FirstOne` (Claude)

**What**: Pôr o componente no objeto da run com o `RewardConfig` e ligar a referência do `RunFinisher`.
**Where**: `Assets/_Project/Scenes/Maps/FirstOne.unity`
**Depends on**: T7
**Reuses**: objeto da run atual
**Requirement**: RWD-07

**Done when**:

- [x] A cena salva com as referências ligadas
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(run): liga as recompensas na cena`

---

### T9: Ícones dos 5 recursos (Claude)

**What**: Desenhar ouro, óleo, osso, ferro e cera em pixel art na paleta do jogo.
**Where**: `Assets/_Project/Art/Sprites/UI/Resources/`
**Depends on**: T8
**Reuses**: paleta do tileset
**Requirement**: RWD-13

**Done when**:

- [x] 5 ícones importados com Point e PPU da UI
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `art(ui): adiciona ícones dos recursos`

---

### T10: Criar `RewardEntryView` (Claude)

**What**: Criar a view com ícone, nome e quantidade, em uma ou duas colunas.
**Where**: `Assets/_Project/Code/Runtime/UI/RewardEntryView.cs`
**Depends on**: T9
**Reuses**: `StatEntryView`
**Requirement**: RWD-13, RWD-14

**Done when**:

- [x] `Show(delivered)` e `Show(collected, delivered)` mudam o texto
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona a entrada de recompensa`

---

### T11: Bloco de recompensas no builder (Claude)

**What**: Criar o bloco com 5 entradas e a linha de perda na tela de resultado, mantendo o resto.
**Where**: `Assets/_Project/Code/Editor/RunResultScreenBuilder.cs`
**Depends on**: T10
**Reuses**: molduras e `Ornament` do builder
**Requirement**: RWD-13, RWD-15, RWD-17

**Done when**:

- [x] As prévias de vitória e de morte em `Logs/` mostram o bloco
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): adiciona o bloco de recompensas à tela de resultado`

---

### T12: Preencher o bloco no `RunResultPanel` (Claude)

**What**: Mostrar entregue na vitória; coletado, entregue e o texto de perda nos outros motivos.
**Where**: `Assets/_Project/Code/Runtime/UI/RunResultPanel.cs`
**Depends on**: T11
**Reuses**: `RaiseResultPanel`
**Requirement**: RWD-13 a RWD-16, RWD-21

**Done when**:

- [x] Vitória sem texto de perda; morte com texto
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(ui): mostra as recompensas na tela de resultado`

---

## Checks

Para acelerar os walks, use `Health.Kill` nos bosses pelo Inspector em modo Debug e leia `RunRewards.Collected` no Inspector.

**C1** - Cerca de 20% dos monstros comuns rendem 1 material (RWD-01, AC 1)
Proof: walk - Matar 100 monstros e ver entre 10 e 30 materiais em `Collected`.

**C2** - Osso, ferro e cera saem em proporções parecidas (RWD-02, AC 2)
Proof: walk - No mesmo teste, nenhum dos três fica com zero e nenhum passa de 60% do total.

**C3** - Os bosses 1, 2 e final rendem 10, 20 e 40 de ouro (RWD-03, AC 3)
Proof: walk - Matar cada boss e ver `Gold` subir 10, depois 20, depois 40.

**C4** - Os bosses 1, 2 e final rendem 30, 30 e 50 de óleo (RWD-04, AC 4)
Proof: walk - No mesmo teste, `Oil` sobe 30, 30 e 50.

**C5** - Os bosses não rendem materiais (RWD-05, AC 5)
Proof: walk - No mesmo teste, a soma de osso, ferro e cera não muda quando cada boss morre.

**C6** - O orbe de óleo soma o óleo dele (RWD-06, AC 6)
Proof: walk - Encostar num orbe e ver `Oil` subir 5.

**C7** - Os números ficam no `RewardConfig.asset` (RWD-07, AC 7)
Proof: walk - Mudar a chance para 1 no asset e ver todo monstro render material.

**C8** - O `RunResult` traz o coletado de cada recurso (RWD-08, AC 1)
Proof: walk - Com um breakpoint em `RaiseResultPanel`, ler os 5 campos de `Collected`.

**C9** - A vitória entrega 100% (RWD-09, AC 2)
Proof: walk - Vencer e ver `Delivered` igual a `Collected`.

**C10** - Morte e escuridão entregam a metade para baixo (RWD-10, AC 3)
Proof: walk - Morrer com 30 de ouro e 7 de osso coletados; `Delivered` mostra 15 e 3.

**C11** - O save ganha o entregue antes da gravação (RWD-11, AC 4)
Proof: walk - Depois da morte do teste anterior, o `save.json` tem 15 de ouro e 3 de osso a mais.

**C12** - O save não muda durante a run (RWD-12, AC 5)
Proof: walk - Matar o boss 1 e ler o `save.json`: o ouro é o do começo da run.

**C13** - A tela mostra os 5 recursos com ícone, nome e quantidade (RWD-13, AC 1)
Proof: walk - Vencer e ver as 5 entradas no bloco de recompensas.

**C14** - Na morte, cada recurso mostra coletado e entregue (RWD-14, AC 2)
Proof: walk - Morrer e ver "30 → 15" no ouro.

**C15** - Na morte, aparece o texto de perda (RWD-15, AC 3)
Proof: walk - No mesmo teste, ler "Metade do que você coletou se perdeu na escuridão."

**C16** - Na vitória, o texto de perda não aparece (RWD-16, AC 4)
Proof: walk - Vencer e não ver o texto.

**C17** - Resumo, status, cartas e botão continuam na tela (RWD-17, AC 5)
Proof: walk - Vencer e ver os quatro blocos atuais além das recompensas.

**C18** - Um recurso ímpar perde o meio na morte (RWD-18, edge case)
Proof: walk - Morrer com 1 de cera coletada e ver 0 entregue.

**C19** - Os monstros apagados no marco não rendem material (RWD-19, edge case)
Proof: walk - Chegar ao marco com 20 monstros vivos e ver `Collected` igual antes e depois.

**C20** - Nada soma depois do fim da run (RWD-20, edge case)
Proof: walk - Com a tela de resultado aberta, matar um monstro por `Health.Kill` e ver `Collected` igual.

**C21** - Sem nada coletado, a tela mostra 0 em cada recurso (RWD-21, edge case)
Proof: walk - Morrer no começo da run sem matar nada e ver 0 nas 5 entradas.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** T11 - os campos `gold`, `oil`, `bones`, `iron`, `wax` e `lossLabel` entram no `RunResultPanel` já na T11, porque o builder liga as referências neles; a T12 só escreve a lógica que os preenche; decisão do agente | T9 - os ícones ficam em `Art/Sprites/UI/Victory/` (IconGold, IconOil, IconBone, IconIron, IconWax), ao lado dos outros ícones da tela, e não em `UI/Resources/`: uma pasta chamada `Resources` é especial no Unity e entraria inteira no build; decisão do agente | T5 - os bosses não rendem materiais no MVP; RWD-05 e C5 mudaram. Usuário, na T5: "O boss não vai dropar material. Pelo menos nao por enquanto. Talvez apenas materiais especificos que são só dele, mas não agora no MVP." | T4 - o sorteio de material usa um enum dos materiais em vez de `rng.Next(3)` solto. Usuário: "acho melhor criar um Enum com os tipos de cada material, por que ficaria mais facil de adicionar um material novo no futuro."
**Dispatched:** Verifier em 2026-11-03 sobre `b9b5792` (diff `a8931ed..b9b5792`, sem o commit do menu `a355182`) - aguardando
**Walked:** 2026-11-03, walk do usuário em Play Mode na cena `FirstOne` (aberta direto no Editor; o hub ainda não leva à run). Conferido pelo agente no `save.json` (`%USERPROFILE%/AppData/LocalLow/DefaultCompany/last-wick/save.json`): uma run com boss 1 e morte levou o save de 0/0/1/2/1 a 5/15/13/17/13 (ouro/óleo/osso/ferro/cera), isto é, metade de 10 de ouro e 30 de óleo do boss 1 e 39 materiais entregues dos monstros, sem material do boss (C3, C4, C5, C10, C11). Relatado pelo usuário: a tela da morte mostrou "coletado → entregue" e o texto de perda (C14, C15), e "está tudo confirmado" para as demais checks. C6 não pôde ser jogada: o orbe de óleo é da feature `campfire-light` e ainda não existe; a prova é o teste do agente no Editor na T5, em que `RaiseOilCollected(5)` subiu o óleo do `RunRewards` em 5. Também conferido pelo agente no Editor: `RewardRules` (T4: 30/7/1 vira 15/3/0 na morte e na escuridão), `RunRewards` (T5: 192 materiais em 1000 abates, bosses 1 e 2 com +10/+30 e +20/+30, nada depois do `RunFinished`) e o bloco da tela (T12: vitória sem perda, morte com setas e perda, 0 → 0 sem nada coletado). Para reproduzir: abrir `Assets/_Project/Scenes/Maps/FirstOne.unity` e dar Play; ler `RunRewards.Collected` e matar bosses pelo Editor (`Health.Kill`).

---

## Phase Execution Map

```
Phase 1 → Phase 2 → Phase 3

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5
Phase 2:  T6 ------→ T7 ------→ T8
Phase 3:  T9 ------→ T10 ------→ T11 ------→ T12
```

Execution is strictly sequential. A feature tem 12 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
