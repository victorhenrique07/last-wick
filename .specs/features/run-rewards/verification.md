# Run-rewards verification

**Verdict**: FAIL
**Proofs**: `proofs.md` at 0114863
**Verifier**: independent sub-agent (author != verifier)

Não dá para rodar o Unity Editor nesta verificação; os exemplos próprios foram feitos lendo o código em `455e9e4`. `Halved()` usa divisão inteira sobre valores nunca negativos: 1 vira 0, 7 vira 3, 0 continua 0 (RWD-18 e RWD-21 se mantêm). `MaterialChance` = 1 faz `NextDouble() < 1` valer sempre e 0 não vale nunca (RWD-07 se mantém nos dois extremos). Um boss com índice 3 (fora de `Bosses`) e a vitória no boss final quebram, como mostram as Findings.

## Checks

| Check | Test | Holds |
| --- | --- | --- |
| C1 | Walked: teste do agente no Editor na T5, 192 materiais em 1000 abates | yes - fora de 10%-30% falharia; o código usa `NextDouble() < MaterialChance` |
| C2 | Walked: delta do `save.json` depois da morte (osso/ferro/cera +12/+15/+12, de 39 entregues) | yes - um material faltando ou dominando apareceria; `rng.Next(0, 3)` é uniforme |
| C3 | Walked: boss 1 no Play Mode (+10, confirmado pelo save) e bosses 1 e 2 no Editor na T5 | no - o boss final nunca foi morto, nem no Play Mode nem no Editor, e é justamente o caso que o código erra (os 40 de ouro não entram) |
| C4 | Walked: o mesmo teste da C3 (+30 do boss 1 no save, +30 do boss 2 no Editor) | no - os 50 de óleo do boss final nunca foram observados e o código os perde |
| C5 | Walked: delta do `save.json` e T5 no Editor | yes - por construção: `BossReward` não tem campo de material e `OnBossDefeated` só soma `Gold` e `Oil`; o delta do save sozinho não separaria material de boss do de monstro |
| C6 | Walked: teste do agente no Editor na T5, `RaiseOilCollected(5)` subiu `Oil` em 5 | yes - na camada do evento; o orbe é da `campfire-light` e ainda não existe |
| C7 | walk "Mudar a chance para 1 no asset" - só "está tudo confirmado" | no - nenhuma observação registrada; o código (`RewardConfig.asset` com 0,2 e 10/30, 20/30, 40/50) atende, mas a prova não falharia se não atendesse |
| C8 | Walked: tela da morte mostrando "coletado → entregue" (relato do usuário) e T12 no Editor | yes - a seta mostra `Collected` de cada recurso; um campo vazio apareceria como 0 → 0 numa run com coleta |
| C9 | walk "Vencer e ver Delivered igual a Collected" - só "está tudo confirmado" | no - o único walk registrado termina em morte; nenhuma vitória foi observada. O código (`Deliver` devolve `collected` na vitória) atende |
| C10 | Walked: `save.json` ganhou 5 de ouro e 15 de óleo de 10 e 30 coletados; T4 no Editor (30/7/1 vira 15/3/0) | yes - os casos ímpares e pares da regra estão cobertos na morte e na escuridão |
| C11 | Walked: delta do `save.json` igual ao entregue | yes - `SaveSystem.Add` e `Save()` vêm antes de `RaiseRunFinished` no `RunFinisher.Finish` |
| C12 | walk "Matar o boss 1 e ler o `save.json`" - só "está tudo confirmado" | no - nenhuma leitura do save no meio da run foi registrada; por construção atende (o único `SaveSystem.Add`/`Save` da run está no `Finish`) |
| C13 | Walked: bloco conferido pelo agente no Editor na T12 e tela da morte relatada | yes - as 5 entradas com ícone e nome vêm do `RunResultScreenBuilder` |
| C14 | Walked: relato do usuário, a tela da morte mostrou "coletado → entregue" | yes |
| C15 | Walked: relato do usuário, o texto de perda apareceu na morte | yes |
| C16 | Walked: T12 no Editor, vitória sem o texto de perda | yes - `lossLabel.SetActive(reason != Victory)` |
| C17 | walk "Vencer e ver os quatro blocos" - só "está tudo confirmado" | no - nenhuma vitória observada; o código mantém resumo, status, cartas e botão |
| C18 | Walked: T4 no Editor, 1 vira 0 na morte e na escuridão | yes |
| C19 | walk "Chegar ao marco com 20 monstros vivos" - só "está tudo confirmado" | no - nada registrado; por construção atende (`Spawner.OnStageCleared` usa `Destroy`, que não levanta `Health.Died`) |
| C20 | Walked: T5 no Editor, nada somado depois do `RunFinished` | yes - `finished` é checado nos três handlers |
| C21 | Walked: T12 no Editor, "0 → 0" sem nada coletado | yes |

## Findings

| Finding | Evidence | Fix | Closed |
| --- | --- | --- | --- |
| Na vitória, o ouro e o óleo do boss final (40 e 50) nunca chegam ao `RunResult` nem ao save; a vitória completa rende 30 de ouro e 60 de óleo, não os 70 e 110 do Success Criteria | `FirstOne.unity`: no objeto `Run`, o `RunFinisher` (`&1480376264`) vem antes do `RunRewards` (`&1480376266`) na lista de componentes, então assina `BossDefeated` primeiro. Com o índice 2, `RunFinisher.Finish` lê `rewards.Collected` (cópia de struct) sem o boss final e levanta `RunFinished`, que liga `RunRewards.finished`; o `RunRewards.OnBossDefeated` que vem depois retorna sem somar | Tirar a dependência da ordem de assinatura: por exemplo, o `RunFinisher` pedir ao `RunRewards` que some o boss antes de montar o resultado, ou o `RunRewards` avisar o `RunFinisher` depois de somar. Adicionar uma check que mate o boss final e veja `Collected.Gold` subir 40 e `Oil` 50 antes da vitória | `4f4fce5` - o `RunFinisher` anota a vitória e encerra no `LateUpdate`; C3 e C4 agora provadas pelo segundo walk (boss final 30/60 → 70/110, save +70/+110) |
| C3 e C4 não cobrem o boss final: o Play Mode só matou o boss 1 e o Editor só os bosses 1 e 2 | Linha `**Walked:**` em `tasks.md` | Conferir os três bosses, com o final levando à vitória, e registrar os valores lidos | segundo walk na linha `**Walked:**` - os três bosses lidos no `Collected`, o final levando à vitória (C3, C4) |
| Índice de boss fora de `RewardConfig.Bosses` lança `IndexOutOfRangeException` em vez do `Debug.LogError` que o design promete; a exceção também interrompe a invocação do evento `BossDefeated` para os assinantes seguintes | `RunRewards.cs:52` - `config.Bosses[bossIndex]` sem checar o tamanho; design, Error Handling Strategy | Checar `bossIndex` contra `config.Bosses.Length`, logar o erro e retornar |`4843c6c` - `RunRewards.OnBossDefeated` checa o índice e loga; o design não tem check para o caso de erro |
| `RewardConfig` ou `RunRewards` sem referência não seguem o design: `RunRewards` lança `NullReferenceException` a cada abate, e no `RunFinisher` a exceção vem depois de `finished = true`, então `RunFinished` nunca é levantado e a tela de resultado não abre | `RunRewards.cs:40` (`config.MaterialChance`), `RunFinisher.cs:71` (`rewards.Collected`), `RunFinisher.cs:79`; design: "Debug.LogError no Awake e RunRewards desligado" e "Debug.LogError e entrega de 0" | Validar as referências no `Awake` como o design diz e, no `RunFinisher`, entregar um `ResourceBag` vazio quando `rewards` for nulo |`4843c6c` - `RunRewards.Awake` loga e se desliga sem config; `RunFinisher.BuildResult` entrega 0 sem `rewards`; o design não tem check para o caso de erro |
| C7, C9, C12, C17 e C19 só têm "está tudo confirmado" como prova; nenhuma vitória foi registrada no walk | Linha `**Walked:**`: a única run descrita termina em morte | Refazer esses passos (incluindo uma vitória) e registrar o que foi visto: o valor do asset, `Delivered` = `Collected`, o ouro do save no meio da run, os quatro blocos e `Collected` antes e depois do marco | segundo walk na linha `**Walked:**` - C7 (chance 1: 20 de 20), C9 (save +70/+110 = coletado), C12 (save em 5 de ouro durante a run), C17 (os quatro blocos e o botão ativos), C19 (38 apagados, `Kills` +1) |
