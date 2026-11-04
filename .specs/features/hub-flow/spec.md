# Hub funcional Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."

## Problem Statement

O hub existe como cena, mas não fecha o ciclo do jogo: não há como sair dele para uma run, ele não mostra os recursos do jogador e o jogo nem abre nele (a primeira cena do build é a `SampleScene`). Sem isso, não dá para testar o ciclo "run → acampamento → run" de que dependem o padre, a fogueira, a mesa de tarot e o ferreiro.

## Goals

- [ ] O jogo abre no hub, e o hub mostra os recursos do save.
- [ ] O jogador sai do hub para a run por um portão clicável, com confirmação.
- [ ] Voltar da run para o hub mostra os recursos já atualizados.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Escolha de mapa ou de dificuldade | Só existe um mapa e uma dificuldade no MVP. |
| Andar pelo hub | O One-Pager mantém o jogador parado ao lado do fogo. |
| Menus do padre, da fogueira, da mesa de tarot e do ferreiro | Cada um tem a sua feature. |
| Montar o deck | Feature `tarot-table`; aqui o portão só lê se o deck é válido. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude monta o portão, o painel e o contador de recursos | AD-001 | y |
| Saída para a run | Portão no fim da trilha de baixo, com painel de confirmação | AD-006 | y |
| Nome do mapa no painel | "Cemitério - Entrada" | O One-Pager sugere que o primeiro mapa é a entrada do cemitério | y |
| Arte do portão | Montada com peças do tileset do cemitério e do Graveyard Set, na paleta do hub | Mesmo estilo dos outros objetos do hub | y |
| Primeira cena do build | `MainMenu`, depois `Hub` e `FirstOne`; a `SampleScene` sai do build | AD-013 trouxe o menu principal, que leva ao hub; substitui o "abre no hub" | y |
| Esc fecha o painel | Sim | Atalho comum e sem custo | y |
| Como provar | Conferência manual em Play Mode | Escolha do usuário na feature anterior | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Partir para a expedição ⭐ MVP

**User Story**: Como jogador, quero clicar no portão do cemitério e confirmar a partida, para começar uma run a partir do acampamento.

**Why P1**: Sem saída, o ciclo do jogo não fecha.

**Acceptance Criteria**:

1. The cena `Hub` SHALL ter um portão clicável no fim da trilha de terra de baixo. [HUB-01]
2. WHEN o mouse passar sobre o portão THEN o hub SHALL mostrar o mesmo contorno dourado dos outros objetos clicáveis. [HUB-02]
3. WHEN o jogador clicar no portão THEN o hub SHALL abrir o painel "Partir para a expedição?" com o nome do mapa e o texto "Deck: N cartas", onde N é o número de cartas do deck salvo. [HUB-03]
4. WHEN o jogador clicar em "Partir" com um deck de 10 a 15 cartas THEN o jogo SHALL carregar a cena da run `FirstOne`. [HUB-04]
5. WHEN o jogador clicar em "Ficar" THEN o hub SHALL fechar o painel e continuar no acampamento. [HUB-05]
6. WHEN o jogador apertar Esc com o painel aberto THEN o hub SHALL fechar o painel. [HUB-06]
7. WHEN a cena da run carregar THEN a run SHALL começar com `Time.timeScale` igual a 1. [HUB-07]

**Independent Test**: Abrir o jogo, clicar no portão, confirmar e cair na run; numa segunda vez, clicar em "Ficar" e continuar no hub.

---

### P1: Recursos à vista no acampamento ⭐ MVP

**User Story**: Como jogador, quero ver meu ouro, óleo e materiais no acampamento, para saber o que posso comprar.

**Why P1**: Todas as compras do hub dependem desses números.

**Acceptance Criteria**:

1. The hub SHALL mostrar ouro, óleo, osso, ferro e cera do save, cada um com ícone e quantidade, o tempo todo. [HUB-08]
2. WHEN um recurso do save mudar THEN o hub SHALL mostrar o novo valor sem precisar recarregar a cena. [HUB-09]
3. WHEN o jogador voltar da run pelo botão "Voltar ao acampamento" THEN o hub SHALL mostrar os recursos com as recompensas dessa run já somadas. [HUB-10]

**Independent Test**: Terminar uma run com 15 de ouro entregue e ver o contador do hub subir 15.

---

### P1: O jogo abre no acampamento ⭐ MVP

**User Story**: Como jogador, quero que o jogo comece no acampamento, para decidir o que fazer antes de partir.

**Why P1**: Hoje o jogo abre numa cena de exemplo.

**Acceptance Criteria**:

1. The build SHALL listar as cenas `MainMenu`, `Hub` e `FirstOne`, nessa ordem, e o "Continuar" e o "Novo jogo" do menu SHALL levar à cena `Hub`. [HUB-11]
2. The lista de cenas do build SHALL não conter a `SampleScene`. [HUB-12]

**Independent Test**: Gerar um build, ver o menu principal e chegar ao acampamento pelo "Continuar".

---

## Edge Cases

- IF o deck salvo tiver menos de 10 ou mais de 15 cartas THEN o painel SHALL desativar o botão "Partir" e mostrar o texto "Monte um deck de 10 a 15 cartas na mesa de tarot.". [HUB-13]
- WHILE um painel do hub estiver aberto the hub SHALL ignorar cliques e contornos nos objetos do mundo. [HUB-14]
- WHEN o jogador clicar em "Partir" mais de uma vez THEN o jogo SHALL carregar a cena da run uma única vez. [HUB-15]

Dimensões implícitas: transições de estado (HUB-14, HUB-15) e validação (HUB-13) estão cobertas. Persistência é do `save-system`. Concorrência, autenticação, dependências externas e ciclo de vida N/A: navegação local entre duas cenas.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| HUB-01 | P1: Partir para a expedição | - | Pending |
| HUB-02 | P1: Partir para a expedição | - | Pending |
| HUB-03 | P1: Partir para a expedição | - | Pending |
| HUB-04 | P1: Partir para a expedição | - | Pending |
| HUB-05 | P1: Partir para a expedição | - | Pending |
| HUB-06 | P1: Partir para a expedição | - | Pending |
| HUB-07 | P1: Partir para a expedição | - | Pending |
| HUB-08 | P1: Recursos à vista no acampamento | - | Pending |
| HUB-09 | P1: Recursos à vista no acampamento | - | Pending |
| HUB-10 | P1: Recursos à vista no acampamento | - | Pending |
| HUB-11 | P1: O jogo abre no acampamento | - | Pending |
| HUB-12 | P1: O jogo abre no acampamento | - | Pending |
| HUB-13 | Edge Cases | - | Pending |
| HUB-14 | Edge Cases | - | Pending |
| HUB-15 | Edge Cases | - | Pending |

**Coverage:** 15 total, 0 mapped to tasks, 15 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - hub como acampamento com o jogador parado ao lado do fogo; primeiro mapa é a entrada do cemitério.
- `.specs/STATE.md` - AD-001, AD-004 e AD-006.
- `Assets/_Project/Scenes/Hub/Hub.unity` e `Assets/_Project/Code/Runtime/Hub/HubInteractable.cs` - hub atual com contorno e clique.
- `ProjectSettings/EditorBuildSettings.asset` - ordem atual: `SampleScene`, `Hub`, `FirstOne`.

---

## Success Criteria

- [ ] O ciclo hub → run → resultado → hub roda três vezes seguidas sem erro no Console.
- [ ] Os recursos do hub sempre batem com o save.
