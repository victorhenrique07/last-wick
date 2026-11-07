# Ferreiro: melhoria da arma verification

**Verdict**: FAIL
**Proofs**: `proofs.md` at c762632 (commitado em `7e60c94`; diff da feature `e352ec8..7e60c94`)
**Verifier**: independent sub-agent (author != verifier)

## Checks

| Check | Test | Holds |
| --- | --- | --- |
| C1 | walk - clique na bigorna pelo raycast do `EventSystem` acertou "Bigorna (Ferreiro)" e abriu "O Ferreiro" | yes |
| C2 | walk - save novo: ícone `StartingWeapon`, "Centelha", "Nível 0/5"; depois "Nível 1/5" e "Nível 5/5" | yes |
| C3 | walk - "Dano" "10 → 12" no nível 0 e "Cadência" "4 → 4,4" no nível 1 | yes para dano e cadência; as linhas de velocidade, alcance e perfuração nunca aparecem (F2) |
| C4 | walk - custo 4/3/3 do nível 1, lido nos três rótulos | yes |
| C5 | walk - com 4/3/3 exatos, "Forjar" deixou 0/0/0 | yes |
| C6 | walk - `WeaponLevels` [centelha:1] lido no `save.json` | yes |
| C7 | walk - "Fechar" pelo raycast; Esc pelo usuário ("Esc fechou") | yes |
| C8 | walk - run com save novo, Força 0: arma "centelha", dano 10, cadência 4, projétil com 10 | yes |
| C9 | walk - nível 2: dano 12 e cadência 4,4; nível 9 (C17): 16 e 4,8 | yes para dano e cadência; a soma de velocidade, alcance e perfuração nunca é exercida, a arma inicial tem esses bônus em 0 (F2) |
| C10 | walk - nível 2 + duas Sangue Quente: projétil 14,4; Força 1: 15,6 | no - só o dano; a cadência com o multiplicador do padre e o bônus de cadência das cartas, e os bônus de projétil multiplicados pelos das cartas, não foram exercidos (F1, F2) |
| C11 | walk - `StartingWeapon` com Id, nome, ícone, `BasicProjectile`, base 10/4 e 5 níveis | yes |
| C12 | walk - 5 níveis com bônus e custo em osso, ferro e cera (conferido no asset: 10/20/30/40/50) | yes |
| C13 | walk - `WeaponCatalog` com 1 arma e "centelha" como inicial | yes; mas nada garante que a inicial esteja na lista (F3) |
| C14 | walk - cópia em memória "arma-teste" (base 25, "ProjetilTeste") no catálogo e no save; disparou "ProjetilTeste(Clone)" com 26 | yes |
| C15 | walk - forjar do 4 ao 5: "Nível 5/5", "Arma no nível máximo", sem status nem custo, "Forjar" desligado, clique extra sem efeito; save com nível 9 também | yes (a prova pedia nível 5 no save, o walk usou 9; o nível 5 exato foi lido pelo forjar) |
| C16 | walk - 0 materiais: três custos em #C96D45 e botão desligado; 4/3/2: só a cera | yes para osso, ferro e cera; custo em ouro ou óleo some da tela (F4) |
| C17 | walk - nível 9 no `save.json`: run com nível 5, dano 16, cadência 4,8, projétil 16 | yes (exemplo próprio: `Build` com -1, 4, 5, 6 e 9 dá 10/4, 14/4,8, 16/4,8, 16/4,8, 16/4,8) |
| C18 | walk - 16/20/14, 5 cliques no mesmo frame: nível 3 e 0/0/0 | yes |
| C19 | walk - `EquippedWeapon` "nao-existe" carregou como "centelha" e o ferreiro mostrou a Centelha | no - a prova pede "no ferreiro e na run"; a run com esse save não está registrada (F5) |
| C20 | walk - `StartingWeapon` vazio em memória: `OnValidate` registrou o erro e o valor foi restaurado | yes para o campo vazio; inicial fora da lista passa sem erro (F3) |
| C21 | walk - [centelha:1, centelha:3, "":2, desconhecida:4] carregou como [centelha:1] | yes |

## Findings

| Finding | Evidence | Fix | Closed |
| --- | --- | --- | --- |
| F1 - C10 só prova o dano; a cadência do SMITH-10 (arma no nível x padre x cartas) não tem caso no walk (L-002) | `PlayerRunStats.cs:73` e `Weapon.cs:51`; o walk de C10 lê só o dano do projétil. Se `TryFire` ignorasse `fireRateMultiplier`, ou usasse a cadência base em vez de `stats.FireRate`, C10 passaria igual | no walk de C10, com nível 2, Cadência do padre acima de 1 e uma carta de cadência, ler o intervalo entre tiros (4,4 x padre x cartas) | `f0c08b5` - walk da rodada 2: com Agilidade 1 e Sino Apressado, multiplicador 1,173 e intervalo real entre tiros de 5,630 tiros/s = 4,8 x 1,02 x 1,15 - C10 |
| F2 - Os bônus de velocidade, alcance e perfuração dos níveis (SMITH-09, SMITH-12) e a composição com as cartas (design: multiplicam, perfuração soma) nunca rodam: a arma inicial tem todos em 0 | `StartingWeapon.asset` (todos os níveis com `ProjectileSpeedBonus`, `LifetimeBonus`, `PierceBonus` 0); `WeaponStats.cs:35-37`, `Weapon.cs:56-58`, `SmithMenu.cs:92-97`. Trocar `*` por `+` em `Weapon.cs:56` ou esquecer `PierceBonus` na soma não derruba nenhum check | no walk de C9/C10 (ou C14, com a arma de teste), usar um nível com velocidade, alcance e perfuração não nulos e ler o projétil lançado e as linhas "Velocidade do projétil", "Alcance" e "Perfuração" do menu | `f0c08b5` - walk da rodada 2: arma de teste com velocidade, alcance e perfuração por nível; menu com as três linhas e projétil com velocidade 18, 2 acertos a mais e vida de ~3 s - C9 |
| F3 - `StartingWeapon` fora da lista `weapons` não dá erro, e então nenhuma arma é equipada | `WeaponCatalog.cs:19-24` procura só em `weapons`; `GameDatabase.cs` `OnValidate` só confere `StartingWeapon == null`. Exemplo próprio no Editor: catálogo com `startingWeapon` = Centelha e lista vazia, `Find("centelha")` devolve `null`. Cenário: o designer marca a inicial e esquece de pô-la na lista; save novo fica com `EquippedWeapon` "centelha", o `Sanitize` não acha e não troca, `PlayerAttack.Start` registra erro e o player não atira; o ferreiro mostra "Sem arma" | `OnValidate` do `GameDatabase` (ou do catálogo) registrar erro quando a inicial não está na lista ou está sem Id, com um check como o C20 | `7c96476` - `WeaponCatalog.Find` devolve a arma inicial mesmo fora da lista e o `OnValidate` avisa; conferido no Editor com a lista vazia - C13, C20 |
| F4 - Custo de nível em ouro ou óleo não aparece no menu, mas bloqueia a melhoria | `WeaponData.cs:22-23` (só um tooltip diz "só osso, ferro e cera"); `SmithMenu.cs:106-110` mostra só os três; `SmithShop.cs:28` usa `Covers` com os cinco recursos. Cenário: nível com `Gold: 5` e o jogador sem ouro: "Forjar" fica desligado sem nenhum material na cor de aviso (contraria SMITH-16) | o `OnValidate` do `WeaponData` avisar custo com ouro ou óleo (ou zerá-los), conforme SMITH-12 | `546afde` - `WeaponData.OnValidate` zera ouro e óleo de um nível, com aviso (o ferreiro só cobra osso, ferro e cera); conferido no Editor - C16 |
| F5 - C19 não registra a metade "na run" da prova | linha **Walked:** de `tasks.md`: o save com "nao-existe" é descrito só até o ferreiro; as runs registradas (C8 a C10, C17) partem de outros saves | refazer C19 até a run e registrar a arma "centelha" equipada e o dano 10 (ou o do nível salvo) no projétil | `f0c08b5` - walk da rodada 2: com "nao-existe" no save, a run equipou "centelha" no nível 0 com dano 10 - C19 |
