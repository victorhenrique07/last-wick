# Cartas com mecânica própria Specification

## Problem Statement

Hoje toda carta é uma soma de números fixos em `CardData` (vida, dano, cadência, luz...), aplicada pelo `PlayerRunStats`. Uma carta com comportamento próprio, como "a cada 5 abates, uma explosão", não tem onde morar: cada mecânica nova exigiria mexer no `CardData` e no `PlayerRunStats`. O sistema dá a cada carta um lugar opcional para a sua mecânica, sem tocar nas cartas de números que já existem.

## Goals

- [ ] Uma carta nova com mecânica própria é criada só com um asset de carta, um prefab de efeito e o script da mecânica, sem mudar `CardData`, `CardInventory` ou `PlayerRunStats`.
- [ ] Cada carta decide o que acontece quando é pega de novo: mais uma instância da mecânica ou a mesma mecânica mais forte.
- [ ] As cartas de números atuais continuam funcionando sem mudança nenhuma.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Uma carta de efeito pronta no MVP | Usuário, em 2026-11-05: "nenhuma, só quero a spec para criar esse sistema carta com mecânica própria." A primeira carta real vem depois e serve de prova. |
| Mostrar o nível da carta no level up | Usuário escolheu "Não, fica igual às outras": o card mostra nome e descrição, como hoje. |
| Efeitos que passam de uma run para outra | Cartas valem só durante a run (One-Pager). |
| Novos eventos de jogo (esquiva, dano levado...) | Cada carta que precisar de um evento novo o adiciona no `GameEvents` na sua própria tarefa. |
| Valores das mecânicas | Balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude cria assets e prefabs quando pedido | AD-001; usuário escolheu "Eu escrevo o código (AD-001)" | y |
| Quando entra no MVP | Junto da mesa de tarot, depois do fechamento do save e do balanceamento | Usuário escolheu "Junto da mesa de tarot"; AD-015 | y |
| Carta pega de novo | O sistema avisa o efeito do novo número de cópias; a mecânica da carta decide o que fazer (mais uma instância ou mais força) | Usuário: "depende da carta. isso será uma mecânica dela. Terão cartas que terão esse exemplo de duas explosoes, outras cartas vao ter um efeito unico e os niveis serão apenas incrementos de status do efeito que ela gera." | y |
| Carta com números e efeito | Uma carta pode ter bônus de números e efeito ao mesmo tempo; os dois valem | Os bônus continuam no `CardData` e o efeito é um campo a mais; não há razão para escolher um só | n |
| Como provar | Walk em Play Mode com a primeira carta de efeito real; a spec fecha quando ela existir | Usuário escolheu "Provar com a 1ª carta real" | y |

**Open questions:** none - all resolved or logged above (required before the spec is confirmed).

---

## User Stories

### P1: Carta com mecânica própria na run ⭐ MVP

**User Story**: Como jogador, quero que algumas cartas do level up tragam uma mecânica nova (não só números) para o build mudar o jeito de jogar.

**Why P1**: Sem isso, toda carta é uma porcentagem e o build tem pouca variedade.

**Acceptance Criteria** (each line is one EARS pattern):

1. The `CardData` SHALL have an optional effect prefab field that is empty in every existing card asset. <!-- CEFX-01 -->
2. WHEN the player chooses, in the level up, a card with an effect prefab that the player does not have yet THEN the system SHALL create one instance of that effect on the player and tell it that the card has 1 copy. <!-- CEFX-02 -->
3. WHEN the player chooses again a card with an effect prefab that the player already has THEN the system SHALL tell the existing effect the new number of copies of that card, without creating a second instance by itself. <!-- CEFX-03 -->
4. WHERE a card has both stat bonuses and an effect prefab the system SHALL apply the stat bonuses and the effect. <!-- CEFX-04 -->
5. WHEN the player chooses a card without an effect prefab THEN the system SHALL apply only its stat bonuses, exactly as before this feature. <!-- CEFX-05 -->
6. WHILE the game is paused (level up open or result screen open) the effects SHALL not act. <!-- CEFX-06 -->

**Independent Test**: com a primeira carta de efeito real no deck, escolher a carta no level up e ver a mecânica agir; escolher de novo e ver a mudança que a carta define para a segunda cópia.

---

### P1: Mecânica escrita sem mexer no sistema ⭐ MVP

**User Story**: Como desenvolvedor, quero escrever uma mecânica nova só com um script e um prefab para criar cartas novas sem mexer nas cartas que já existem.

**Why P1**: É o motivo da feature: cada carta nova não pode exigir mudança no núcleo das cartas.

**Acceptance Criteria**:

1. The system SHALL let a new card mechanic be added with only a new script, a new effect prefab and a new card asset, without changes to `CardData`, `CardInventory`, `PlayerRunStats` or the level up. <!-- CEFX-07 -->
2. The system SHALL give each effect access to the player it was created on. <!-- CEFX-08 -->
3. The cards with an effect prefab SHALL enter the card collection, the deck and the level up options like any other card. <!-- CEFX-09 -->

**Independent Test**: criar a primeira carta de efeito real e conferir no diff que só há arquivos novos (script, prefab, asset) e a carta na coleção.

---

## Edge Cases

- IF a card's effect prefab has no effect component THEN the system SHALL log a warning with the card's Id, skip the effect and still apply the card's stat bonuses. <!-- CEFX-10 -->
- WHEN the run ends (victory, death or time out) and the player returns to the hub THEN the next run SHALL start with no card effect. <!-- CEFX-11 -->
- WHEN the player dies THEN the effects SHALL stop acting together with the player. <!-- CEFX-12 -->

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| CEFX-01 | P1: Carta com mecânica própria na run | - | Pending |
| CEFX-02 | P1: Carta com mecânica própria na run | - | Pending |
| CEFX-03 | P1: Carta com mecânica própria na run | - | Pending |
| CEFX-04 | P1: Carta com mecânica própria na run | - | Pending |
| CEFX-05 | P1: Carta com mecânica própria na run | - | Pending |
| CEFX-06 | P1: Carta com mecânica própria na run | - | Pending |
| CEFX-07 | P1: Mecânica escrita sem mexer no sistema | - | Pending |
| CEFX-08 | P1: Mecânica escrita sem mexer no sistema | - | Pending |
| CEFX-09 | P1: Mecânica escrita sem mexer no sistema | - | Pending |
| CEFX-10 | Edge Cases | - | Pending |
| CEFX-11 | Edge Cases | - | Pending |
| CEFX-12 | Edge Cases | - | Pending |

**Coverage:** 12 total, 0 mapped to tasks, 12 unmapped ⚠️

---

## Sources

- Conversa de 2026-11-05: o caminho proposto (campo de prefab no `CardData`, componente no player que ouve `GameEvents.CardChosen`) e as respostas do usuário.
- `Docs/Last Wick - One-Pager.md` - cartas valem só durante a run; deck montado na mesa de tarot.
- `.specs/features/tarot-table/spec.md` - coleção, deck e level up das cartas.

---

## Success Criteria

- [ ] A primeira carta de efeito real é criada sem nenhuma linha mudada em `CardData`, `CardInventory`, `PlayerRunStats` ou no level up.
- [ ] As 20 cartas de números atuais dão os mesmos status de antes.
