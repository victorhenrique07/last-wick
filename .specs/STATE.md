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

### AD-013
- **Decision**: O jogo abre num menu principal (cena `MainMenu`, primeira do build) com Novo jogo, Continuar, Opções e Sair. Novo jogo com save existente pede confirmação e então começa um save novo; Continuar fica desabilitado sem save; Opções fica desabilitado até a tela existir. A arte vem das peças originais do Gothic Pixel UI, sem reaproveitar os sprites de `UI/Victory`. Isso substitui o "MVP abre direto no hub" do `hub-flow` e o "sem menu principal" do `save-system`.
- **Reason**: Usuário: "quero que você monte a cena do menu principal do jogo" e "Não é para criar esse menu principal utilizando as sprites que já existem na unity"; escolheu "Confirmar e apagar", "Aparece desabilitado", "Desabilitado" e "Claude escreve tudo".
- **Status**: active

## Handoff

**Updated**: 2026-11-03
**Feature**: run-rewards pausada em T9 de 12 (último commit `4239381`); o usuário pediu o menu principal antes de seguir
**Phase/Task**: menu principal (AD-013), feito pelo Claude; depois volta para run-rewards T10 (`RewardEntryView`, Claude)
**Completed**: run-rewards T1-T9. Bosses sem materiais no MVP (RWD-05 mudou). Ícones dos recursos em `Art/Sprites/UI/Victory/`.
**In progress**: menu principal - cena, script e arte
**Next step**: terminar o menu; depois T10, T11 e T12 do run-rewards e o fechamento (walk, check_proofs, Verifier)
**Blockers**: nenhum
**Uncommitted**: nada além das pastas de ferramentas (`.agents/`, `.claude/`, `.cursor/`, `.windsurf/`)
**Branch**: main, sem push
**Como trabalhamos**: AD-001 - o usuário escreve o código das tasks marcadas "usuário"; o agente explica, revisa, compila, marca a task e commita uma por task, em português, no estilo do repo. Com a Unity aberta, o agente confere e monta cenas pelo Editor via `unity command` (script eval, console status); com ela fechada, compila em batch mode.
