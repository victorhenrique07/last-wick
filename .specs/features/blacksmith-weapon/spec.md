# Ferreiro: melhoria da arma Specification

**Approved**: 2026-11-06 - user: "Aprovo, sub-agentes"
**Revisão**: 2026-11-06 - ajustes da revisão contra o código atual (usuário: "Aplica os 7 ajustes"). Primeira aprovação: user: "tudo ok. Podemos seguir com o design das specs.".

## Problem Statement

A arma do player tem dano e cadência fixos no componente `Weapon` da cena, e nada a melhora entre runs. O One-Pager coloca o ferreiro no MVP para melhorar a arma inicial com materiais, e o usuário pediu que o código aceite novas armas sem mudanças. A bigorna do hub já é clicável, mas não abre nada.

## Goals

- [ ] Clicar na bigorna abre o menu do ferreiro, que melhora a arma equipada com materiais.
- [ ] Cada nível de melhoria fica no save e vale em todas as runs seguintes.
- [ ] Adicionar uma arma nova é criar um asset e colocá-lo na lista de armas, sem mudar código.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Criar ou desbloquear armas novas no ferreiro | AD-003: só a arma atual no MVP. |
| Trocar a arma equipada pela interface | Só existe uma arma; vira tela quando houver outras. |
| Cartas que mudam a arma principal | Feature de cartas separada. |
| Ataques diferentes por arma (área, corpo a corpo) | A primeira arma é de projétil; outros tipos vêm com as armas novas. |
| NPC animado do ferreiro | O MVP usa a bigorna como símbolo. |
| Valores finais de ganho e custo | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O Claude escreve o código e monta o menu | Usuário, em 2026-11-06: "Claude escreve tudo" (substitui a AD-001 nesta feature) | y |
| Escopo do ferreiro | Só melhora a arma equipada; dados prontos para várias armas | AD-003 | y |
| Chave da arma no save | O `Id` do asset da arma | AD-007 | y |
| O que cada nível melhora | Cada nível define seus próprios bônus de dano, cadência, velocidade do projétil, alcance e perfuração | Cada arma pode ter um caminho de melhoria diferente | y |
| Níveis da arma inicial (provisório) | 5 níveis, alternando +2 de dano e +10% de cadência | Ganho perceptível sem quebrar a horda | y |
| Custo por nível (provisório) | Cada nível tem seu custo em osso, ferro e cera; começa em 10 materiais e sobe 10 por nível | Materiais são do ferreiro no One-Pager | y |
| Teto | O número de níveis definido no asset da arma | O One-Pager deixa o teto do ferreiro "por definir" | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário na feature anterior | y |
| Dano e cadência na run | Dano = dano do padre (Força) + dano da arma no nível + dano fixo das cartas, tudo vezes o bônus de dano das cartas; cadência = cadência da arma no nível x multiplicador do padre x cartas | É a fórmula que o `PlayerRunStats` já usa desde o priest-stats; a arma só troca o número fixo pelo do asset | y |
| Cor do material que falta | Cor de aviso da paleta (terracota #C96D45), igual ao padre, à fogueira e à mesa | Revisão de 2026-11-06; o "vermelho" do SMITH-16 é essa cor | y |
| Ids de arma no save | Ids vazios e repetidos em `WeaponLevels` saem ao carregar, como nas cartas | Lição da mesa de tarot (Verifier, achado 1) | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Melhorar a arma ⭐ MVP

**User Story**: Como jogador, quero gastar materiais na bigorna para melhorar minha arma, para matar mais rápido nas próximas runs.

**Why P1**: É o uso dos materiais junto com a mesa de tarot e está no escopo do MVP.

**Acceptance Criteria**:

1. WHEN o jogador clicar na bigorna THEN o hub SHALL abrir o menu do ferreiro. [SMITH-01]
2. The menu do ferreiro SHALL mostrar o nome, o ícone e o nível da arma equipada, no formato "Nível N/M". [SMITH-02]
3. The menu do ferreiro SHALL mostrar os status atuais da arma e o que muda no próximo nível. [SMITH-03]
4. The menu do ferreiro SHALL mostrar o custo do próximo nível em osso, ferro e cera. [SMITH-04]
5. WHEN o jogador melhorar a arma com materiais suficientes THEN o ferreiro SHALL tirar o custo dos materiais do save. [SMITH-05]
6. WHEN o jogador melhorar a arma com materiais suficientes THEN o ferreiro SHALL subir em 1 o nível dessa arma no save. [SMITH-06]
7. WHEN o jogador clicar em "Fechar" ou apertar Esc THEN o hub SHALL fechar o menu do ferreiro. [SMITH-07]

**Independent Test**: Com 10 materiais do tipo pedido, melhorar a arma para o nível 1 e ver os materiais caírem e o nível subir.

---

### P1: A melhoria vale na run ⭐ MVP

**User Story**: Como jogador, quero que a arma melhorada atire mais forte na run.

**Why P1**: Sem isso a compra não tem efeito.

**Acceptance Criteria**:

1. WHEN uma run começar THEN o player SHALL usar a arma equipada do save com os status base do asset dela. [SMITH-08]
2. WHEN uma run começar THEN a arma SHALL somar os bônus de todos os níveis comprados, do nível 1 até o nível atual. [SMITH-09]
3. The bônus das cartas SHALL ser aplicado sobre os status da arma já melhorada, somados ao dano e à cadência do padre. [SMITH-10]

**Independent Test**: Melhorar a arma até o nível 2 (+2 de dano e +10% de cadência) e ver o dano e a cadência maiores na run.

---

### P1: Armas como dados ⭐ MVP

**User Story**: Como desenvolvedor, quero que cada arma seja um asset com status e níveis, para adicionar armas novas sem mexer no código.

**Why P1**: Pedido explícito do usuário (AD-003).

**Acceptance Criteria**:

1. The cada arma SHALL ser um asset com ID, nome, ícone, projétil, status base e lista de níveis de melhoria. [SMITH-11]
2. The cada nível de melhoria SHALL definir seus bônus e seu custo em osso, ferro e cera. [SMITH-12]
3. The jogo SHALL ter uma lista de todas as armas, com uma marcada como arma inicial. [SMITH-13]
4. WHEN um novo asset de arma for colocado na lista e equipado no save THEN a run SHALL usá-lo sem mudança de código. [SMITH-14]

**Independent Test**: Criar uma segunda arma de teste com outro projétil, equipá-la mudando o save e jogar uma run com ela.

---

## Edge Cases

- WHILE a arma estiver no último nível the menu SHALL mostrar "Arma no nível máximo" e desativar a melhoria. [SMITH-15]
- IF o jogador não tiver os materiais do próximo nível THEN o menu SHALL desativar a melhoria e mostrar na cor de aviso (#C96D45) o material que falta. [SMITH-16]
- IF o nível salvo for maior que o número de níveis da arma THEN a arma SHALL usar o último nível. [SMITH-17]
- WHEN o jogador clicar várias vezes seguidas em melhorar THEN o ferreiro SHALL cobrar e subir um nível por clique, sem deixar material negativo. [SMITH-18]

Dimensões implícitas: validação (SMITH-15, SMITH-16, SMITH-17) e repetição (SMITH-18) estão cobertas. Persistência via `save-system` (SAVE-06, SAVE-07, SAVE-13, SAVE-22). Concorrência, autenticação e dependências externas N/A.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| SMITH-01 | P1: Melhorar a arma | - | Pending |
| SMITH-02 | P1: Melhorar a arma | - | Pending |
| SMITH-03 | P1: Melhorar a arma | - | Pending |
| SMITH-04 | P1: Melhorar a arma | - | Pending |
| SMITH-05 | P1: Melhorar a arma | - | Pending |
| SMITH-06 | P1: Melhorar a arma | - | Pending |
| SMITH-07 | P1: Melhorar a arma | - | Pending |
| SMITH-08 | P1: A melhoria vale na run | - | Pending |
| SMITH-09 | P1: A melhoria vale na run | - | Pending |
| SMITH-10 | P1: A melhoria vale na run | - | Pending |
| SMITH-11 | P1: Armas como dados | - | Pending |
| SMITH-12 | P1: Armas como dados | - | Pending |
| SMITH-13 | P1: Armas como dados | - | Pending |
| SMITH-14 | P1: Armas como dados | - | Pending |
| SMITH-15 | Edge Cases | - | Pending |
| SMITH-16 | Edge Cases | - | Pending |
| SMITH-17 | Edge Cases | - | Pending |
| SMITH-18 | Edge Cases | - | Pending |

**Coverage:** 18 total, 0 mapped to tasks, 18 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - ferreiro melhora a arma inicial no MVP com materiais; teto por definir.
- `.specs/STATE.md` - AD-001, AD-003 e AD-007.
- `Assets/_Project/Code/Runtime/Combat/Weapon.cs` e `Player/PlayerAttack.cs` - dano e cadência atuais e como as cartas se aplicam.
- `.specs/features/save-system/spec.md` - SAVE-06, SAVE-07 e SAVE-22.

---

## Success Criteria

- [ ] Um nível comprado aparece no dano ou na cadência da run seguinte.
- [ ] Uma segunda arma de teste roda numa run só com assets novos.
