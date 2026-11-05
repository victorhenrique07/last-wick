# priest-stats proofs

**HEAD**: a35455a
**Tree**: clean
**Shell**: C:/Program Files/Git\bin\bash.exe
**Result**: 28 pass, 0 fail

| Check | Proof | Result |
| --- | --- | --- |
| C1 | `walk - Clicar na igreja no hub.` | pass (Walked) |
| C2 | `walk - Num save novo, ler "Nível 0" e custo 5.` | pass (Walked) |
| C3 | `walk - Ler "Vitalidade Nv 0" e "Vida máxima 100 → 110"; ler as 5 linhas da Agilidade.` | pass (Walked) |
| C4 | `walk - Com 20 de ouro, comprar Vitalidade e ver 15.` | pass (Walked) |
| C5 | `walk - Ler PriestLevels no save.json com o ID da Vitalidade em 1.` | pass (Walked) |
| C6 | `walk - Comprar Vitalidade e Força e ler "Nível 2".` | pass (Walked) |
| C7 | `walk - Com 20 de ouro, comprar Vitalidade (5) e Força (9): sobram 6 e o próximo custa 16 em todos.` | pass (Walked) |
| C8 | `walk - Depois de comprar Vitalidade, ler "Vida máxima 110 → 120" e o custo 9.` | pass (Walked) |
| C9 | `walk - Fechar pelo botão; reabrir e apertar Esc no Game view.` | pass (Walked) |
| C10 | `walk - Com pontos em cada atributo, começar uma run e ler os status do PlayerRunStats.` | pass (Walked) |
| C11 | `walk - Com Vitalidade 3, começar a run com 130 de vida máxima.` | pass (Walked) |
| C12 | `walk - Com Força 2, ver CalculateDamage da arma 2 acima do de antes.` | pass (Walked) |
| C13 | `walk - Com Agilidade 5, ler velocidade +0,25, esquiva +0,25, recarga −0,2 s, cadência ×1,1 e coleta +0,25; a v` | pass (Walked) |
| C14 | `walk - Com Resistência 10, ler defesa 0,183 (1 − 0,98^10) no PlayerRunStats.` | pass (Walked) |
| C15 | `walk - Com Vitalidade 3 e uma carta de +10% de vida, ler 143.` | pass (Walked) |
| C16 | `walk - Com Vitalidade 3, terminar a run e ler 130 em "Vida máxima".` | pass (Walked) |
| C17 | `walk - Com Resistência 10, um golpe de 10 tira 8,17 de vida.` | pass (Walked) |
| C18 | `walk - Sem Resistência, um golpe de 10 tira 10; com Resistência 30, a defesa é 0,455 (1 − 0,98^30), e não 0,6.` | pass (Walked) |
| C19 | `walk - Ler "Defesa 0% → 2%" na Resistência.` | pass (Walked) |
| C20 | `walk - Abrir os 4 assets no Inspector.` | pass (Walked) |
| C21 | `walk - Criar um atributo de teste, pôr na lista e vê-lo no menu; depois removê-lo.` | pass (Walked) |
| C22 | `walk - Pôr um efeito de vida na Força de teste e ver a vida subir na run e no menu; depois desfazer.` | pass (Walked) |
| C23 | `walk - Com 0 de ouro, abrir o menu e ver os 4 botões desligados.` | pass (Walked) |
| C24 | `walk - No mesmo teste, ler a cor do custo.` | pass (Walked) |
| C25 | `walk - Com 20 de ouro, clicar 5 vezes em Vitalidade: 2 pontos e 6 de ouro.` | pass (Walked) |
| C26 | `walk - Com Agilidade 50, ler 0,2 s de recarga.` | pass (Walked) |
| C27 | `walk - Com Resistência 100, ler defesa 0,867 e um golpe de 10 tirando 1,33; o ganho do ponto 100 é menor que o` | pass (Walked) |
| C28 | `walk - Pôr no save um nível 3 de um ID inexistente e ler "Nível 3" e custo 29, sem efeito na run.` | pass (Walked) |
