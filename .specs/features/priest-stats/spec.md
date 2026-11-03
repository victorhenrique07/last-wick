# Padre: status permanentes Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."

## Problem Statement

O ouro dos bosses não tem uso, e o player começa toda run com os mesmos status do `PlayerStats.asset`. O One-Pager define o padre como quem sobe os status base permanentes, um ponto por vez, pagando só com ouro. A igreja do hub já é clicável, mas não abre nada.

## Goals

- [ ] Clicar na igreja abre o menu do padre com os status que podem subir.
- [ ] Cada ponto comprado custa ouro, fica no save e vale em todas as runs seguintes.
- [ ] Adicionar um novo status ao padre não pede mudança de código.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Teto de nível por mapa e dificuldade | O One-Pager tira o teto do MVP. |
| Devolver pontos (respec) | Não está no One-Pager. |
| Status de defesa | O jogo não tem defesa hoje; vira status quando a mecânica existir. |
| Luz | Sobe na fogueira (feature `fogueira-light`), nunca no padre. |
| NPC animado do padre | O MVP usa a igreja como símbolo. |
| Valores finais de custo e de ganho | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude monta o menu | AD-001 | y |
| Status disponíveis | Vida máxima, Dano, Velocidade, Recarga da esquiva e Coleta de XP | São status que já existem no `PlayerStats` e que o jogador sente na run | y |
| Ganho por ponto (provisório) | Vida +10, Dano +1, Velocidade +0,05, Recarga da esquiva -3%, Coleta de XP +0,1 | Passos pequenos para muitos níveis | y |
| Custo por ponto (provisório) | 5 de ouro no primeiro ponto, +5 a cada ponto seguinte do mesmo status | Uma vitória (70 de ouro) compra alguns pontos no começo e poucos depois | y |
| Teto | Nenhum no MVP | Decidido no One-Pager | y |
| Onde ficam os números | Um asset por status, editável no Inspector | Adicionar status e balancear sem código | y |
| Ordem de aplicação | Padre soma ao `PlayerStats`; as cartas multiplicam por cima | Mantém a regra atual das cartas sobre a base | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário na feature anterior | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Comprar pontos de status ⭐ MVP

**User Story**: Como jogador, quero gastar ouro com o padre para subir meus status base, para ficar mais forte a cada run.

**Why P1**: É o único uso do ouro e um dos dois níveis de progressão do One-Pager.

**Acceptance Criteria**:

1. WHEN o jogador clicar na igreja THEN o hub SHALL abrir o menu do padre. [PRST-01]
2. The menu do padre SHALL listar cada status com nome, nível atual, valor atual, valor com o próximo ponto e custo do próximo ponto. [PRST-02]
3. WHEN o jogador comprar um ponto com ouro suficiente THEN o padre SHALL tirar o custo do ouro do save. [PRST-03]
4. WHEN o jogador comprar um ponto com ouro suficiente THEN o padre SHALL subir em 1 o nível desse status no save. [PRST-04]
5. WHEN o jogador comprar um ponto THEN o menu SHALL mostrar o novo nível, o novo valor e o novo custo. [PRST-05]
6. The custo de um ponto SHALL ser 5 + 5 x nível atual do status, em ouro. [PRST-06]
7. WHEN o jogador clicar em "Fechar" ou apertar Esc THEN o hub SHALL fechar o menu do padre. [PRST-07]

**Independent Test**: Com 20 de ouro, comprar Vida duas vezes (5 e 10) e ver o ouro em 5, o nível de Vida em 2 e o próximo custo em 15.

---

### P1: Pontos valem na run ⭐ MVP

**User Story**: Como jogador, quero que os pontos comprados apareçam na run, para sentir o progresso.

**Why P1**: Sem isso a compra não tem efeito.

**Acceptance Criteria**:

1. WHEN uma run começar THEN o player SHALL somar ao valor do `PlayerStats` o ganho por ponto vezes o nível de cada status do padre. [PRST-08]
2. The bônus das cartas SHALL ser aplicado sobre o valor que já inclui os pontos do padre. [PRST-09]
3. The tela de resultado SHALL mostrar os status finais com os pontos do padre incluídos. [PRST-10]

**Independent Test**: Comprar 3 pontos de Vida e começar uma run com 130 de vida máxima, partindo de 100.

---

### P1: Status do padre como dados ⭐ MVP

**User Story**: Como desenvolvedor, quero definir cada status do padre num asset, para adicionar ou ajustar status sem mexer no código.

**Why P1**: Os números são provisórios e vão mudar no balanceamento.

**Acceptance Criteria**:

1. The cada status do padre SHALL ser um asset com ID, nome, ícone, ganho por ponto e custo. [PRST-11]
2. WHEN um novo asset de status for adicionado à lista do padre THEN o menu SHALL mostrá-lo sem mudança de código. [PRST-12]

**Independent Test**: Criar um asset de status de teste, colocá-lo na lista e vê-lo no menu.

---

## Edge Cases

- IF o jogador não tiver ouro para o próximo ponto THEN o menu SHALL desativar o botão de compra desse status. [PRST-13]
- IF o jogador não tiver ouro para o próximo ponto THEN o menu SHALL mostrar o custo em vermelho. [PRST-14]
- WHEN o jogador clicar várias vezes seguidas em comprar THEN o padre SHALL cobrar e subir um ponto por clique, sem deixar o ouro negativo. [PRST-15]
- IF a Recarga da esquiva com os pontos ficar abaixo de 0,2 s THEN o player SHALL usar 0,2 s. [PRST-16]

Dimensões implícitas: validação (PRST-13, PRST-16), repetição (PRST-15) e persistência (via `save-system`, SAVE-13) estão cobertas. Concorrência, autenticação, dependências externas e ciclo de vida N/A: compra local, um clique por vez.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| PRST-01 | P1: Comprar pontos de status | - | Pending |
| PRST-02 | P1: Comprar pontos de status | - | Pending |
| PRST-03 | P1: Comprar pontos de status | - | Pending |
| PRST-04 | P1: Comprar pontos de status | - | Pending |
| PRST-05 | P1: Comprar pontos de status | - | Pending |
| PRST-06 | P1: Comprar pontos de status | - | Pending |
| PRST-07 | P1: Comprar pontos de status | - | Pending |
| PRST-08 | P1: Pontos valem na run | - | Pending |
| PRST-09 | P1: Pontos valem na run | - | Pending |
| PRST-10 | P1: Pontos valem na run | - | Pending |
| PRST-11 | P1: Status do padre como dados | - | Pending |
| PRST-12 | P1: Status do padre como dados | - | Pending |
| PRST-13 | Edge Cases | - | Pending |
| PRST-14 | Edge Cases | - | Pending |
| PRST-15 | Edge Cases | - | Pending |
| PRST-16 | Edge Cases | - | Pending |

**Coverage:** 16 total, 0 mapped to tasks, 16 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - padre sobe status base um ponto por vez, só com ouro, sem teto no MVP; luz sobe à parte, na fogueira.
- `.specs/STATE.md` - AD-001 e AD-007.
- `Assets/_Project/Code/Runtime/Player/PlayerStats.cs` e `PlayerRunStats.cs` - status base atuais e como as cartas se aplicam.
- `.specs/features/save-system/spec.md` - SAVE-02 e SAVE-13.

---

## Success Criteria

- [ ] O ouro de uma vitória compra pelo menos 4 pontos no começo do jogo.
- [ ] Um ponto comprado aparece no valor da run seguinte e na tela de resultado.
