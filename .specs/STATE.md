# STATE

## Decisions

### AD-001
- **Decision**: Nas features do MVP (save, recompensas, hub, padre, fogueira, mesa de tarot, ferreiro), o usuário escreve o código de gameplay e o Claude monta as telas, a arte e a hierarquia no Unity.
- **Reason**: Pergunta "Quem escreve o código desses sistemas?"; o usuário escolheu "Claude: telas; eu: código".
- **Status**: active

### AD-002
- **Decision**: Os materiais dos monstros são sorteados no abate, sem item no chão, e o total entra no fim da run, já com a perda de 50% na morte. Isso substitui a regra do One-Pager de que os drops caem no chão.
- **Reason**: Pergunta "Como os materiais dos monstros devem funcionar?"; o usuário escolheu "Sorteados no abate, somados no fim".
- **Status**: active

### AD-003
- **Decision**: No MVP o ferreiro só melhora a arma atual, mas cada arma é um dado (`WeaponData`) e adicionar uma arma nova não pede mudança de código.
- **Reason**: Usuário: "apenas a arma atual, mas, o código deve ser escrito de um jeito que funcione normalmente com mais armas no jogo. Deve ser fácil de adicionar uma arma nova no jogo."
- **Status**: active

### AD-004
- **Decision**: A mesa de tarot reúne tudo sobre cartas: desbloquear, ler, ver a coleção e montar o deck de 10 a 15 cartas antes de cada run.
- **Reason**: Usuário escolheu "Tela de montar deck no hub" e disse "a mesa de cartas engloba tudo sobre cartas. Desbloquear cartas, ler cartas, coleção de cartas e montar o deck."
- **Status**: active

### AD-005
- **Decision**: A fogueira (nível de luz comprado com óleo) e os orbes de óleo no escuro entram no MVP.
- **Reason**: Usuário: "entra no MVP. esqueci desse ponto."
- **Status**: active

### AD-006
- **Decision**: A saída do hub para a run é um portão de cemitério clicável no fim da trilha de baixo, que abre um painel de confirmação.
- **Reason**: Usuário delegou: "você decide como fazer isso. fique a vontade para criar telas, se for preciso."
- **Status**: active

### AD-007
- **Decision**: Toda chave persistida de carta e de arma é um ID próprio do asset (campo `Id`), nunca o nome do arquivo.
- **Reason**: Renomear um asset não pode apagar o progresso salvo; decorre do pedido de que adicionar armas e cartas seja fácil.
- **Status**: active

### AD-008
- **Decision**: Todo progresso persistente passa pela classe estática `SaveSystem`, que carrega `save.json` na primeira leitura e guarda uma única instância de `SaveData`. Nenhuma outra classe lê ou escreve o arquivo.
- **Reason**: Design do `save-system`: funciona em qualquer cena sem objeto de bootstrap (SAVE-09).
- **Status**: active

### AD-009
- **Decision**: Os assets que precisam ser achados por ID (cartas, deck inicial, armas, status do padre) ficam listados no `GameDatabase`, carregado de `Resources/GameDatabase.asset`.
- **Reason**: Design do `save-system`: um ponto só para resolver os IDs do save (AD-007).
- **Status**: active

### AD-010
- **Decision**: Recursos e custos usam sempre o tipo `ResourceBag` (ouro, óleo, osso, ferro, cera). Gastar passa por `SaveSystem.TrySpend`.
- **Reason**: Design do `save-system`: recompensas, padre, fogueira, mesa de tarot e ferreiro usam a mesma conta.
- **Status**: active

### AD-011
- **Decision**: Todo menu e painel do hub usa o componente `HubPanel`: um painel aberto por vez, Esc fecha, e um fundo em tela cheia bloqueia os cliques no mundo.
- **Reason**: Design do `hub-flow`: o portão, o padre, a fogueira, a mesa de tarot e o ferreiro abrem painéis com o mesmo comportamento.
- **Status**: active

### AD-012
- **Decision**: Os status base da run são montados em `PermanentStats.Build` (`PlayerStats.asset` + pontos do padre + luz da fogueira). As cartas se aplicam depois, no `PlayerRunStats`.
- **Reason**: Design do `priest-stats` e do `campfire-light`: uma ordem única de aplicação para todo bônus permanente.
- **Status**: active

## Handoff
