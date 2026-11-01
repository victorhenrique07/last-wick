# Spawn por ondas Specification

**Approved**: 2026-11-01 - user: "Aprovo a spec"

## Problem Statement

Os três stages do `Spawner` criam 2 inimigos por segundo, o mesmo teto teórico de abates da arma inicial. Sem cartas, a horda fica no limite entre estável e acumular sem parar; com qualquer carta de dano, ela nunca cresce. Por isso o jogo parece fácil e, com pequenos ajustes de vida ou spawn, passa a parecer difícil demais. As ondas dão à run momentos de pressão e de respiro, e o mínimo de vivos repõe inimigos para quem mata rápido.

## Goals

- [ ] Cada stage segue uma sequência de ondas configurável no Inspector, com duração, mínimo de vivos e intervalo próprios.
- [ ] Inimigos nascem mais rápido enquanto os vivos estão abaixo do mínimo da onda ativa.
- [ ] Marcos por pontos, os 3 bosses e o limite de 10:00 continuam funcionando sem mudança de regra.

## Out of Scope

Explicitamente fora desta feature.

| Feature | Reason |
| ------- | ------ |
| Mistura de inimigos por onda | Decisão do usuário: as ondas mudam só o ritmo; a mistura continua no stage. |
| Eventos, cercos e grupos de elite | Feature separada, depois de validar o ritmo das ondas. |
| Mudar vida, dano ou velocidade dos inimigos | Balanceamento separado; aqui muda uma alavanca por vez. |
| Reescalar bosses | Os bosses ficam como estão; esse é um pedido explícito da feature. |
| Registro de métricas de balanceamento | Feature separada. |
| Balanceamento final das ondas | Os valores são provisórios e serão ajustados em playtest. |
| Trocar `FindGameObjectsWithTag` por contagem própria | Otimização sem efeito no comportamento desta feature. |

---

## Assumptions & Open Questions

Toda ambiguidade está resolvida ou registrada aqui.

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Ondas acabam antes do marco | A última onda continua ativa até o marco | Usuário escolheu "Repetir a última" | y |
| Origem dos tipos de inimigo | `enemies` do stage, para todas as ondas | Usuário escolheu "Do stage" | y |
| Marcos no `RunConfig.asset` | Voltam a 3000/7500/14000 nesta feature | Usuário escolheu "Sim, entra" | y |
| Escopo do `fillInterval` | Um valor único no `Spawner`, começando em 0,1s | Um campo a menos por onda; reduz dados para balancear | y |
| `maxAlive` por stage | Mantém 50/120/250, valores atuais da cena | Só passa a atuar como limite técnico | n |
| Stage sem ondas | Não cria inimigos nele e registra um erro no Console | Erro de configuração visível e sem exceção a cada `FixedUpdate` | n |
| Valores das ondas | Provisórios, com pelo menos uma onda em cada stage | Os números finais vêm do playtest | n |
| Remoção de `using NUnit.Framework` do `Spawner.cs` | Remover junto com a mudança | Import sem uso que pode quebrar o build fora do Editor | n |
| Quem implementa | O usuário escreve, salvo pedido explícito de criação | Regra do `CLAUDE.md` do projeto | y |
| Como provar a feature | Conferência manual em Play Mode, sem testes automatizados | Usuário escolheu "Só teste manual" | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Ritmo por ondas ⭐ MVP

**User Story**: Como jogador, quero que a pressão da horda suba e alivie durante o stage para que a run tenha ritmo e continue exigente mesmo quando eu mato rápido.

**Why P1**: É a mudança central da feature.

**Acceptance Criteria**:

1. The `Spawner` SHALL escolher a onda ativa somando as `duration` do array `waves`, na ordem, contra o tempo decorrido no stage atual. [WAVE-01]
2. WHILE os inimigos vivos estiverem abaixo do `minAlive` da onda ativa o `Spawner` SHALL criar um inimigo a cada `fillInterval`. [WAVE-02]
3. WHILE os inimigos vivos estiverem entre o `minAlive` da onda ativa e abaixo do `maxAlive` do stage o `Spawner` SHALL criar um inimigo a cada `spawnInterval` da onda ativa. [WAVE-03]
4. WHILE os inimigos vivos forem iguais ou maiores que o `maxAlive` do stage o `Spawner` SHALL não criar inimigos. [WAVE-04]
5. WHEN o tempo do stage passar da soma das `duration` THEN o `Spawner` SHALL manter a última onda ativa até o marco do stage. [WAVE-05]
6. The `Spawner` SHALL sortear cada inimigo pelos pesos de `enemies` do stage atual, em todas as ondas. [WAVE-06]
7. The `Spawner` SHALL criar no máximo um inimigo por `FixedUpdate`. [WAVE-07]

**Independent Test**: Numa cena com uma onda de `minAlive` 20, sem atirar, os vivos sobem até 20 no ritmo de `fillInterval` e depois crescem no ritmo de `spawnInterval`, até parar em `maxAlive`.

---

### P1: Marcos e bosses preservados ⭐ MVP

**User Story**: Como jogador, quero que os marcos e os 3 bosses continuem como antes para que as ondas não mudem o objetivo da run.

**Why P1**: Pedido explícito da feature, e o fim da run depende dessa sequência.

**Acceptance Criteria**:

1. WHEN um marco for alcançado THEN o `Spawner` SHALL parar de criar inimigos. [WAVE-08]
2. WHEN um marco for alcançado THEN o `Spawner` SHALL remover os inimigos vivos. [WAVE-09]
3. WHEN um boss que não seja o último for derrotado THEN o `Spawner` SHALL retomar a criação de inimigos no stage seguinte. [WAVE-10]
4. WHEN um stage começar depois de um boss THEN o `Spawner` SHALL ativar a primeira onda desse stage. [WAVE-11]
5. WHEN o último boss for derrotado THEN o `Spawner` SHALL não criar mais inimigos. [WAVE-12]
6. WHEN o player morrer THEN o `Spawner` SHALL parar de criar inimigos. [WAVE-13]
7. The `RunConfig.asset` SHALL definir os marcos 3000, 7500 e 14000. [WAVE-14]

**Independent Test**: Jogar uma run até a vitória: cada marco limpa a arena e chama seu boss, cada stage recomeça na primeira onda e, depois do último boss, nenhum inimigo nasce.

---

### P1: Ondas configuradas no Inspector ⭐ MVP

**User Story**: Como desenvolvedor, quero editar as ondas no Inspector para ajustar o ritmo sem mudar código.

**Why P1**: Sem dados editáveis, o balanceamento em playtest depende de mudar código.

**Acceptance Criteria**:

1. The `SpawnWave` SHALL expor `duration`, `minAlive` e `spawnInterval` no Inspector. [WAVE-15]
2. The `SpawnStage` SHALL expor o array `waves` e manter `enemies` e `maxAlive`. [WAVE-16]
3. The `Spawner` SHALL expor `fillInterval` no Inspector. [WAVE-17]
4. The `Spawner` da cena `FirstOne` SHALL ter pelo menos uma onda em cada um dos 3 stages. [WAVE-18]

**Independent Test**: Alterar `minAlive` de uma onda no Inspector e ver a nova quantidade mínima no Play Mode, sem recompilar.

---

## Edge Cases

- IF um stage não tiver ondas THEN o `Spawner` SHALL não criar inimigos nesse stage. [WAVE-19]
- IF um stage não tiver ondas THEN o `Spawner` SHALL registrar um erro no Console uma vez. [WAVE-20]
- IF uma onda tiver `duration` igual ou menor que 0 THEN o `Spawner` SHALL pular para a onda seguinte. [WAVE-21]
- IF o `minAlive` da onda for maior que o `maxAlive` do stage THEN o `Spawner` SHALL parar de criar inimigos ao atingir `maxAlive`. [WAVE-22]
- WHEN a nova onda tiver `minAlive` menor que os vivos atuais THEN o `Spawner` SHALL manter os inimigos existentes vivos. [WAVE-23]

Dimensões implícitas: validação de dados e transições de estado estão cobertas acima; as demais são N/A nesta feature, que roda localmente e não persiste dados.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| WAVE-01 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-02 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-03 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-04 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-05 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-06 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-07 | P1: Ritmo por ondas | Tasks | In Tasks |
| WAVE-08 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-09 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-10 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-11 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-12 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-13 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-14 | P1: Marcos e bosses preservados | Tasks | In Tasks |
| WAVE-15 | P1: Ondas configuradas no Inspector | Tasks | In Tasks |
| WAVE-16 | P1: Ondas configuradas no Inspector | Tasks | In Tasks |
| WAVE-17 | P1: Ondas configuradas no Inspector | Tasks | In Tasks |
| WAVE-18 | P1: Ondas configuradas no Inspector | Tasks | In Tasks |
| WAVE-19 | Edge Cases | Tasks | In Tasks |
| WAVE-20 | Edge Cases | Tasks | In Tasks |
| WAVE-21 | Edge Cases | Tasks | In Tasks |
| WAVE-22 | Edge Cases | Tasks | In Tasks |
| WAVE-23 | Edge Cases | Tasks | In Tasks |

**Coverage:** 23 total, 23 mapped to tasks, 0 unmapped

---

## Sources

- `Docs/Last Wick - One-Pager.md` - regras de marcos, bosses, limite de 10:00, marcos 3000/7500/14000 e hordas sem parada até cada marco.
- `Assets/_Project/Code/Runtime/Run/Spawner.cs` - comportamento atual de spawn, marcos e bosses.
- `Assets/_Project/Code/Runtime/Run/SpawnEntry.cs` - dados atuais de `SpawnStage`.
- `Assets/_Project/Scenes/Maps/FirstOne.unity` - stages atuais: `spawnInterval` 0,5 e `maxAlive` 50/120/250.
- [Vampire Survivors Wiki: Enemies](https://vampire.survivors.wiki/w/Enemies) - referência do mínimo de vivos por onda.

---

## Success Criteria

- [ ] Uma run completa com os marcos 3000/7500/14000 chega à tela de vitória sem erros no Console.
- [ ] Sem atirar, os vivos alcançam o `minAlive` de cada onda e nunca passam do `maxAlive` do stage.
- [ ] Em playtest, a transição entre uma onda de pico e uma de respiro é perceptível.
