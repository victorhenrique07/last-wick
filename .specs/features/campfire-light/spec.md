# Fogueira e óleo Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."

## Problem Statement

A visão é o diferencial do jogo, mas hoje o raio da luz é um número fixo do `PlayerStats.asset` (5,0) que nada no jogo muda de forma permanente. O One-Pager define a fogueira do hub como o lugar onde o jogador gasta óleo para subir a luz, e os orbes de óleo escondidos no escuro como a recompensa por explorar em vez de ficar parado. Nenhum dos dois existe.

## Goals

- [ ] Clicar na fogueira abre o menu de luz, que compra níveis com óleo.
- [ ] O nível de luz define o raio base da run: 3,0 no nível 0, +0,5 por nível, até o nível 10 (8,0).
- [ ] Cada run espalha orbes de óleo pelo escuro, que o jogador vê de longe e coleta ao encostar.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Penalidade de luz por dificuldade | Dificuldades ficam depois do MVP. |
| Luz que diminui durante a run | O One-Pager mantém a visão fixa no MVP. |
| Indicador de orbe fora da tela | O One-Pager só pede o ponto visível na tela. |
| Fogueira animada nova | O MVP usa o grupo de velas atual como fogueira. |
| Valores finais de custo, raio e orbes | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude monta o menu, a fogueira clicável e a arte do orbe | AD-001 | y |
| Fogueira no MVP | Sim | AD-005 | y |
| Raio por nível | 3,0 + 0,5 x nível, nível 0 a 10 | Balanceamento inicial do One-Pager | y |
| Custo do nível n | 40 + 20 x (n - 1) de óleo | Balanceamento inicial do One-Pager | y |
| Orbes por run | 8 orbes de 5 de óleo | Balanceamento inicial do One-Pager | y |
| Raio do `PlayerStats` | O raio da fogueira substitui o `LightRadius` do `PlayerStats` como base da run | O One-Pager diz que a luz sobe só na fogueira | y |
| Onde os orbes nascem | Pontos sorteados dentro de uma área retangular do mapa, a pelo menos 6 unidades do início do player e 4 unidades uns dos outros | "Escondidos no escuro": longe da luz inicial e espalhados | y |
| Como o orbe é coletado | Ao encostar no player, como o XP | Mesma regra de coleta do One-Pager | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário na feature anterior | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Comprar luz na fogueira ⭐ MVP

**User Story**: Como jogador, quero gastar óleo na fogueira para enxergar mais longe em todas as runs.

**Why P1**: É o diferencial do jogo e o único uso do óleo.

**Acceptance Criteria**:

1. WHEN o mouse passar sobre a fogueira THEN o hub SHALL mostrar o contorno dourado dos objetos clicáveis. [LIGHT-01]
2. WHEN o jogador clicar na fogueira THEN o hub SHALL abrir o menu da fogueira. [LIGHT-02]
3. The menu da fogueira SHALL mostrar o nível de luz atual, o raio atual, o raio do próximo nível e o custo em óleo do próximo nível. [LIGHT-03]
4. The custo do nível n SHALL ser 40 + 20 x (n - 1) de óleo. [LIGHT-04]
5. WHEN o jogador comprar o próximo nível com óleo suficiente THEN a fogueira SHALL tirar o custo do óleo do save. [LIGHT-05]
6. WHEN o jogador comprar o próximo nível com óleo suficiente THEN a fogueira SHALL subir em 1 o nível de luz do save. [LIGHT-06]
7. WHEN o jogador clicar em "Fechar" ou apertar Esc THEN o hub SHALL fechar o menu da fogueira. [LIGHT-07]

**Independent Test**: Com 100 de óleo, comprar o nível 1 (40) e o nível 2 (60) e ver o óleo em 0, o nível em 2 e o próximo custo em 80.

---

### P1: A luz da run vem da fogueira ⭐ MVP

**User Story**: Como jogador, quero que o nível da fogueira defina meu raio de visão na run.

**Why P1**: Sem isso a compra não tem efeito.

**Acceptance Criteria**:

1. WHEN uma run começar THEN o player SHALL usar como raio base de luz 3,0 + 0,5 x o nível de luz do save. [LIGHT-08]
2. The bônus de luz das cartas SHALL ser aplicado sobre o raio base da fogueira. [LIGHT-09]

**Independent Test**: Com nível de luz 4, a run começa com raio 5,0 sem cartas.

---

### P1: Orbes de óleo no escuro ⭐ MVP

**User Story**: Como jogador, quero encontrar orbes de óleo no escuro, para ganhar óleo explorando em vez de só matar hordas.

**Why P1**: É a fonte de óleo fora dos bosses e o incentivo para explorar.

**Acceptance Criteria**:

1. WHEN uma run começar THEN a run SHALL criar 8 orbes de óleo em posições sorteadas dentro da área do mapa. [LIGHT-10]
2. The orbe de óleo SHALL aparecer como um ponto de luz fraco e pulsante, visível em qualquer ponto da tela mesmo fora da luz do player. [LIGHT-11]
3. The orbe de óleo SHALL não iluminar o chão nem os inimigos em volta. [LIGHT-12]
4. WHEN o player encostar num orbe THEN a run SHALL somar 5 de óleo ao óleo coletado e remover o orbe. [LIGHT-13]
5. The orbes SHALL nascer a pelo menos 6 unidades da posição inicial do player e a pelo menos 4 unidades uns dos outros. [LIGHT-14]
6. The quantidade de orbes, o óleo de cada um e as distâncias mínimas SHALL ficar num asset de dados editável no Inspector. [LIGHT-15]

**Independent Test**: Começar uma run com luz mínima, ver pontos pulsando no escuro, andar até um e ver o óleo coletado subir 5 na tela de resultado.

---

## Edge Cases

- WHILE o nível de luz for 10 the menu SHALL mostrar "Luz no máximo" e desativar a compra. [LIGHT-16]
- IF o jogador não tiver óleo para o próximo nível THEN o menu SHALL desativar a compra e mostrar o custo em vermelho. [LIGHT-17]
- IF a área do mapa não couber todos os orbes com as distâncias mínimas THEN a run SHALL criar os que couberem e registrar um aviso no Console. [LIGHT-18]
- WHEN o player encostar num orbe depois do fim da run THEN a run SHALL não somar o óleo. [LIGHT-19]

Dimensões implícitas: validação (LIGHT-16, LIGHT-17, LIGHT-18) e transições de estado (LIGHT-19) estão cobertas. Persistência via `save-system` (SAVE-03, SAVE-13); a perda de metade do óleo na morte é do `run-rewards` (RWD-10). Concorrência, autenticação e dependências externas N/A.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| LIGHT-01 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-02 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-03 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-04 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-05 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-06 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-07 | P1: Comprar luz na fogueira | - | Pending |
| LIGHT-08 | P1: A luz da run vem da fogueira | - | Pending |
| LIGHT-09 | P1: A luz da run vem da fogueira | - | Pending |
| LIGHT-10 | P1: Orbes de óleo no escuro | - | Pending |
| LIGHT-11 | P1: Orbes de óleo no escuro | - | Pending |
| LIGHT-12 | P1: Orbes de óleo no escuro | - | Pending |
| LIGHT-13 | P1: Orbes de óleo no escuro | - | Pending |
| LIGHT-14 | P1: Orbes de óleo no escuro | - | Pending |
| LIGHT-15 | P1: Orbes de óleo no escuro | - | Pending |
| LIGHT-16 | Edge Cases | - | Pending |
| LIGHT-17 | Edge Cases | - | Pending |
| LIGHT-18 | Edge Cases | - | Pending |
| LIGHT-19 | Edge Cases | - | Pending |

**Coverage:** 19 total, 0 mapped to tasks, 19 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - fogueira, custo `40 + 20 x (n - 1)`, raio 3,0 + 0,5 por nível até 8,0, 8 orbes de 5, orbe como ponto de luz fraco e pulsante que não ilumina em volta.
- `.specs/STATE.md` - AD-001 e AD-005.
- `Assets/_Project/Code/Runtime/Light/PlayerLight.cs` e `Player/PlayerRunStats.cs` - como o raio é aplicado hoje.
- `Assets/_Project/Data/Progression/PlayerStats.asset` - `LightRadius` atual de 5,0.
- `.specs/features/run-rewards/spec.md` - RWD-06 e RWD-10.

---

## Success Criteria

- [ ] Uma vitória com todos os orbes rende cerca de 150 de óleo (110 dos bosses e 40 dos orbes).
- [ ] O nível de luz comprado aparece no raio da run seguinte.
