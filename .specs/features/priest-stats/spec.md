# Padre: atributos permanentes Specification

**Approved**: 2026-11-04 - user: "Aprovo a spec"

## Problem Statement

O ouro dos bosses não tem uso, e o player começa toda run com os mesmos status do `PlayerStats.asset`. O One-Pager define o padre como quem sobe o personagem de forma permanente, um ponto por vez, pagando só com ouro, no estilo dos soulslikes. A igreja do hub já é clicável, mas não abre nada, e o jogo não tem defesa.

## Goals

- [ ] Clicar na igreja abre o menu do padre com os atributos Vitalidade, Força, Agilidade e Resistência.
- [ ] Cada ponto num atributo sobe o nível do personagem, custa ouro pelo nível total, fica no save e vale em todas as runs seguintes.
- [ ] Cada atributo move um ou mais status do player, e o jogo ganha a defesa, que reduz o dano recebido.
- [ ] Criar um atributo, ou fazer um atributo mover outro status que já existe, não pede mudança de código.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Teto de nível por mapa e dificuldade | O One-Pager tira o teto do MVP. |
| Devolver pontos (respec) | Não está no One-Pager; decidido fora do MVP. |
| Soft cap (atributo rendendo menos depois de certo nível) | Decidido: ganho linear no MVP; soft cap entra no balanceamento. |
| Resistências a efeitos (veneno, fogo etc.) | O nome "Resistência" foi escolhido para abrigá-las depois; o MVP só tem a defesa. |
| Defesa na tela de resultado | A grade de status da tela é fixa; a defesa aparece no menu do padre. |
| Luz | Sobe na fogueira (feature `campfire-light`), nunca no padre. |
| NPC animado do padre | O MVP usa a igreja como símbolo. |
| Velocidade do projétil no padre | Sobe só pelas cartas durante a run. Usuário, na T1: "esse campo nao vai existir. Isso será aumentado através das cartas durante a run." |
| Valores finais de custo e de ganho | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código de gameplay; o Claude monta o menu, a arte e os assets | AD-001 | y |
| Modelo de progressão | Atributos no estilo soulslike: cada um move um ou mais status | Usuário: "Será igual Soulslike." | y |
| Atributos | Vitalidade, Força, Agilidade e Resistência | Usuário escolheu os quatro; "não existirá destreza. Apenas agilidade, pois são a mesma coisa." | y |
| Vitalidade | Vida máxima | Rascunho aprovado: "está correto." | y |
| Força | Dano base do personagem, que vale para todas as armas | Usuário: "A força sobe o DANO BASE do PERSONAGEM. Esse dano base vai se refletir em todas as armas." | y |
| Agilidade | Velocidade de movimento, distância da esquiva, menos recarga da esquiva, cadência de tiro e um pouco de raio de coleta de XP; a velocidade do projétil fica só nas cartas | Usuário: Agilidade absorve a Destreza e "aumenta o raio de coleta de XP, mas bem pouco." Usuário, na T1: "esse campo nao vai existir. Isso será aumentado através das cartas durante a run." | y |
| Resistência | Defesa | Usuário pediu um nome que abranja vários tipos para receber novos efeitos depois | y |
| Defesa | Reduz uma porcentagem do dano recebido; os pontos se multiplicam como a absorção dos soulslikes: cada ponto tira 2% do dano que sobrou, defesa = 1 - (1 - 0,02)^nível. Rende um pouco menos a cada ponto e nunca chega a 100%, sem teto | Usuário, na T2, depois de comparar com a defesa dos soulslikes: escolheu "Multiplicando (soulslike)". Substitui o "Percentual com teto" | y |
| Custo | Pelo nível total do personagem: o próximo ponto custa o mesmo em qualquer atributo | Usuário escolheu "Pelo nível total" | y |
| Fórmula do custo (provisória) | 5 + 5 x nível total, em ouro | Mantém a curva da spec anterior; uma vitória (70 de ouro) compra 4 pontos no começo | y |
| Ganho por ponto | Linear, sem soft cap | Usuário escolheu "Linear no MVP" | y |
| Ganhos por ponto (provisórios) | Vitalidade: vida +10. Força: dano base +1. Agilidade: velocidade +0,05, distância da esquiva +0,05, recarga da esquiva -0,04 s, cadência +2%, coleta de XP +0,05. Resistência: 2% do dano que sobrou por ponto | Passos pequenos para muitos níveis; valores ajustados no balanceamento | y |
| Pisos | Recarga da esquiva nunca abaixo de 0,2 s | Regra da spec anterior, mantida | y |
| Onde ficam os números | Um asset por atributo, com uma lista de efeitos (status + ganho por ponto) | Usuário: novos efeitos e atributos "sem mexer no nome dos ups" | y |
| Ordem de aplicação | O padre soma à base do `PlayerStats`; as cartas se aplicam por cima | AD-012 | y |
| Teto | Nenhum no MVP | Decidido no One-Pager | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário nas features anteriores | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Comprar pontos de atributo ⭐ MVP

**User Story**: Como jogador, quero gastar ouro com o padre para subir meus atributos, para ficar mais forte a cada run.

**Why P1**: É o único uso do ouro e um dos dois níveis de progressão do One-Pager.

**Acceptance Criteria**:

1. WHEN o jogador clicar na igreja THEN o hub SHALL abrir o menu do padre. [PRST-01]
2. The menu do padre SHALL mostrar o nível do personagem e o custo do próximo ponto. [PRST-02]
3. The menu do padre SHALL listar cada atributo com nome e nível, e, para cada status que ele move, o nome do status, o valor atual e o valor com o próximo ponto. [PRST-03]
4. WHEN o jogador comprar um ponto num atributo com ouro suficiente THEN o padre SHALL tirar o custo do ouro do save. [PRST-04]
5. WHEN o jogador comprar um ponto num atributo com ouro suficiente THEN o padre SHALL subir em 1 o nível desse atributo no save. [PRST-05]
6. The nível do personagem SHALL ser a soma dos níveis de todos os atributos. [PRST-06]
7. The custo do próximo ponto SHALL ser 5 + 5 x nível do personagem, em ouro, e SHALL ser o mesmo para qualquer atributo. [PRST-07]
8. WHEN o jogador comprar um ponto THEN o menu SHALL mostrar o novo nível do personagem, o novo custo e os novos valores de todos os atributos. [PRST-08]
9. WHEN o jogador clicar em "Fechar" ou apertar Esc THEN o hub SHALL fechar o menu do padre. [PRST-09]

**Independent Test**: Com 20 de ouro, comprar Vitalidade (5) e Força (10): sobram 5, o personagem fica no nível 2 e o próximo ponto custa 15 em qualquer atributo.

---

### P1: Atributos valem na run ⭐ MVP

**User Story**: Como jogador, quero que os pontos comprados mudem meu personagem na run, para sentir o progresso.

**Why P1**: Sem isso a compra não tem efeito.

**Acceptance Criteria**:

1. WHEN uma run começar THEN o player SHALL somar à base do `PlayerStats`, para cada status, o ganho por ponto vezes o nível de cada atributo que move esse status. [PRST-10]
2. The Vitalidade SHALL subir a vida máxima. [PRST-11]
3. The Força SHALL subir o dano base do personagem, que entra no dano de qualquer arma. [PRST-12]
4. The Agilidade SHALL subir a velocidade de movimento, a distância da esquiva, a cadência de tiro e o raio de coleta de XP, e SHALL diminuir a recarga da esquiva. [PRST-13]
5. The Resistência SHALL subir a defesa. [PRST-14]
6. The bônus das cartas SHALL ser aplicado sobre os valores que já incluem os pontos do padre. [PRST-15]
7. The tela de resultado SHALL mostrar os status finais com os pontos do padre incluídos. [PRST-16]

**Independent Test**: Comprar 3 pontos de Vitalidade e começar uma run com 130 de vida máxima, partindo de 100.

---

### P1: Defesa ⭐ MVP

**User Story**: Como jogador, quero que a Resistência reduza o dano que recebo, para sobreviver mais tempo.

**Why P1**: A Resistência não teria efeito sem a mecânica de defesa.

**Acceptance Criteria**:

1. WHEN o player receber dano THEN o player SHALL perder o dano vezes (1 - defesa). [PRST-17]
2. The defesa SHALL começar em 0 e SHALL combinar os pontos de forma multiplicativa: defesa = 1 - (1 - ganho por ponto) ^ nível. [PRST-18]
3. The menu do padre SHALL mostrar a defesa como porcentagem. [PRST-19]

**Independent Test**: Com 10 pontos de Resistência (defesa de 18,3%), um golpe de 10 tira 8,17 de vida.

---

### P1: Atributos como dados ⭐ MVP

**User Story**: Como desenvolvedor, quero definir cada atributo num asset com uma lista de efeitos, para criar ou ajustar atributos sem mexer no código.

**Why P1**: Os números são provisórios, e novos efeitos vão entrar nos atributos depois.

**Acceptance Criteria**:

1. The cada atributo SHALL ser um asset com ID, nome, ícone e uma lista de efeitos, e cada efeito SHALL ter um status e um ganho por ponto. [PRST-20]
2. WHEN um novo asset de atributo for adicionado à lista do padre THEN o menu SHALL mostrá-lo sem mudança de código. [PRST-21]
3. WHEN um efeito com um status que já existe for adicionado a um atributo THEN a run e o menu SHALL usá-lo sem mudança de código. [PRST-22]

**Independent Test**: Criar um atributo de teste com um efeito de vida, colocá-lo na lista e vê-lo no menu e na run.

---

## Edge Cases

- IF o jogador não tiver ouro para o próximo ponto THEN o menu SHALL desativar o botão de compra de todos os atributos. [PRST-23]
- IF o jogador não tiver ouro para o próximo ponto THEN o menu SHALL mostrar o custo em vermelho. [PRST-24]
- WHEN o jogador clicar várias vezes seguidas em comprar THEN o padre SHALL cobrar e subir um ponto por clique, sem deixar o ouro negativo. [PRST-25]
- IF a recarga da esquiva com os pontos e as cartas ficar abaixo de 0,2 s THEN o player SHALL usar 0,2 s. [PRST-26]
- WHEN a Resistência tiver muitos pontos THEN a defesa SHALL continuar abaixo de 100%, e cada ponto SHALL render menos que o anterior. [PRST-27]
- IF o save tiver nível de um atributo que não existe mais THEN o nível SHALL ficar no save sem efeito e SHALL continuar contando no nível do personagem. [PRST-28]

Dimensões implícitas: validação (PRST-23, PRST-26, PRST-27), repetição (PRST-25), dados antigos (PRST-28) e persistência (via `save-system`, SAVE-13) estão cobertas. Concorrência, autenticação, dependências externas e ciclo de vida N/A: compra local, um clique por vez.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| PRST-01 | P1: Comprar pontos de atributo | - | Pending |
| PRST-02 | P1: Comprar pontos de atributo | - | Pending |
| PRST-03 | P1: Comprar pontos de atributo | - | Pending |
| PRST-04 | P1: Comprar pontos de atributo | - | Pending |
| PRST-05 | P1: Comprar pontos de atributo | - | Pending |
| PRST-06 | P1: Comprar pontos de atributo | - | Pending |
| PRST-07 | P1: Comprar pontos de atributo | - | Pending |
| PRST-08 | P1: Comprar pontos de atributo | - | Pending |
| PRST-09 | P1: Comprar pontos de atributo | - | Pending |
| PRST-10 | P1: Atributos valem na run | - | Pending |
| PRST-11 | P1: Atributos valem na run | - | Pending |
| PRST-12 | P1: Atributos valem na run | - | Pending |
| PRST-13 | P1: Atributos valem na run | - | Pending |
| PRST-14 | P1: Atributos valem na run | - | Pending |
| PRST-15 | P1: Atributos valem na run | - | Pending |
| PRST-16 | P1: Atributos valem na run | - | Pending |
| PRST-17 | P1: Defesa | - | Pending |
| PRST-18 | P1: Defesa | - | Pending |
| PRST-19 | P1: Defesa | - | Pending |
| PRST-20 | P1: Atributos como dados | - | Pending |
| PRST-21 | P1: Atributos como dados | - | Pending |
| PRST-22 | P1: Atributos como dados | - | Pending |
| PRST-23 | Edge Cases | - | Pending |
| PRST-24 | Edge Cases | - | Pending |
| PRST-25 | Edge Cases | - | Pending |
| PRST-26 | Edge Cases | - | Pending |
| PRST-27 | Edge Cases | - | Pending |
| PRST-28 | Edge Cases | - | Pending |

**Coverage:** 28 total, 0 mapped to tasks, 28 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - padre sobe o personagem um ponto por vez, só com ouro, sem teto no MVP; referência soulslike; luz sobe à parte, na fogueira; defesa citada entre os status.
- Conversa de 2026-11-04 com o usuário: modelo soulslike, os quatro atributos, custo pelo nível total, ganho linear, defesa percentual multiplicativa no estilo soulslike, Agilidade no lugar da Destreza, nome amplo para a Resistência.
- `.specs/STATE.md` - AD-001, AD-007 e AD-012.
- `Assets/_Project/Code/Runtime/Player/PlayerStats.cs`, `PlayerRunStats.cs` e `Combat/Health.cs` - status base, cartas e dano recebido.
- `.specs/features/save-system/spec.md` - SAVE-02 e SAVE-13.

---

## Success Criteria

- [ ] O ouro de uma vitória (70) compra pelo menos 4 pontos no começo do jogo.
- [ ] Um ponto comprado aparece no valor da run seguinte e na tela de resultado.
- [ ] Com Resistência, o dano recebido cai na porcentagem mostrada no menu.
