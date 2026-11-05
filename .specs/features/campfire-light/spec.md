# Fogueira e óleo Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."

## Problem Statement

A visão é o diferencial do jogo, mas hoje o raio da luz é um número fixo do `PlayerStats.asset` (5,0) que nada no jogo muda de forma permanente. O One-Pager define a fogueira do hub como o lugar onde o jogador gasta o óleo dos bosses para subir a luz. Ela ainda não existe.

## Goals

- [ ] Clicar na fogueira abre o menu de luz, que compra níveis com óleo.
- [ ] O nível de luz define o raio base da run: 3,0 no nível 0, +0,5 por nível, até o nível 10 (8,0).

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Penalidade de luz por dificuldade | Dificuldades ficam depois do MVP. |
| Luz que diminui durante a run | O One-Pager mantém a visão fixa no MVP. |
| Fogueira animada nova | O MVP usa o grupo de velas atual como fogueira. |
| Orbes de óleo no escuro do mapa | Saíram do jogo: o óleo vem só dos bosses. Usuário, em 2026-11-04: "Acredito que isso possa ser removido do gdd. nao vamos spawnar o oleo no mapa. será apenas de bosses." |
| Valores finais de custo e raio | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude monta o menu, a fogueira clicável e o asset de configuração | AD-001 | y |
| Fogueira no MVP | Sim | AD-005 | y |
| Fonte do óleo | Só os bosses (30, 30 e 50 por vitória, 110 no total) | Usuário, em 2026-11-04: "Acredito que isso possa ser removido do gdd. nao vamos spawnar o oleo no mapa. será apenas de bosses." | y |
| Raio por nível | 3,0 + 0,5 x nível, nível 0 a 10 | Balanceamento inicial do One-Pager | y |
| Custo do nível n | 40 + 20 x (n - 1) de óleo | Balanceamento inicial do One-Pager | y |
| Raio do `PlayerStats` | O raio da fogueira substitui o `LightRadius` do `PlayerStats` como base da run | O One-Pager diz que a luz sobe só na fogueira | y |
| Cor do custo sem óleo | Cor de aviso da paleta (terracota #C96D45), igual ao padre (PRST-24) | Revisão de 2026-11-04 depois do priest-stats. Usuário, ao aprovar os ajustes da revisão de 2026-11-04: "ok. pode atualizar o design da spec da fogueira". | y |
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

## Edge Cases

- WHILE o nível de luz for 10 the menu SHALL mostrar "Luz no máximo" e desativar a compra. [LIGHT-16]
- IF o jogador não tiver óleo para o próximo nível THEN o menu SHALL desativar a compra e mostrar o custo na cor de aviso da paleta (terracota #C96D45), como o padre. [LIGHT-17]

Dimensões implícitas: validação (LIGHT-16, LIGHT-17) está coberta. Persistência via `save-system` (SAVE-03, SAVE-13); a perda de metade do óleo na morte é do `run-rewards` (RWD-10). Concorrência, autenticação e dependências externas N/A.

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
| LIGHT-16 | Edge Cases | - | Pending |
| LIGHT-17 | Edge Cases | - | Pending |

**Coverage:** 11 total, 0 mapped to tasks, 11 unmapped (antes do Design); LIGHT-10 a LIGHT-15, LIGHT-18 e LIGHT-19 saíram com os orbes

---

## Sources

- `Docs/Last Wick - One-Pager.md` - fogueira, custo `40 + 20 x (n - 1)`, raio 3,0 + 0,5 por nível até 8,0, óleo só dos bosses (os orbes de óleo saíram em 2026-11-04).
- `.specs/STATE.md` - AD-001 e AD-005.
- `Assets/_Project/Code/Runtime/Light/PlayerLight.cs` e `Player/PlayerRunStats.cs` - como o raio é aplicado hoje.
- `Assets/_Project/Data/Progression/PlayerStats.asset` - `LightRadius` atual de 5,0.
- `.specs/features/run-rewards/spec.md` - RWD-04 e RWD-10.

---

## Success Criteria

- [ ] Uma vitória rende 110 de óleo, que compra os dois primeiros níveis de luz.
- [ ] O nível de luz comprado aparece no raio da run seguinte.
