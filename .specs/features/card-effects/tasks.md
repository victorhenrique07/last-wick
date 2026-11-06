# Cartas com mecânica própria Tasks

## Execution Protocol (MANDATORY -- do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, the walk, check_proofs, the Verifier and the screens sub-agent).

**If the skill cannot be activated, STOP and tell the user - do not proceed without it.**

Pela AD-001, o usuário escreve o código de gameplay e o Claude monta prefabs, assets e cenas. Cada task diz quem a faz.

---

**Design**: `.specs/features/card-effects/design.md`
**Status**: Approved
**Approved**: 2026-11-06 - user: "Aprovo as tasks"
**Build**: none - o projeto não tem comando de build; a compilação é conferida no Console do Unity Editor

---

## Test Coverage Matrix

> Generated from codebase, project guidelines, and spec - confirm before Execute. Guidelines found: none. O projeto não tem `.asmdef` nem testes; a spec escolheu provar com a primeira carta de efeito real em Play Mode.

| Code Layer | Required Test Type | Coverage Expectation | Location Pattern | Run Command |
| ---------- | ------------------ | -------------------- | ---------------- | ----------- |
| Componentes de runtime das cartas | none | Cada critério conferido no walk em Play Mode com a primeira carta de efeito real | - | Play Mode no Unity Editor |
| Prefabs e assets | none | Cada critério conferido no walk em Play Mode | - | Play Mode no Unity Editor |

## Gate Check Commands

> Generated from codebase - confirm before Execute.

| Gate Level | When to Use | Command |
| ---------- | ----------- | ------- |
| Build | Depois de cada task | Sem comando: o Console do Unity Editor sem erros de compilação |

---

## Execution Plan

Phases are ordered and run sequentially - each phase completes before the next begins, and tasks within a phase execute in order.

### Phase 1: Sistema

O campo na carta, a contagem de cópias, a base das mecânicas e o componente no player.

```
T1 → T2 → T3 → T4 → T5
```

### Phase 2: Prova

A primeira carta de efeito real, que prova o sistema. Fica bloqueada até o usuário definir a mecânica dela.

```
T5 → T6
```

---

## Task Breakdown

### T1: `effectPrefab` no `CardData` (usuário)

**What**: Adicionar o campo opcional do prefab da mecânica e a propriedade de leitura.
**Where**: `Assets/_Project/Code/Runtime/Cards/CardData.cs`
**Depends on**: None
**Reuses**: padrão dos campos atuais (`[SerializeField] private` + propriedade só de leitura)
**Requirement**: CEFX-01

**Como fazer**:

- Campo `[SerializeField] private GameObject effectPrefab;` com o `[Tooltip("Prefab com um CardEffect: a mecânica própria da carta. Vazio = só números.")]`, num `[Header("Mecânica própria")]` depois da coleta.
- Propriedade `public GameObject EffectPrefab => effectPrefab;`.
- O tipo é `GameObject`, não `CardEffect` (design, Tech Decisions): um prefab com o script removido depois tem de dar aviso, não sumir calado.

**Done when**:

- [x] O Inspector de uma carta mostra o campo vazio
- [x] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): adiciona o prefab de efeito da carta`

---

### T2: Evento `CardAdded` no `CardInventory` (usuário)

**What**: Avisar qual carta entrou e com quantas cópias ficou, depois de contar.
**Where**: `Assets/_Project/Code/Runtime/Cards/CardInventory.cs`
**Depends on**: T1
**Reuses**: o `Add` e o evento `Changed` que já existem
**Requirement**: CEFX-02, CEFX-03

**Como fazer**:

- Declarar `public event Action<CardData, int> CardAdded;`.
- No `Add`, depois de somar a cópia no dicionário e **antes** do `Changed?.Invoke()`, chamar `CardAdded?.Invoke(card, cards[card]);`.
- Não mexer em `Total` nem em `ReductionMultiplier`: os bônus de números seguem iguais (CEFX-05).

**Done when**:

- [ ] Escolher uma carta pela segunda vez dispara `CardAdded` com 2
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): avisa a carta adicionada e as cópias`

---

### T3: Classe base `CardEffect` (usuário)

**What**: Criar o contrato de toda mecânica: player, carta, cópias e parar na morte do player.
**Where**: `Assets/_Project/Code/Runtime/Cards/Effects/CardEffect.cs` (pasta nova)
**Depends on**: T2
**Reuses**: `Health.Died` (`Assets/_Project/Code/Runtime/Combat/Health.cs`)
**Requirement**: CEFX-06, CEFX-08, CEFX-12

**Como fazer**:

- `public abstract class CardEffect : MonoBehaviour`, no namespace `Assets._Project.Code.Runtime.Cards`.
- Propriedades `protected GameObject Player { get; private set; }`, `protected CardData Card { get; private set; }` e `protected int Copies { get; private set; }`.
- `public void Attach(GameObject player, CardData card)`: guarda os dois, pega o `Health` do player e se inscreve no `Died`.
- `public void SetCopies(int copies)`: guarda em `Copies` e chama `OnCopiesChanged(copies)`.
- `protected virtual void OnCopiesChanged(int copies) { }` e `protected virtual void OnPlayerDied() { }`, vazios na base.
- No `Died` do player: `enabled = false;` e depois `OnPlayerDied()`.
- No `OnDestroy`, sair do `Died` (o `Health` pode viver mais que o efeito).
- Comentário curto no topo com as regras do design para quem escreve uma mecânica: eventos no `OnEnable`/`OnDisable`, só tempo escalado, componentes pelo `Player`.

**Done when**:

- [ ] Um script de teste que herda de `CardEffect` compila sem implementar nada
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): adiciona a base das mecânicas de carta`

---

### T4: Componente `CardEffects` (usuário)

**What**: Criar o efeito na primeira cópia, avisar nas seguintes e avisar quando o prefab não tem `CardEffect`.
**Where**: `Assets/_Project/Code/Runtime/Cards/Effects/CardEffects.cs`
**Depends on**: T3
**Reuses**: `CardInventory.CardAdded`; padrão do `PlayerRunStats` (componente no player que lê o `CardInventory` do mesmo objeto)
**Requirement**: CEFX-02, CEFX-03, CEFX-05, CEFX-10

**Como fazer**:

- `[RequireComponent(typeof(CardInventory))] public class CardEffects : MonoBehaviour`.
- `private readonly Dictionary<CardData, CardEffect> effects = new();`.
- `Awake`: pega o `CardInventory`. `OnEnable`/`OnDisable`: entra e sai do `inventory.CardAdded`.
- `OnCardAdded(CardData card, int copies)`:
  1. `card.EffectPrefab == null` → `return` (carta só de números, CEFX-05).
  2. `effects.TryGetValue(card, out var effect)` → `effect.SetCopies(copies); return;` (CEFX-03).
  3. `var instance = Instantiate(card.EffectPrefab, transform);` e `instance.GetComponent<CardEffect>()`.
  4. Sem `CardEffect`: `Debug.LogWarning($"CardEffects: o prefab de efeito da carta '{card.Id}' não tem CardEffect; a carta vale só pelos números.", this);`, `Destroy(instance)` e `return` (CEFX-10).
  5. Com ele: `effect.Attach(gameObject, card); effect.SetCopies(copies); effects[card] = effect;` (CEFX-02).
- Não ouvir o `GameEvents.CardChosen` direto: a ordem dos ouvintes do evento estático não é garantida (design, Risks).

**Done when**:

- [ ] O componente compila e não depende de nada além do `CardInventory`
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): cria e atualiza os efeitos das cartas no player`

---

### T5: `CardEffects` no `Player.prefab` (Claude)

**What**: Pôr o componente no prefab do player e criar a pasta dos prefabs de efeito.
**Where**: `Assets/_Project/Prefabs/Player/Player.prefab`, `Assets/_Project/Prefabs/Cards/Effects/`
**Depends on**: T4
**Reuses**: `Player.prefab`
**Requirement**: CEFX-02, CEFX-11

**Done when**:

- [ ] O player da cena `FirstOne` tem `CardInventory` e `CardEffects`
- [ ] Uma run com as 20 cartas atuais dá os mesmos status de antes
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(player): liga os efeitos de carta no player`

---

### T6: Primeira carta de efeito real (usuário e Claude)

**What**: Criar a primeira carta com mecânica própria, que prova o sistema. A mecânica é definida pelo usuário quando a carta for desenhada.
**Where**: `Assets/_Project/Code/Runtime/Cards/Effects/<Mecânica>Effect.cs` (usuário); prefab em `Assets/_Project/Prefabs/Cards/Effects/`, asset da carta em `Assets/_Project/Data/Cards/` e a carta na `CardCollection` (Claude)
**Depends on**: T5
**Reuses**: `CardEffect`; os eventos do `GameEvents`
**Requirement**: CEFX-04, CEFX-06, CEFX-07, CEFX-09

**Bloqueada**: até o usuário definir a mecânica da carta, inclusive o que acontece com a segunda cópia.

**Done when**:

- [ ] O diff da carta só tem arquivos novos, mais a carta na `CardCollection`
- [ ] A carta aparece na mesa de tarot e no level up
- [ ] Console sem erros de compilação

**Tests**: none
**Gate**: build

**Commit**: `feat(cards): adiciona a primeira carta com mecânica própria`

---

## Checks

**C1** - O campo do prefab de efeito existe e está vazio nas cartas atuais (CEFX-01, AC 1)
Proof: walk - Ler o `EffectPrefab` das 20 cartas da `CardCollection` e ver nulo em todas, menos na carta da T6.

**C2** - A primeira escolha de uma carta de efeito cria uma instância no player com 1 cópia (CEFX-02, AC 2)
Proof: walk - Escolher a carta da T6 no level up e ver um filho com o `CardEffect` no player e `Copies` 1.

**C3** - A segunda escolha avisa o efeito com 2 cópias, sem criar outra instância pelo sistema (CEFX-03, AC 3)
Proof: walk - Escolher a carta da T6 de novo e ver um único `CardEffect` dela no player, com `Copies` 2, e o comportamento que a carta define para a segunda cópia.

**C4** - Carta com bônus e efeito aplica os dois (CEFX-04, AC 4)
Proof: walk - Com a carta da T6 tendo um bônus de número, ver o status no `PlayerRunStats` subir e a mecânica agir.

**C5** - Carta sem efeito aplica só os bônus, como antes (CEFX-05, AC 5)
Proof: walk - Escolher uma carta só de números e ver o status subir e nenhum filho novo no player.

**C6** - Com o jogo pausado, os efeitos não agem (CEFX-06, AC 6)
Proof: walk - Com o level up aberto, ver a mecânica da carta da T6 parada, e voltar a agir ao fechar.

**C7** - Uma mecânica nova entra só com arquivos novos (CEFX-07, AC 1)
Proof: walk - No diff da T6, ver só arquivos novos, mais a carta na `CardCollection`, sem mudança em `CardData`, `CardInventory`, `PlayerRunStats` ou no level up.

**C8** - O efeito tem acesso ao player em que foi criado (CEFX-08, AC 2)
Proof: walk - Ver a mecânica da carta da T6 agir a partir da posição ou dos componentes do player.

**C9** - Cartas de efeito entram na coleção, no deck e no level up (CEFX-09, AC 3)
Proof: walk - Ver a carta da T6 na Coleção, pô-la no deck e vê-la como opção no level up.

**C10** - Prefab sem `CardEffect` dá aviso com o Id e a carta vale só pelos números (CEFX-10, edge case)
Proof: walk - Com um prefab sem `CardEffect` na carta, escolhê-la e ver o aviso com o Id no Console, o status subir e nenhum efeito no player.

**C11** - A run seguinte começa sem efeito (CEFX-11, edge case)
Proof: walk - Terminar a run com a carta da T6, voltar ao hub, partir de novo e ver nenhum `CardEffect` no player.

**C12** - Na morte do player, os efeitos param (CEFX-12, edge case)
Proof: walk - Matar o player com a carta da T6 ativa e ver o `CardEffect` desligado e a mecânica parada.

---

## Handoff

The feature's live state, written as it happens - a compaction gives no warning.

**Settled mid-build:** 2026-11-06 - o Claude faz T1 a T5 (usuário: "faça para mim."); execução inline, um lote só.
**Dispatched:** none
**Walked:** none

---

## Phase Execution Map

```
Phase 1 → Phase 2

Phase 1:  T1 ------→ T2 ------→ T3 ------→ T4 ------→ T5
Phase 2:  T6 (bloqueada até a mecânica da carta ser definida)
```

Execution is strictly sequential. A feature tem 6 tasks e cabe num lote só.
