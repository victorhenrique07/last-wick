# Campfire light verification

**Verdict**: FAIL
**Proofs**: `proofs.md` at 8e0e6e7
**Verifier**: independent sub-agent (author != verifier)

## Checks

| Check | Test | Holds |
| --- | --- | --- |
| C1 | walk - raycast do `EventSystem` no ponto das velas acerta "Fogueira" e o pointer enter liga o `Contorno` | yes - o raycast passa pelo `Physics2DRaycaster` e pelo `BoxCollider2D` reais (22x20 px, offset 0,5, coerente com o pivô 0,1); sem o `HubInteractable` ou o collider o contorno não acende |
| C2 | walk - clique no mesmo hit abre "A Fogueira" | yes - o clique dispara o `onClick` serializado no `Hub.unity` (`CampfireMenu.Open`, alvo `1013270523`); sem essa ligação o painel não abre |
| C3 | walk - save novo mostra "Nível 0 de 10", "Raio da luz 3,0 → 3,5" e "Custo: 40 de óleo" | yes - os quatro valores do critério são lidos dos textos redesenhados pelo `Redraw`, não do texto padrão do builder (que coincide, mas o redesenho seguinte em C4 mostra que os textos mudam) |
| C4 | walk - compras em sequência mostram custos 40 e 60 e o próximo 80 | yes - três pontos da reta fixam base 40 e passo 20; uma fórmula errada (passo trocado, `n` em vez de `n − 1`) dá outros números |
| C5 | walk - com 100 de óleo, 60 após a primeira compra e 0 após a segunda | yes - confere o óleo descontado a cada compra |
| C6 | walk - `save.json` com `LightLevel` 2 após duas compras, com "Nível 1 de 10" no meio | yes - lê o arquivo gravado, então prova o `Save()` e o +1 por compra |
| C7 | walk - "Fechar" fecha o menu; Esc apertado pelo usuário no Game view fecha | yes - o Esc passou pelo Input System real; o botão usa a chamada persistente serializada para `CampfireMenu.Close` |
| C8 | walk - run com `LightLevel` 4: `PlayerRunStats.LightRadius` e `PlayerLight.Radius` 5 | yes - 5,0 só sai de 3,0 + 0,5 × 4; o antigo `PlayerStats.LightRadius` (5) saiu do asset e do código, então não há como o valor vir dele |
| C9 | walk - run com `LightLevel` 4 e carta de +20% de luz pelo `GameEvents.RaiseCardChosen`: 6 | yes - o evento é o mesmo que o `LevelUpPanel` dispara e o `CardInventory` escuta; 6,0 distingue multiplicar a base da fogueira de somar ou de multiplicar a base 3,0 |
| C10 | walk - `LightLevel` 10 mostra "Nível 10 de 10", "Raio da luz 8,0", "Luz no máximo" e botão desligado | yes - o texto "Luz no máximo" só existe no ramo `IsMaxed`, que desliga o botão sem depender do óleo; o nível foi posto em memória e não no `save.json` como a prova diz, o que não muda o que o menu lê |
| C11 | walk - com 0 de óleo o custo fica #C96D45 e o botão desligado | yes - cor serializada `missingOilColor` = (201, 109, 69), igual ao `Warning` do builder e à cor do padre; o botão segue `CanBuy` |

## Findings

| Finding | Evidence | Fix | Closed |
| --- | --- | --- | --- |
| Um `LightLevel` negativo no save deixa comprar um nível de graça e o menu mostra valores sem sentido | `SaveSystem.Sanitize` não toca no `LightLevel`; com -1, `CampfireShop.NextCost` = `LightRules.CostOf(config, 0)` = 0, `CanBuy` é verdadeiro com 0 de óleo, `TryBuy` gasta 0 e sobe para 0; o menu mostra "Nível -1 de 10", "Raio da luz 3,0 → 3,0" (o `Radius` limita os dois lados a 0) e "Custo: 0 de óleo" | limitar `LightLevel` a `0..MaxLevel` no `Sanitize` (ou no `CampfireShop.Level`) |  `c2f327a` - `SaveSystem.Sanitize` prende o `LightLevel` em 0..MaxLevel e o `CampfireShop.Level` lê o nível preso; conferido no Editor (-1 vira 0 com custo 40, 12 vira 10, Sanitize -5→0 e 99→10) |
| O menu não limita o nível salvo acima de 10, ao contrário do que o design pede | Error Handling Strategy: "`Mathf.Clamp` no `Radius` e no menu"; `CampfireMenu.Redraw` escreve `Nível {level} de {MaxLevel}` com o valor cru, então um save com 12 mostra "Nível 12 de 10" (raio 8,0 e "Luz no máximo" estão certos); nenhuma check passa por um nível acima de 10 | usar o nível limitado no rótulo (o mesmo clamp do achado anterior resolve os dois) |  `c2f327a` - `SaveSystem.Sanitize` prende o `LightLevel` em 0..MaxLevel e o `CampfireShop.Level` lê o nível preso; conferido no Editor (-1 vira 0 com custo 40, 12 vira 10, Sanitize -5→0 e 99→10) |
