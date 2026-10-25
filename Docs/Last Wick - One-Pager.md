# Last Wick · One-Pager

22/10/2026 · Victor Henrique Silva Galvão

Este arquivo é a versão atual do design. O `OnePager.pdf` desta pasta é de uma versão anterior (antes dos drops e das decisões de balanceamento) e não deve ser usado.

## Pitch

**Um roguelike de sobrevivência onde você monta seu deck de habilidades antes de cada expedição e enfrenta 10 minutos de hordas e chefes, voltando à base para forjar armas e fortalecer sua chama.**

| | |
| --- | --- |
| Gênero | Roguelike survivors-like com meta-progressão |
| Tema | Cemitério; o jogador é uma pequena luz no escuro |
| Plataforma | PC, mouse e teclado |
| Modo | Solo |
| Sessão | Uma run = um mapa = até 10 minutos, bosses incluídos |
| Referências | Vampire Survivors e Brotato (a run), Hades (o hub e a meta-progressão), Diablo (as dificuldades do mundo), soulslikes (esquiva e subida de status ponto a ponto) |

**Diferencial:** visão limitada pela luz, que cresce com cartas e na fogueira, e um deck montado antes de cada partida.

## Pilares de design

Toda decisão nova deve servir a pelo menos um destes pilares. *Proposta inicial, para validar.*

1. **O escuro é a ameaça.** A luz não é a vida do personagem (ele tem vida própria), mas enxergar mais é tão valioso quanto bater mais forte. O escuro é a principal fonte de tensão.
2. **Cada run deixa algo.** Vencendo ou morrendo, o jogador volta à fogueira mais perto do próximo desbloqueio.
3. **O build começa antes da partida.** O deck montado no hub define a estratégia; a run decide como executá-la.

## Core loop

O ciclo gira entre a fogueira e a run: o que se coleta na run vira progresso permanente no hub.

## A run

Uma run dura até 10 minutos, e o relógio nunca para, nem durante os bosses. Matar inimigos enche uma barra única com 2 marcos de boss e o boss final no fim. Quem mata rápido termina antes.

1. **Hordas.** Monstros perseguem o jogador o tempo todo. Cada abate soma pontos na barra (cada tipo de inimigo vale um número diferente de pontos), dá XP (nível da run) e pode soltar material.
2. **Marco de boss.** A barra tem 2 marcos no meio, e cada trecho entre marcos é mais difícil que o anterior (mais tipos de inimigo, inimigos mais fortes e spawn mais rápido). Ao atingir um marco, param de nascer monstros e os que sobraram vêm até o jogador. Esses abates não contam para a barra. O boss só aparece quando a arena estiver limpa.
3. **Luta com o boss.** Só o boss fica na arena, e o relógio continua correndo.
4. **Boss final.** Quando a barra completa, vale a mesma regra dos marcos: param os spawns, a arena precisa ser limpa e só então ele aparece. É o mais forte em vida e dano. Ao matá-lo, a run termina na hora.
5. **Tela de resultado.** Estatísticas e recompensas da partida.

**Limite de 10:00:** se o boss final não tiver morrido, a escuridão consome o jogador e a run falha (conta como morte). Isso vale também no meio de uma luta com boss.

**Recompensas**

- Monstros comuns: XP, materiais e cura (detalhes em Drops).
- Bosses: são únicos, com ataques próprios (não são versões ampliadas dos inimigos comuns). Dropam XP, materiais, **ouro** (única fonte de ouro, usado no padre) e uma quantidade garantida de óleo.
- Vitória: o jogador fica com 100% do que coletou, mais um bônus sobre tudo (materiais, ouro e óleo) que cresce quanto mais cedo terminar antes dos 10:00.
- Morte, saída no meio ou escuridão aos 10:00: perde metade de tudo (materiais, ouro e óleo).
- Completar o mapa desbloqueia o próximo, mais difícil e com novos desafios. Por enquanto, não dá para rejogar mapas anteriores.

## Drops

Todo drop cai no chão e é coletado automaticamente ao encostar; os materiais são gerais, iguais em todos os mapas.

| Fonte | Drops | Uso |
| --- | --- | --- |
| Monstros comuns | XP | Nível da run (escolha de cartas) |
| Monstros comuns | Materiais gerais | Ferreiro e mago |
| Monstros comuns | Cura | Recupera a vida do personagem na hora |
| Bosses | XP | Nível da run (escolha de cartas) |
| Bosses | Materiais gerais | Ferreiro e mago |
| Bosses | Ouro | Padre (única fonte) |
| Bosses | Óleo garantido | Fogueira (luz) |
| Escuro do mapa | Óleo extra | Fogueira (luz) |

Com a visão curta, drops longe do jogador ficam no escuro: buscá-los é um risco que o jogador escolhe correr.

**Orbes de óleo:** nascem em posições aleatórias do mapa a cada run e são a única fonte de óleo fora dos bosses (cerca de 40 dos ~150 de uma vitória). O óleo vai para a fogueira do hub, que sobe a luz. Os orbes servem para recompensar quem se arrisca a explorar o escuro, em vez de só ficar parado matando hordas. Cada orbe é um ponto de luz fraco e pulsante, visível em qualquer lugar da tela mesmo no escuro, mas que não ilumina o que está em volta: o jogador vê aonde ir, mas não vê o que o espera no caminho.

**Fora por enquanto:** raridades, monstros de elite e drops temporários (baús, ímã, bombas).

## Combate e cartas

O ataque é 100% manual: o jogador controla movimento, ataque e esquiva, e define o build pelas cartas.

**Combate**

- **Ataque:** manual. O jogador mira com o mouse e segura o botão para atacar. A arma define o ataque principal (no MVP, uma arma inicial fixa, que o ferreiro melhora).
- **Cartas de ataque:** as que somam ataques extras disparam sozinhas; as que mudam a arma principal seguem o ataque manual dela. As demais cartas modificam o build.
- **Esquiva:** cooldown de 2 s, 0,5 s de invencibilidade e 2 a 3 unidades de distância, no estilo dos soulslikes da FromSoftware.
- **Visão:** raio pequeno ao redor do jogador. Aumenta de forma permanente na fogueira, gastando óleo (garantido nos bosses e extra escondido no escuro do mapa; não usa os pontos de status) e durante a run com cartas. Cada nível de luz pede mais óleo. No MVP, não diminui durante a run.
- **Inimigos no escuro:** fora da luz aparecem só como silhuetas, que vão sumindo de forma linear com a distância: 100% de opacidade na borda da luz até 0% a 3 unidades além dela. Assim os inimigos surgem aos poucos do escuro, sem parecer que nasceram do lado do jogador.

**Deck**

- Montado no hub antes de cada run, só com cartas padrão e cartas já desbloqueadas no mago.
- O jogador leva entre 10 e 15 cartas, sem repetição no deck.
- Ao subir de nível na run, aparecem 3 opções, todas tiradas desse deck.
- Uma carta já escolhida pode aparecer de novo como opção, e escolhê-la sobe o nível dela. Por enquanto, as cartas não têm nível máximo.
- Os efeitos das cartas duram até o fim da run.

| Tipo de carta | Função |
| --- | --- |
| Ataque | Novos ataques ou mudanças no ataque |
| Passiva | Efeitos contínuos durante a run |
| Status | Aumento de vida, defesa, dano etc. até o fim da run |
| Luz | Aumento do raio de visão até o fim da run |

**Dois níveis diferentes**

- **Nível da run:** sobe com XP dos monstros e dos bosses, dá escolha de cartas e reseta ao fim da run.
- **Nível permanente:** comprado no padre só com ouro, um ponto de status por vez.

## Hub e meta-progressão

O hub é um acampamento com uma fogueira no centro. O jogador fica parado ao lado do fogo e clica nos NPCs (sem andar, por enquanto).

| NPC | O que faz | Usa | Tetos de desbloqueio |
| --- | --- | --- | --- |
| Padre | Sobe status base permanentes, um ponto por vez (vida, defesa, ataque etc.). A luz sobe à parte, na fogueira | Só ouro (dos bosses) | Passar de certos níveis exige ter alcançado o mapa X naquela dificuldade. No MVP, sem teto |
| Mago | Desbloqueia novas cartas para o deck | Materiais | Nenhum: só o custo em materiais |
| Ferreiro | Melhora a arma (no MVP); cria novas armas (depois) | Materiais | Por definir |
| Fogueira | Sobe a luz permanentemente; cada nível pede mais óleo | Óleo | Nível máximo de luz (ver Balanceamento) |

**Por que o teto do padre existe:** evita que o jogador fique farmando um mapa fácil para subir muito de nível. O ouro e o mapa alcançado são as travas.

**Tetos por dificuldade:** cada dificuldade tem seus próprios tetos. Exemplo ilustrativo (números a balancear):

| Dificuldade | Mapa alcançado | Teto do padre |
| --- | --- | --- |
| 1 | 5 | Nível 10 |
| 1 | 10 | Nível 20 |
| 2 | 5 | Nível 30 |

No futuro, novas classes jogáveis.

## Progressão do mundo

O objetivo é chegar ao último dos 10 mapas; depois, o jogador pode subir de dificuldade, no estilo dos níveis de Tormento de Diablo.

- **10 mapas** em sequência, cada um mais difícil e com novos desafios. Ideia: cada mapa é uma área do cemitério (entrada, capela, criptas, mausoléus, pântano…).
- **Tamanho do mapa:** grande, com limites distantes. Não é infinito, mas também não é uma arena fechada.
- **Dificuldades do mundo** (pós-MVP). A dificuldade é escolha do jogador: depois de terminar uma, ele pode subir para a próxima ou continuar na atual. Ao subir de dificuldade:
  - Inimigos e bosses ganham mais vida e dano.
  - Os modificadores continuam os mesmos.
  - O jogador pode continuar subindo nível no padre (novos tetos).
  - Os drops ficam um pouco melhores.
  - A luz sofre uma penalidade automática, como a penalidade de resistências ao subir de dificuldade em Diablo. O nível de luz comprado na fogueira é um só e nunca muda; a penalidade é aplicada por cima dele, no raio. Por isso, ao voltar para uma dificuldade menor, a luz volta sozinha ao valor daquela dificuldade.
- **Recomendação para o teto da luz** *(a confirmar)*: como nas resistências de Diablo, o raio máximo continua 8,0 em todas as dificuldades, mas o teto de nível da fogueira sobe 1 a cada dificuldade desbloqueada (nível máximo = 10 + dificuldade − 1). Na dificuldade 2, o jogador precisa do nível 11 para voltar ao raio 8,0. Nas dificuldades menores, o nível extra não passa do raio 8,0: ele só faz diferença onde há penalidade.
- **Recomendação para rejogar mapas** *(a confirmar)*: no MVP não há replay (só existe 1 mapa). Depois do MVP, o hub mostra os mapas já completados em cada dificuldade e o jogador escolhe qual jogar. Isso é necessário para "continuar na dificuldade 1" depois de terminá-la, e o teto do padre já impede que ele farme um mapa fácil para subir de nível sem limite.

## Escopo do MVP

O MVP responde a uma pergunta: o núcleo (10 minutos de hordas com visão limitada e cartas) é divertido?

| Dentro do MVP | Fora do MVP |
| --- | --- |
| 1 classe | Outras classes |
| 1 mapa, com bosses e boss final | Mapas 2 a 10 e biomas |
| ~10 cartas (mais serão adicionadas) | Mago e desbloqueio de cartas |
| Ataque manual, esquiva, visão fixa | Visão que diminui durante a run |
| Padre e ouro dos bosses, sem teto | Criação de novas armas no ferreiro |
| Ferreiro melhorando a arma inicial (usa os materiais) | |
| Hub como menu ao lado da fogueira | Hub explorável |
| Dificuldade única | Dificuldades do mundo |
| Materiais gerais, cura e XP no chão | Raridades, elites e drops temporários |

## Balanceamento inicial

Valores **provisórios**, para o primeiro protótipo. Todos vão para ScriptableObjects (o balanceamento é no Inspector) e mudam depois do primeiro playtest. A fórmula do bônus por tempo fica com o Victor.

**Escala:** distâncias em unidades Unity. A câmera é ortográfica com size 5, ou seja, a tela mostra 10 unidades de altura (cerca de 17,8 de largura em 16:9). Um raio de 3,0 ilumina pouco mais da metade da altura da tela.

**Pontos por inimigo**

| Inimigo | Pontos na barra |
| --- | --- |
| Soul | 10 |
| Ghoul | 15 |
| Mortician | 25 |
| Revenant | 40 |

**Inimigos por trecho**

Alavancas de dificuldade de cada trecho: tipos de inimigo, stats e taxa de spawn. A mistura abaixo é o peso de spawn de cada inimigo no trecho.

| Trecho | Soul | Ghoul | Mortician | Revenant | Média de pontos por abate |
| --- | --- | --- | --- | --- | --- |
| 1 (até o Boss 1) | 60% | 40% | | | 12 |
| 2 (até o Boss 2) | 45% | 35% | 20% | | ~15 |
| 3 (até o boss final) | 35% | 30% | 20% | 15% | 19 |

**Barra de progresso (pontos)**

| Marco | Pontos acumulados | Trecho | Abates aproximados no trecho |
| --- | --- | --- | --- |
| Boss 1 | 3000 | 3000 | ~250 |
| Boss 2 | 7500 | 4500 | ~300 |
| Boss final | 14000 | 6500 | ~350 |

Os marcos foram convertidos dos 250/550/900 abates originais usando a média de pontos de cada trecho, então uma run continua pedindo uns 900 abates no total.

As hordas nascem sem parar até a barra atingir o próximo marco. Não há divisão de tempo entre hordas e bosses: quem mata devagar pode levar os 10 minutos inteiros para chegar ao primeiro marco, e aí a escuridão o consome. Os marcos só definem quanto se precisa matar; o relógio decide se dá tempo.

**Materiais gerais**

| Material | Origem temática |
| --- | --- |
| Osso | Restos dos mortos |
| Ferro | Grades, pregos e correntes do cemitério |
| Cera | Velas e círios, ligada ao tema da chama |

Os três caem com a mesma chance, sorteados no drop de material.

**Ouro dos bosses**

| Boss | Ouro |
| --- | --- |
| Boss 1 | 10 |
| Boss 2 | 20 |
| Boss final | 40 |

Uma vitória rende 70 de ouro antes do bônus de tempo.

**Luz e óleo**

- Raio de visão fixo na run: 3,0 no nível 0, +0,5 por nível de luz, até o nível 10 (raio 8,0).
- Custo em óleo do nível *n*: `40 + 20 × (n − 1)`, ou seja 40, 60, 80… até 220 no nível 10. Chegar ao máximo custa 1300 de óleo.
- Óleo por run: boss de marco 30 cada, boss final 50, mais 8 orbes de 5 escondidos no escuro (40). Uma vitória completa rende cerca de 150, então o máximo leva umas 9 vitórias.
- Pós-MVP: cada dificuldade acima da 1 tira 0,5 de raio, de forma cumulativa, com raio mínimo de 2,0 e máximo de 8,0. O nível de luz nunca fica negativo; a penalidade só reduz o raio. Fórmula: `raio = min(8,0; max(2,0; 3,0 + 0,5 × nível − 0,5 × (dificuldade − 1)))`, com nível máximo de `10 + (dificuldade desbloqueada − 1)` (recomendação a confirmar).

**Esquiva**

- Cooldown: 2 s.
- Invencibilidade: 0,5 s.
- Distância: 2 a 3 unidades.

**Drops dos monstros comuns**

| Drop | Chance por abate | Quantidade |
| --- | --- | --- |
| XP | 100% | Valor do inimigo (`expOnDeath`) |
| Material geral | 20% | 1, do tipo sorteado entre os materiais gerais |
| Cura | 4% | 10% da vida máxima do personagem |

Com cerca de 900 abates isso dá uns 180 materiais e 36 curas por run, o equivalente a uns 360% da vida máxima ao longo dos 10 minutos. A cura é a válvula de escape da vida, então se as hordas finais ficarem pesadas demais, a chance sobe antes de mexer em outra coisa.

## Questões em aberto

**Decididas**

- [x] Tetos de desbloqueio: o mago não tem teto; o padre não tem teto no MVP; a fogueira tem nível máximo; o ferreiro fica por definir.
- [x] Inimigos comuns: só os que já existem no projeto (Soul, Ghoul, Mortician, Revenant), distribuídos por trecho (ver Balanceamento inicial).
- [x] Bosses: únicos, com ataques próprios, sem material exclusivo por enquanto.
- [x] Luz tirada por dificuldade e óleo por nível de luz (ver Balanceamento inicial).
- [x] Chance de drop e quanto cada cura recupera (ver Balanceamento inicial).
- [x] Padre usa só ouro; ouro por boss: 10, 20 e 40.
- [x] Relógio não congela nos bosses; o boss final também espera a arena limpa.
- [x] Ataque manual com mira no mouse, segurando o botão; cartas de ataque extra disparam sozinhas.
- [x] Esquiva: 2 s de cooldown, 0,5 s de invencibilidade, 2 a 3 unidades.
- [x] Silhuetas somem de forma linear até 3 unidades além da luz.
- [x] Pontos na barra: Soul 10, Ghoul 15, Mortician 25, Revenant 40; marcos em 3000, 7500 e 14000.
- [x] Hordas nascem sem parar até cada marco, sem divisão de tempo entre hordas e bosses.
- [x] Cartas: 3 opções por nível, repetir sobe o nível da carta, sem nível máximo, efeitos até o fim da run.
- [x] Ferreiro entra no MVP para melhorar a arma inicial; é ele quem dá uso aos materiais.
- [x] Materiais gerais: Osso, Ferro e Cera.
- [x] Orbes de óleo em posições aleatórias a cada run, visíveis como um ponto de luz fraco e pulsante.
- [x] Sem rejogar mapas anteriores, por enquanto.

**Abertas**

- [ ] Fórmula do bônus por tempo (depois).
- [ ] Quais são as cartas padrão (Victor vai adicionar).
- [ ] Curva de XP e quantos níveis uma run rende.
- [ ] Custo de cada ponto no padre.
- [ ] Ferreiro: quais upgrades de arma existem, quanto custam e se têm teto (depois).
- [ ] Origem da estimativa de 1000 a 1300 abates em 10 minutos ("a noite atual", Victor vai explicar).
- [ ] Design dos 3 bosses e seus ataques.

**Pós-MVP**

- [ ] Confirmar a recomendação do teto da luz (raio máximo fixo em 8,0, teto de nível sobe 1 por dificuldade).
- [ ] Confirmar a recomendação de rejogar mapas já completados, escolhidos no hub.
- [ ] Testar se a visão diminui durante a run.
