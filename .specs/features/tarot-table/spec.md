# Mesa de tarot: cartas e deck Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."
**Revisão**: 2026-11-06 - a aba Desbloquear sai; o desbloqueio passa para a Coleção, com nome e descrição em cada carta. Aguardando aprovação.

## Problem Statement

Hoje as opções do level up saem de um `Deck.asset` fixo, montado no Editor, e o jogador não escolhe nada antes da run nem pode liberar cartas novas. O One-Pager define o deck de 10 a 15 cartas montado no hub como um dos três pilares ("O build começa antes da partida"), e o mago como quem desbloqueia cartas com materiais. A mesa de tarot do hub já é clicável, mas não abre nada.

## Goals

- [ ] A mesa de tarot abre um menu com três abas: Desbloquear, Coleção e Deck.
- [ ] O jogador libera cartas com materiais, lê todas as cartas e monta o deck de 10 a 15 cartas.
- [ ] As opções do level up na run saem do deck montado pelo jogador.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Descrição das cartas na aba Deck | Usuário, em 2026-11-06, escolheu "Não, só o nome": quem quiser ler vai à Coleção. |
| Linhas de efeito geradas dos números | Usuário, em 2026-11-06, escolheu "Descrição do asset": a carta mostra o texto escrito no asset. |
| Aba só de desbloqueio | Usuário, em 2026-11-06: "não devemos ter uma aba apenas para desbloqueio. Teremos a aba de coleção, onde cada carta terá sua descrição e nome. EMbaixo disso ficará o botão "Desbloquear" junto do custo dela." |
| Cartas repetidas no deck | O One-Pager proíbe repetição no deck. |
| Vários decks salvos | O MVP guarda um deck só. |
| Raridades de carta | Fora do MVP no One-Pager. |
| Arte própria de cada carta | O campo `Icon` já existe; sem arte, a carta usa o ícone padrão. |
| Nível máximo de carta | O One-Pager não define teto. |
| NPC animado do mago | O MVP usa a mesa de tarot como símbolo. |
| Valores finais de custo | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude monta o menu e as abas | AD-001 | y |
| O que a mesa faz | Desbloquear, ler, coleção e montar o deck | AD-004 | y |
| Cartas padrão | As 15 cartas do `Deck.asset` atual começam liberadas; as outras 5 começam bloqueadas | É o deck que a run já usa; o One-Pager deixa a lista em aberto | y |
| Lista de todas as cartas | O `CardCollection` existente vira a lista oficial de cartas do jogo | Já existe e não é usado | y |
| Custo de desbloqueio | Cada carta tem seu custo em osso, ferro e cera; provisório 15 materiais por carta | Materiais são do mago no One-Pager | y |
| Carta bloqueada na Coleção | Mostra nome, descrição, efeitos e custo, com a marca "Bloqueada" | Ajuda o jogador a planejar o que liberar | y |
| Deck inicial | As 15 cartas padrão | Deck válido desde a primeira run | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário na feature anterior | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Desbloquear cartas ⭐ MVP

**User Story**: Como jogador, quero gastar materiais na mesa de tarot para liberar cartas novas, para ter mais opções de build.

**Why P1**: É o uso dos materiais junto com o ferreiro e o que amplia o deck.

**Acceptance Criteria**:

1. WHEN o jogador clicar na mesa de tarot THEN o hub SHALL abrir o menu da mesa na aba Coleção. [TAROT-01]
2. WHILE uma carta estiver bloqueada the aba Coleção SHALL mostrar, embaixo da descrição dela, o botão "Desbloquear" e o custo em osso, ferro e cera. [TAROT-02]
3. WHEN o jogador clicar em "Desbloquear" numa carta com materiais suficientes THEN a mesa SHALL tirar o custo dos materiais do save. [TAROT-03]
4. WHEN o jogador clicar em "Desbloquear" numa carta com materiais suficientes THEN a mesa SHALL acrescentar o ID da carta às cartas liberadas do save. [TAROT-04]
5. WHEN uma carta for desbloqueada THEN a carta SHALL deixar de mostrar o botão "Desbloquear" e o custo. [TAROT-05]
6. WHEN o jogador clicar em "Fechar" ou apertar Esc THEN o hub SHALL fechar o menu da mesa. [TAROT-06]

**Independent Test**: Com os materiais de uma carta bloqueada, clicar em "Desbloquear" embaixo dela na Coleção, ver os materiais caírem, o botão e o custo sumirem, e a carta entrar na aba Deck como disponível.

---

### P1: Ler a coleção ⭐ MVP

**User Story**: Como jogador, quero ver todas as cartas do jogo e ler o que cada uma faz, para planejar meu deck e meus desbloqueios.

**Why P1**: Sem ler as cartas, montar o deck é chute.

**Acceptance Criteria**:

1. The aba Coleção SHALL listar todas as cartas do `CardCollection`. [TAROT-07]
2. The aba Coleção SHALL mostrar em cada carta o nome, o ícone e a descrição escrita no asset da carta. [TAROT-08]
3. The menu da mesa SHALL ter só as abas Coleção e Deck. [TAROT-09]
4. The aba Coleção SHALL mostrar quantas cartas estão liberadas do total, no formato "Liberadas: N/M". [TAROT-10]

**Independent Test**: Abrir a Coleção, ver as 20 cartas com nome e descrição, e as bloqueadas com "Desbloquear" e o custo embaixo.

---

### P1: Montar o deck ⭐ MVP

**User Story**: Como jogador, quero escolher de 10 a 15 cartas antes da run, para que o level up me ofereça o build que planejei.

**Why P1**: Pilar "O build começa antes da partida".

**Acceptance Criteria**:

1. The aba Deck SHALL listar as cartas liberadas, separando as que estão no deck das que estão fora. [TAROT-11]
2. WHEN o jogador clicar numa carta fora do deck com menos de 15 cartas no deck THEN a mesa SHALL colocá-la no deck. [TAROT-12]
3. WHEN o jogador clicar numa carta do deck THEN a mesa SHALL tirá-la do deck. [TAROT-13]
4. The aba Deck SHALL mostrar o contador "Deck: N/15". [TAROT-14]
5. WHEN o deck mudar THEN a mesa SHALL gravar a lista de IDs do deck no save. [TAROT-15]
6. WHEN o jogador subir de nível na run THEN o level up SHALL sortear as 3 opções entre as cartas do deck salvo. [TAROT-16]

**Independent Test**: Tirar 3 cartas do deck, partir para a run e ver que elas nunca aparecem no level up.

---

## Edge Cases

- IF o jogador não tiver os materiais de uma carta THEN a aba Coleção SHALL desativar o botão "Desbloquear" dela e mostrar na cor de aviso (#C96D45) o material que falta. [TAROT-17]
- IF o deck tiver 15 cartas THEN a aba Deck SHALL desativar a entrada de novas cartas. [TAROT-18]
- WHILE o deck tiver menos de 10 cartas the aba Deck SHALL mostrar "O deck precisa de pelo menos 10 cartas." [TAROT-19]
- The deck SHALL não conter a mesma carta duas vezes. [TAROT-20]

Dimensões implícitas: validação (TAROT-17, TAROT-18, TAROT-19, TAROT-20) estão cobertas. Persistência via `save-system` (SAVE-04, SAVE-05, SAVE-13, SAVE-14). Concorrência, autenticação e dependências externas N/A.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| TAROT-01 | P1: Desbloquear cartas | - | Pending |
| TAROT-02 | P1: Desbloquear cartas | - | Pending |
| TAROT-03 | P1: Desbloquear cartas | - | Pending |
| TAROT-04 | P1: Desbloquear cartas | - | Pending |
| TAROT-05 | P1: Desbloquear cartas | - | Pending |
| TAROT-06 | P1: Desbloquear cartas | - | Pending |
| TAROT-07 | P1: Ler a coleção | - | Pending |
| TAROT-08 | P1: Ler a coleção | - | Pending |
| TAROT-09 | P1: Ler a coleção | - | Pending |
| TAROT-10 | P1: Ler a coleção | - | Pending |
| TAROT-11 | P1: Montar o deck | - | Pending |
| TAROT-12 | P1: Montar o deck | - | Pending |
| TAROT-13 | P1: Montar o deck | - | Pending |
| TAROT-14 | P1: Montar o deck | - | Pending |
| TAROT-15 | P1: Montar o deck | - | Pending |
| TAROT-16 | P1: Montar o deck | - | Pending |
| TAROT-17 | Edge Cases | - | Pending |
| TAROT-18 | Edge Cases | - | Pending |
| TAROT-19 | Edge Cases | - | Pending |
| TAROT-20 | Edge Cases | - | Pending |
| TAROT-21 | Removido na revisão de 2026-11-06 (sem aba Desbloquear) | - | Removed |
| TAROT-22 | Removido na revisão de 2026-11-06 (a carta mostra a descrição do asset) | - | Removed |

**Coverage:** 22 total, 0 mapped to tasks, 22 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - deck de 10 a 15 cartas sem repetição, montado no hub com cartas padrão e desbloqueadas; mago desbloqueia cartas com materiais, sem teto.
- `.specs/STATE.md` - AD-001, AD-004 e AD-007.
- `Assets/_Project/Code/Runtime/Cards/` - `CardData`, `CardCollection`, `DeckData`, `CardInventory`.
- `Assets/_Project/Code/Runtime/UI/LevelUpPanel.cs` - hoje sorteia do `Deck.asset`.
- `Assets/_Project/Data/Cards/` - 20 cartas, 15 delas no `Deck.asset`.
- `.specs/features/hub-flow/spec.md` - HUB-13, o portão lê se o deck é válido.

---

## Success Criteria

- [ ] Uma carta desbloqueada e colocada no deck aparece como opção de level up na run seguinte.
- [ ] Nenhuma run começa com deck fora de 10 a 15 cartas.
