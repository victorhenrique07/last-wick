# Recompensas da run Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."

## Problem Statement

Hoje a run termina sem render nada: o `RunResult` já tem campos de ouro, óleo, osso, ferro e cera, mas ninguém os preenche, e a tela de resultado não mostra recompensas. Sem recompensas, o padre, a fogueira, a mesa de tarot e o ferreiro não têm o que gastar. Esta feature calcula o que cada run rende, aplica a perda da morte, mostra o resultado e entrega o valor ao save.

## Goals

- [ ] Cada abate de monstro comum pode render um material, e cada boss rende ouro, óleo e materiais.
- [ ] Vitória guarda 100% do coletado; morte e escuridão guardam metade.
- [ ] A tela de resultado mostra o que o jogador leva e, na morte, o que perdeu.
- [ ] O que o jogador leva entra no save no fim da run.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Itens de material caindo no chão | AD-002: o material é sorteado no abate e somado no fim. |
| Drop de cura | É uma mecânica da run, não uma recompensa permanente; feature separada. |
| Bônus por terminar antes dos 10:00 | A fórmula está em aberto no One-Pager ("fica com o Victor"). |
| Sair no meio da run | Não existe menu de pausa; vira feature junto com ele. |
| Orbes de óleo no mapa | Fazem parte da feature `fogueira-light`; aqui só somamos o óleo que eles entregam. |
| Valores finais de chance e quantidade | Provisórios; balanceamento depois do MVP. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude monta o bloco de recompensas da tela | AD-001 | y |
| Drop dos monstros comuns | 20% de chance de 1 material por abate, osso, ferro ou cera com a mesma chance | Balanceamento inicial do One-Pager | y |
| Ouro dos bosses | 10, 20 e 40 | Decidido no One-Pager | y |
| Óleo dos bosses | 30, 30 e 50 | Balanceamento inicial do One-Pager | y |
| Materiais dos bosses | Uma quantidade fixa por boss, sorteada material a material; provisório 10, 15 e 20 | O One-Pager diz que bosses dropam materiais, sem quantidade | y |
| Arredondamento da perda | Metade arredondada para baixo (7 vira 3) | Regra simples e previsível; nunca dá meio material | y |
| Onde ficam os números | Num asset de dados editável no Inspector | Balancear sem mudar código | y |
| Texto da perda na tela | "Metade do que você coletou se perdeu na escuridão." | Tom do jogo; ajustável | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário na feature anterior | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Cada abate e cada boss rendem recursos ⭐ MVP

**User Story**: Como jogador, quero que matar monstros e bosses me renda materiais, ouro e óleo, para ter o que gastar no acampamento.

**Why P1**: É a entrada de todos os recursos da meta-progressão.

**Acceptance Criteria**:

1. WHEN um monstro comum morrer por dano THEN a run SHALL sortear 1 material com 20% de chance. [RWD-01]
2. WHEN o sorteio do material de um monstro comum acertar THEN a run SHALL escolher osso, ferro ou cera com a mesma chance. [RWD-02]
3. WHEN o boss 1, 2 ou final morrer THEN a run SHALL somar 10, 20 ou 40 de ouro, respectivamente. [RWD-03]
4. WHEN o boss 1, 2 ou final morrer THEN a run SHALL somar 30, 30 ou 50 de óleo, respectivamente. [RWD-04]
5. WHEN um boss morrer THEN a run SHALL somar a quantidade de materiais desse boss, sorteando cada unidade entre osso, ferro e cera. [RWD-05]
6. WHEN um orbe de óleo for coletado THEN a run SHALL somar o óleo do orbe ao óleo coletado. [RWD-06]
7. The chance de material, os materiais possíveis e as recompensas de cada boss SHALL ficar num asset de dados editável no Inspector. [RWD-07]

**Independent Test**: Matar 50 monstros e o boss 1 numa run de teste e conferir, no `RunResult`, cerca de 10 materiais dos monstros, 10 de ouro, 30 de óleo e os materiais do boss.

---

### P1: Perda na morte ⭐ MVP

**User Story**: Como jogador, quero perder metade do que coletei quando morro, para que vencer valha mais e morrer tenha peso.

**Why P1**: Regra central de risco do One-Pager.

**Acceptance Criteria**:

1. The `RunResult` SHALL trazer a quantidade coletada de ouro, óleo, osso, ferro e cera. [RWD-08]
2. WHEN a run terminar em vitória THEN a run SHALL entregar ao jogador 100% de cada recurso coletado. [RWD-09]
3. WHEN a run terminar em morte ou escuridão THEN a run SHALL entregar ao jogador metade de cada recurso coletado, arredondada para baixo. [RWD-10]
4. WHEN a run terminar THEN a run SHALL somar ao save a quantidade entregue de cada recurso, antes da gravação do arquivo. [RWD-11]
5. WHILE a run estiver em andamento the save SHALL manter os recursos que tinha no início da run. [RWD-12]

**Independent Test**: Morrer com 7 osso e 30 de ouro coletados e ver o save ganhar 3 osso e 15 de ouro.

---

### P1: Recompensas na tela de resultado ⭐ MVP

**User Story**: Como jogador, quero ver na tela de resultado o que levo para o acampamento, para entender o valor da run.

**Why P1**: Sem ver as recompensas, o jogador não percebe o ciclo da meta-progressão.

**Acceptance Criteria**:

1. WHEN a tela de resultado abrir THEN ela SHALL mostrar uma seção de recompensas com ouro, óleo, osso, ferro e cera, cada um com ícone, nome e a quantidade entregue. [RWD-13]
2. WHEN a tela de resultado abrir depois de uma morte ou da escuridão THEN a seção de recompensas SHALL mostrar, para cada recurso, a quantidade coletada ao lado da entregue. [RWD-14]
3. WHEN a tela de resultado abrir depois de uma morte ou da escuridão THEN a seção de recompensas SHALL mostrar o texto "Metade do que você coletou se perdeu na escuridão.". [RWD-15]
4. WHEN a tela de resultado abrir depois de uma vitória THEN a seção de recompensas SHALL não mostrar o texto de perda. [RWD-16]
5. The tela de resultado SHALL manter o resumo, os status finais, as cartas e o botão "Voltar ao acampamento" que já mostra. [RWD-17]

**Independent Test**: Vencer uma run e morrer em outra: a vitória mostra só as quantidades; a morte mostra coletado, entregue e o texto de perda.

---

## Edge Cases

- IF um recurso coletado for ímpar THEN a run SHALL entregar o valor arredondado para baixo na morte (1 vira 0). [RWD-18]
- WHEN um marco remover os monstros vivos THEN a run SHALL não sortear material para eles. [RWD-19]
- WHEN um monstro morrer depois do fim da run THEN a run SHALL não somar o material dele. [RWD-20]
- IF nenhum recurso for coletado THEN a seção de recompensas SHALL mostrar 0 em cada um. [RWD-21]

Dimensões implícitas: transições de estado (RWD-12, RWD-20) e validação (RWD-18) estão cobertas. Persistência é do `save-system` (RWD-11 entrega o valor). Concorrência, autenticação, dependências externas e ciclo de vida N/A: tudo roda na thread principal, local, dentro de uma run.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| RWD-01 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-02 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-03 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-04 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-05 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-06 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-07 | P1: Cada abate e cada boss rendem recursos | - | Pending |
| RWD-08 | P1: Perda na morte | - | Pending |
| RWD-09 | P1: Perda na morte | - | Pending |
| RWD-10 | P1: Perda na morte | - | Pending |
| RWD-11 | P1: Perda na morte | - | Pending |
| RWD-12 | P1: Perda na morte | - | Pending |
| RWD-13 | P1: Recompensas na tela de resultado | - | Pending |
| RWD-14 | P1: Recompensas na tela de resultado | - | Pending |
| RWD-15 | P1: Recompensas na tela de resultado | - | Pending |
| RWD-16 | P1: Recompensas na tela de resultado | - | Pending |
| RWD-17 | P1: Recompensas na tela de resultado | - | Pending |
| RWD-18 | Edge Cases | - | Pending |
| RWD-19 | Edge Cases | - | Pending |
| RWD-20 | Edge Cases | - | Pending |
| RWD-21 | Edge Cases | - | Pending |

**Coverage:** 21 total, 0 mapped to tasks, 21 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - tabela de drops, ouro 10/20/40, óleo 30/30/50, 20% de material, perda de metade na morte e na escuridão.
- `.specs/STATE.md` - AD-001 e AD-002.
- `Assets/_Project/Code/Runtime/Run/RunResult.cs` - campos `Gold`, `Oil`, `Bones`, `Iron`, `Wax` já existentes.
- `Assets/_Project/Code/Runtime/Enemies/EnemyDeath.cs` e `Bosses/BossDeath.cs` - onde as mortes são avisadas hoje.
- `.specs/features/save-system/spec.md` - SAVE-12, gravação no fim da run.

---

## Success Criteria

- [ ] Uma vitória completa rende 70 de ouro e 110 de óleo antes dos orbes.
- [ ] O save depois de uma morte mostra metade, arredondada para baixo, de cada recurso coletado.
