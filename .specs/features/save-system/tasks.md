# Sistema de save Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta telas, arte, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/save-system/design.md`
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

### Phase 1: Dados e IDs

Os tipos e os assets que o save precisa existem antes do save.

```
T1 → T2 → T3 → T4 → T5 → T6 → T7
```

### Phase 2: Save

Carregar, validar e gravar o arquivo.

```
T7 → T8 → T9 → T10 → T11
```

---

## Task Breakdown

### T1: Criar `ResourceBag` (usuário)

**What**: Criar a struct com ouro, óleo, osso, ferro e cera, soma, subtração, `Covers`, `Halved` e `ClampedToZero`.
**Where**: `Assets/_Project/Code/Runtime/Save/ResourceBag.cs`
**Depends on**: None
**Reuses**: nada
**Requirement**: SAVE-01, SAVE-20

**Done when**:

- [x] `ResourceBag` tem os 5 campos `int` serializáveis
- [x] `Halved` arredonda para baixo e `ClampedToZero` troca negativos por 0
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): adiciona ResourceBag`

---

### T2: Adicionar `Id` ao `CardData` (usuário)

**What**: Criar o campo `public string Id` no topo do `CardData`.
**Where**: `Assets/_Project/Code/Runtime/Cards/CardData.cs` (o preenchimento automático fica em `Assets/_Project/Code/Editor/CardIdAssigner.cs`)
**Depends on**: T1
**Reuses**: `CardData` atual
**Requirement**: SAVE-04, SAVE-05

**Done when**:

- [x] O Inspector de uma carta mostra `Id`
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): adiciona Id às cartas`

---

### T3: Preencher o `Id` das 20 cartas (Claude)

**What**: Dar a cada carta um ID em minúsculas com hífen, derivado do nome (ex.: `chama-certeira`).
**Where**: `Assets/_Project/Data/Cards/*.asset`
**Depends on**: T2
**Reuses**: nomes atuais das cartas
**Requirement**: SAVE-04

**Done when**:

- [x] As 20 cartas têm `Id` único e não vazio
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(cards): preenche os IDs das cartas`

---

### T4: Deixar o `DeckData` só de leitura (usuário)

**What**: Expor `MinCards` e `MaxCards` e remover `TryAdd` e `Remove`.
**Where**: `Assets/_Project/Code/Runtime/Cards/DeckData.cs`
**Depends on**: T3
**Reuses**: `DeckData` atual
**Requirement**: SAVE-05, SAVE-10

**Done when**:

- [x] `MinCards` e `MaxCards` são públicos
- [x] Nenhum código chama `TryAdd` ou `Remove`
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `refactor(cards): deixa o DeckData como deck inicial`

---

### T5: Criar `GameDatabase` (usuário)

**What**: Criar o ScriptableObject com `CardCollection`, `StartingDeck` e `FindCard`, que delega a `CardCollection.Find`; o `CardCollection` ganha `Find` e o aviso de ID vazio ou repetido no `OnValidate`.
**Where**: `Assets/_Project/Code/Runtime/Save/GameDatabase.cs` (busca e validação em `Assets/_Project/Code/Runtime/Cards/CardCollection.cs`)
**Depends on**: T4
**Reuses**: `CardCollection`, `DeckData`
**Requirement**: SAVE-19

**Done when**:

- [x] `FindCard` devolve `null` para ID desconhecido
- [x] Um ID repetido gera aviso no Console ao editar o asset
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): adiciona GameDatabase`

---

### T6: Criar `CardCollection.asset` (Claude)

**What**: Criar o asset com as 20 cartas na ordem em que a Coleção vai mostrar.
**Where**: `Assets/_Project/Data/Cards/CardCollection.asset`
**Depends on**: T5
**Reuses**: `CardCollection`
**Requirement**: SAVE-19

**Done when**:

- [x] O asset lista as 20 cartas
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(cards): cria a coleção oficial de cartas`

---

### T7: Criar `GameDatabase.asset` em `Resources` (Claude)

**What**: Criar o asset apontando para `CardCollection.asset` e `Deck.asset`.
**Where**: `Assets/_Project/Resources/GameDatabase.asset`
**Depends on**: T6
**Reuses**: assets da T6
**Requirement**: SAVE-09

**Done when**:

- [x] `Resources.Load<GameDatabase>("GameDatabase")` acha o asset
- [x] Sem avisos de ID no Console
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `chore(save): cria o GameDatabase do projeto`

---

### T8: Criar `SaveData` e `IdLevel` (usuário)

**What**: Criar a classe serializável com os campos do design e `CreateNew(db)` com o estado inicial.
**Where**: `Assets/_Project/Code/Runtime/Save/SaveData.cs`
**Depends on**: T7
**Reuses**: `ResourceBag`, `DeckData`
**Requirement**: SAVE-01 a SAVE-08, SAVE-10

**Done when**:

- [x] `CreateNew` libera e põe no deck as 15 cartas do `Deck.asset`
- [x] `Version` começa em 1
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): adiciona SaveData`

---

### T9: Carregar o save (usuário)

**What**: Criar `SaveSystem` com `Data`, `Database`, `FilePath`, carga preguiçosa, arquivo corrompido, limpeza de IDs de carta, moedas negativas e reset ao entrar em Play Mode.
**Where**: `Assets/_Project/Code/Runtime/Save/SaveSystem.cs`
**Depends on**: T8
**Reuses**: `JsonUtility`, `SaveData.CreateNew`
**Requirement**: SAVE-09, SAVE-10, SAVE-11, SAVE-17, SAVE-18, SAVE-19, SAVE-20

**Done when**:

- [x] Sem arquivo, `Data` volta o estado inicial
- [x] Um arquivo inválido vira `save.corrupt.json` com aviso
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): carrega o save do disco`

---

### T10: Gravar e mudar o save (usuário)

**What**: Adicionar `Save()` com arquivo temporário, `Changed`, `NotifyChanged`, `Add` e `TrySpend`.
**Where**: `Assets/_Project/Code/Runtime/Save/SaveSystem.cs`
**Depends on**: T9
**Reuses**: T9
**Requirement**: SAVE-12 a SAVE-15, SAVE-21

**Done when**:

- [ ] `Save()` escreve `save.json.tmp` e depois troca o `save.json`
- [ ] Erro de disco vira `Debug.LogError` e o jogo segue
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): grava o save com arquivo temporário`

---

### T11: Menu "Apagar save" (Claude)

**What**: Criar o menu do Editor que apaga `save.json` e esquece a instância em memória.
**Where**: `Assets/_Project/Code/Editor/SaveMenu.cs`
**Depends on**: T10
**Reuses**: `SaveSystem.FilePath`
**Requirement**: SAVE-16

**Done when**:

- [ ] O menu "Last Wick/Apagar save" aparece e apaga o arquivo
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(save): adiciona menu para apagar o save`

---

## Checks

O arquivo fica em `Application.persistentDataPath` (no Windows, `%USERPROFILE%/AppData/LocalLow/<empresa>/<produto>/save.json`). SAVE-12 a SAVE-14 e SAVE-22 são ligados pelas features que gravam e pela `blacksmith-weapon`; os walks deles rodam depois dessas features.

**C1** - O `save.json` traz `Gold`, `Oil`, `Bones`, `Iron` e `Wax` como inteiros (SAVE-01, AC 1)
Proof: walk - Abrir o `save.json` depois de uma gravação e ler os 5 campos dentro de `Resources`.

**C2** - O `save.json` traz `PriestLevels` com `Id` e `Level` (SAVE-02, AC 2)
Proof: walk - Depois de comprar um ponto no padre, o arquivo mostra o ID do status com nível 1.

**C3** - O `save.json` traz `LightLevel` (SAVE-03, AC 3)
Proof: walk - Depois de comprar luz na fogueira, o arquivo mostra `LightLevel` 1.

**C4** - O `save.json` traz `UnlockedCards` com os IDs liberados (SAVE-04, AC 4)
Proof: walk - Num save novo, o arquivo lista os 15 IDs das cartas do `Deck.asset`.

**C5** - O `save.json` traz `Deck` na ordem montada (SAVE-05, AC 5)
Proof: walk - Tirar e pôr de volta uma carta na mesa; o ID dela vai para o fim da lista.

**C6** - O `save.json` traz `WeaponLevels` com `Id` e `Level` (SAVE-06, AC 6)
Proof: walk - Depois de forjar uma vez, o arquivo mostra o ID da arma inicial com nível 1.

**C7** - O `save.json` traz `EquippedWeapon` (SAVE-07, AC 7)
Proof: walk - Num save novo, o arquivo mostra o ID da arma inicial.

**C8** - O `save.json` traz `Version` 1 (SAVE-08, AC 8)
Proof: walk - Ler `"Version": 1` no arquivo de um save novo.

**C9** - Hub e run leem a mesma instância (SAVE-09, AC 9)
Proof: walk - Ganhar recursos numa run e voltar ao hub: a barra mostra o mesmo valor sem reiniciar o Play Mode.

**C10** - Sem arquivo, o save começa com moedas 0, níveis 0, 15 cartas liberadas, deck de 15 e arma inicial (SAVE-10, AC 1)
Proof: walk - Usar "Apagar save", entrar em Play Mode e ler os valores no hub e no arquivo gravado.

**C11** - Com arquivo, o save carrega os valores dele (SAVE-11, AC 2)
Proof: walk - Editar `Gold` para 123 no arquivo, entrar em Play Mode e ver 123 no hub.

**C12** - O fim da run grava o arquivo (SAVE-12, AC 3)
Proof: walk - Terminar uma run e ver a data de modificação do `save.json` mudar.

**C13** - Uma compra no hub grava o arquivo (SAVE-13, AC 4)
Proof: walk - Comprar um ponto no padre e ver o arquivo mudar no mesmo instante.

**C14** - Mudar o deck grava o arquivo (SAVE-14, AC 5)
Proof: walk - Tirar uma carta do deck e ver a lista `Deck` do arquivo sem ela.

**C15** - A gravação passa por `save.json.tmp` (SAVE-15, AC 6)
Proof: walk - Com um breakpoint em `Save()`, ver o `.tmp` criado antes da troca; depois da troca, só `save.json` existe.

**C16** - O menu "Last Wick/Apagar save" apaga o arquivo (SAVE-16, AC 1)
Proof: walk - Usar o menu e ver o `save.json` sumir da pasta.

**C17** - Um arquivo inválido é renomeado para `save.corrupt.json` (SAVE-17, edge case)
Proof: walk - Escrever `abc` no `save.json`, entrar em Play Mode e ver o `save.corrupt.json` com `abc`.

**C18** - Depois de um arquivo inválido, o jogo começa do zero com aviso (SAVE-18, edge case)
Proof: walk - No mesmo teste, o hub mostra 0 de tudo e o Console tem o aviso.

**C19** - Um ID desconhecido sai e o resto fica (SAVE-19, edge case)
Proof: walk - Pôr `carta-falsa` em `UnlockedCards`; o jogo abre com as outras cartas e avisa o ID no Console.

**C20** - Moeda negativa carrega como 0 (SAVE-20, edge case)
Proof: walk - Escrever `Gold` -5 no arquivo; o hub mostra 0.

**C21** - Falha de gravação vira erro e o jogo segue (SAVE-21, edge case)
Proof: walk - Marcar `save.json` como somente leitura, comprar no hub: erro no Console e a compra continua valendo na tela.

**C22** - Arma equipada desconhecida vira a arma inicial (SAVE-22, edge case)
Proof: walk - Escrever `EquippedWeapon` `arma-falsa` e jogar uma run: o player atira com a arma inicial.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** 2026-11-03 - usuário escreveu a T1; a pedido ("adicione os namespaces"), o agente pôs o `ResourceBag` em `Assets._Project.Code.Runtime.Save`. O usuário pediu que o agente fizesse a T2 e a T3 e que "OS IDS DEVEM SER PREENCHIDOS AUTOMATICAMENTE". Por isso a T2 ganhou o `CardIdAssigner` (Editor), que dá a cada carta sem Id, ou com Id repetido, o nome do arquivo em minúsculas com hífen. O agente também pôs `CardData` e `GameEvents` em namespaces, a pedido. Na T5, o usuário achou `Cards.Cards` "ambiguo e feio"; a busca e a validação de IDs passaram para o `CardCollection` (`Find`, `OnValidate`), e o `GameDatabase` expõe `CardCollection` e delega `FindCard`.
**Dispatched:** none
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5 ------→ T6 ------→ T7
Phase 2:  T8 ------→ T9 ------→ T10 ------→ T11
```

Execution is strictly sequential. A feature tem 11 tasks; no Execute, as fases viram lotes de cerca de 7 tasks.
