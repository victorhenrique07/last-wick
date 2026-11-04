# Hub funcional verification

**Verdict**: FAIL
**Proofs**: `proofs.md` at aabd3f2
**Verifier**: independent sub-agent (author != verifier)

## Checks

| Check | Test | Holds |
| --- | --- | --- |
| C1 | walk - portão `Portao (Expedicao)` em (0.5, -5.2), na ponta da trilha de baixo, `Logs/hub_preview.png` | yes |
| C2 | walk - raycast do `EventSystem` no ponto do portão acerta o portão; pointer enter liga o `Contorno` e o exit desliga | yes - o enter/exit foi disparado pela CLI, mas o raycast real mostra que o portão é o alvo do mouse; as duas partes juntas falham se o collider ou o contorno estiverem errados |
| C3 | walk - clique no portão abre o painel com "Cemitério - Entrada", "Deck: 15 cartas", sem aviso e "Partir" ligado (`Logs/hub_walk.png`) | yes - passa pelo listener persistente `onClick` → `ExpeditionPanel.Open` serializado em `Hub.unity` |
| C4 | walk - "Partir" carregou a `FirstOne` | yes |
| C5 | walk - "Ficar" fecha o painel | yes |
| C6 | walk - o usuário apertou Esc no Game view com o painel aberto: "funciona." | yes - entrada real pelo Input System |
| C7 | walk - com `timeScale` 0 antes de "Partir", a run começou com `timeScale` 1 | yes - parte de `timeScale` 0, que é o caso que falharia sem o `Time.timeScale = 1f` do `Depart` |
| C8 | walk - barra com Ouro 5, Óleo 15, Osso 13, Ferro 17, Cera 13, igual ao save | yes |
| C9 | walk - `SaveSystem.Add` de 15 de ouro levou a barra de 5 a 20 na hora | yes - `Add` usa o mesmo `NotifyChanged` que `TrySpend` e `StartNew` |
| C10 | walk - vitória e "Voltar ao acampamento": barra com Ouro 75 e Óleo 125 (+70, +110) | yes |
| C11 | walk - Build Settings com `MainMenu`, `Hub`, `FirstOne` nos índices 0-2; "Continuar" carregou a `Hub` | no - o critério também exige que o "Novo jogo" leve à `Hub`, e esse caminho não foi percorrido |
| C12 | walk - Build Settings sem `SampleScene` | yes |
| C13 | walk - deck de 9 cartas: "Deck: 9 cartas", aviso e "Partir" desligado | no - o critério nomeia "menos de 10 ou mais de 15"; só o lado de baixo foi exercitado, o de cima (16+) não |
| C14 | walk - com o painel aberto, o raycast no portão e na igreja acerta o fundo `Conteudo`, e a igreja não ganha contorno | yes |
| C15 | walk - três cliques seguidos em "Partir" carregaram a `FirstOne` uma vez, Console sem erros | yes - os três cliques caem no mesmo frame, antes da troca de cena, que é o único momento em que um segundo clique real pode chegar |

## Findings

| Finding | Evidence | Fix | Closed |
| --- | --- | --- | --- |
| C11 não prova o "Novo jogo" → `Hub` que HUB-11 exige | `**Walked:**` só registra "Continuar carregou a cena `Hub`"; o código (`MainMenu.StartNewGame` → `LoadScene(hubSceneName)`, `hubSceneName: Hub` em `MainMenu.unity`) está certo, mas nenhuma prova o cobre | percorrer também o "Novo jogo" (com e sem save, confirmando) e registrar que chega à `Hub` | |
| C13 não exercita o deck acima de 15 cartas que HUB-13 nomeia | `**Walked:**` só registra o deck de 9 cartas; a regra `count <= deck.MaxCards` (`ExpeditionPanel.cs:29`, `maxCards: 15` no asset) não foi vista falhar com 16 | repetir o passo com 16 cartas em memória e registrar o aviso e o "Partir" desligado | |
