# Priest-stats verification

**Verdict**: FAIL
**Proofs**: `proofs.md` at 414d454
**Verifier**: independent sub-agent (author != verifier)

## Checks

| Check | Test | Holds |
| --- | --- | --- |
| C1 | walk - "Hub: o clique na igreja abriu 'O Padre'" (`HubInteractable.OnPointerClick`) | yes - o `onClick` serializado em `Hub.unity` aponta para `PriestMenu.Open` (fileID 122333398); o caminho de raycast é do `hub-flow`, e o usuário abriu o menu pelo clique real antes do Esc |
| C2 | walk - "Nível 0" e "Próximo ponto: 5 de ouro" num save novo | yes |
| C3 | walk - "Vitalidade Nv 0 [Vida máxima 100 → 110]" e as 5 linhas da Agilidade | yes |
| C4 | walk - com 20 de ouro, comprar Vitalidade deixou 15 | yes |
| C5 | walk - `save.json` com vitalidade 1 e forca 1 | yes |
| C6 | walk - comprar Vitalidade e Força e ler "Nível 2" | no - com 1 ponto em cada atributo, a soma dos níveis e a contagem de entradas do `PriestLevels` dão 2; uma regra errada (contar atributos) passa. Só a C28 (nível 3 numa entrada) separa as duas |
| C7 | walk - Vitalidade (5) e Força (9) deixaram 6 e custo 16 | yes - 5, 9, 16 separa 1,8× de um passo linear; o custo é um rótulo só, igual para todos |
| C8 | walk - depois da compra, "Nível 1", custo 9 e "Vida máxima 110 → 120" | yes |
| C9 | walk - "Fechar" (`onClick` persistente → `PriestMenu.Close`) e Esc apertado pelo usuário no Game view | yes |
| C10 | walk - run com Vitalidade 3, Força 2, Agilidade 5, Resistência 10, status lidos no `PlayerRunStats` | yes |
| C11 | walk - vida 130 com Vitalidade 3 | yes |
| C12 | walk - `CalculateDamage(0)` 2 com Força 2 | yes - `block.Damage` entra na soma que o `PlayerAttack` usa com o `BaseDamage` de qualquer arma |
| C13 | walk - velocidade 1,75, esquiva 2,75, recarga 1,8 s, cadência 1,1, coleta 2,25, projétil 1 | yes |
| C14 | walk - defesa 0,1829 e multiplicador do `Health` 0,8171 | yes - separa a conta multiplicativa da soma (0,2) |
| C15 | walk - Vitalidade 3 + carta de +10%: 143 | yes - carta antes dos pontos daria 140 |
| C16 | walk - tela de resultado com "Vida máxima 143" | yes - o walk leu 143 (com a carta), e não os 130 da prova, mas sem os pontos seriam 110 |
| C17 | walk - golpe de 10 (`Health.TakeDamage`) tirou 8,17 | yes - o `Hurtbox` só repassa para o `TakeDamage` |
| C18 | walk - sem Resistência 10; Resistência 1: 9,80; Resistência 30: 0,4545 e 5,45 | yes |
| C19 | walk - "Defesa 0% → 2%" | yes |
| C20 | walk - os 4 assets em `Data/Priest/` com ID, nome, ícone e efeitos | yes - conferido também no YAML: `Id`, `DisplayName`, `Icon` (sprites existentes) e `effects` |
| C21 | walk - atributo "Teste" na lista em memória apareceu no menu depois de recarregar o hub | yes |
| C22 | walk - "Teste" com +7 de vida: "Vida máxima 107 → 114" no menu e 107 na run | yes - o efeito é lido da lista pelo `PermanentStats.Build`, o mesmo caminho dos 5 efeitos da Agilidade |
| C23 | walk - com 6 de ouro, os 4 botões desligados | yes |
| C24 | walk - custo em #C96D45 sem ouro | no - #C96D45 é laranja-terracota (matiz ~18°), não vermelho; o walk aceitou a cor que o builder pôs (`Warning`), e a captura `Logs/priest_walk_sem_ouro.png` mostra o custo laranja |
| C25 | walk - 5 `onClick.Invoke` seguidos em Vitalidade com 20 de ouro: 2 pontos e 6 de ouro | yes - o `Invoke` ignora o `interactable`, então prova também a guarda do `TryBuy` |
| C26 | walk - Agilidade 50: recarga 0,2 s | no - com Agilidade 50 o piso já vem do `PermanentStats.Build`; tirar o `Mathf.Max` do `PlayerRunStats.Refresh` (depois das cartas) não faz o walk falhar. O caso que o critério nomeia, pontos e cartas juntos abaixo de 0,2 s, não foi exercitado |
| C27 | walk - Resistência 100: 0,8674 e 1,33; ponto 100 rende ~0,27% contra 2% | yes |
| C28 | walk - nível 3 de "atributo-removido": "Nível 3", custo 29, sem efeito na run | yes |

## Findings

| Finding | Evidence | Fix | Closed |
| --- | --- | --- | --- |
| Uma posição vazia na lista `Attributes` do `GameDatabase` quebra a run e o menu, quando o `OnValidate` a trata como só um aviso | `PermanentStats.cs:48-50` chama `levelOf(attribute)` sem checar `null`; na run o lambda faz `attr.Id` (`PermanentStats.cs:62`) e o `PlayerRunStats.Awake` lança NRE antes do `Refresh`, e depois o `Refresh` do `inventory.Changed` roda com o `block` zerado (vida máxima 0); no menu o `Redraw` chama `PriestShop.LevelOf(null)` (`PriestShop.cs:11`), embora o `CreateRows` pule a posição. O design diz que o menu e a run ignoram atributo inválido | Pular `null` (e `Id` vazio) dentro do `PermanentStats.Build`, para a run e o menu || `9823ef4` - `PermanentStats.Build` pula posição vazia e atributo sem ID; conferido no Editor (lista com vazio e sem ID dá vida 130) |
| A C6 não separa soma de contagem de atributos | O walk compra 1 ponto em Vitalidade e 1 em Força: soma e contagem dão 2 | Na C6, comprar 2 pontos no mesmo atributo e ler "Nível 2" || `3a1996a` - segundo walk: 2 pontos na Vitalidade dão "Nível 2" (C6) |
| O custo sem ouro sai laranja, não vermelho (PRST-24) | `HubCanvasBuilder.Warning` = `Color32(201, 109, 69)`, serializado em `missingGoldColor` no `Hub.unity:619`; a captura mostra laranja | Usar um vermelho em `missingGoldColor`, ou registrar a confirmação do usuário de que essa cor vale como "vermelho" || `30407d3` - o usuário escolheu a cor de aviso da paleta; PRST-24 e C24 passaram a pedir o terracota #C96D45 |
| A C26 não exercita o piso depois das cartas | O piso existe duas vezes (`PermanentStats.cs:57` e `PlayerRunStats.cs:64`); Agilidade 50 sem carta passa só com o primeiro | Somar ao walk um caso com pontos e uma carta de recarga que levem a conta abaixo de 0,2 s sem o piso do `Build` (ex.: Agilidade 40 → 0,4 s e carta de −50%), e ler 0,2 s || `3a1996a` - segundo walk: Agilidade 40 (0,4 s) com carta de -60% fica em 0,2 s, não 0,16 s (C26) |
| Sem teto, o custo estoura o `int` a partir do nível 34 e fica negativo; o `TrySpend` com custo negativo dá ouro | `PriestShop.NextCost`: `5 × 1,8^34` ≈ 2,4e9 > `int.MaxValue` no `Mathf.RoundToInt`; `ResourceBag.Covers` aceita custo negativo e `Resources -= cost` soma ouro. Inalcançável jogando (o ponto 34 custa ~1,3e9), mas um save editado com níveis de atributos removidos (PRST-28) chega lá | Calcular em `double` e prender o custo em `int.MaxValue`, ou recusar custo negativo no `TryBuy` || `9823ef4` - `NextCost` calcula em double e para no `int.MaxValue`; conferido no Editor (nível 34 e 200 dão 2147483647, níveis 0 e 3 continuam 5 e 29) |
