# Sistema de save Specification

**Approved**: 2026-11-03 - user: "tudo ok. Podemos seguir com o design das specs."

## Problem Statement

Hoje nada sobrevive ao fim de uma run: ouro, materiais, óleo, níveis do padre, da fogueira e da arma, cartas liberadas e o deck não existem fora da memória. Sem isso não há meta-progressão, que é o pilar "Cada run deixa algo" do One-Pager. Todas as outras features do MVP leem e escrevem nesse save, então ele vem primeiro.

## Goals

- [ ] Todo o progresso permanente do MVP fica num único save, lido ao abrir o jogo e gravado nos momentos em que muda.
- [ ] Fechar e abrir o jogo devolve exatamente o último estado gravado.
- [ ] Um save ausente, corrompido ou com IDs desconhecidos nunca impede o jogo de abrir.

## Out of Scope

| Feature | Reason |
| ------- | ------ |
| Vários slots de save | O MVP tem um jogador e um progresso; vira pedido próprio se precisar. |
| Save na nuvem | Sem serviço online no MVP. |
| Criptografia ou proteção contra edição | Jogo solo, sem competição; não muda o MVP. |
| Opções de configuração (volume, resolução) | Não são progresso; feature separada. |
| Tela de apagar save ou "novo jogo" para o jogador | O MVP não tem menu principal; o apagar existe só no Editor (P2). |
| Migração entre versões do save | Só existe a versão 1; o campo de versão deixa a porta aberta. |
| Salvar no meio de uma run | O progresso só muda no fim da run e no hub. |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Quem implementa | O usuário escreve o código; o Claude orienta | AD-001 | y |
| Quantos saves | Um único save | Jogo solo, sem menu principal | y |
| Formato e local | JSON em `Application.persistentDataPath/save.json` | Legível para depurar e sem dependência externa | y |
| Quando grava | No fim de cada run e logo após cada compra ou mudança de deck no hub | Nenhum progresso fica só na memória | y |
| Save corrompido | Renomeia para `save.corrupt.json`, cria um save novo e registra um aviso | O jogo abre e o arquivo ruim fica guardado para análise | y |
| ID desconhecido no save (carta ou arma removida) | Ignora esse ID, mantém o resto e registra um aviso | Remover um asset não pode apagar o progresso todo | y |
| Estado inicial | Moedas e níveis em 0, cartas padrão liberadas, deck padrão, arma inicial equipada | Começo descrito no One-Pager | y |
| Cartas padrão e deck padrão | As 15 cartas do `Deck.asset` atual; as outras 5 começam bloqueadas | É o deck que o level up já usa hoje | y |
| Como provar | Conferência manual em Play Mode, como na feature de ondas | Escolha do usuário na feature anterior | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Progresso guardado entre sessões ⭐ MVP

**User Story**: Como jogador, quero que meu ouro, materiais, óleo, níveis, cartas e deck continuem lá quando eu fechar e abrir o jogo, para que cada run conte.

**Why P1**: Sem save não existe meta-progressão.

**Acceptance Criteria**:

1. The save SHALL guardar ouro, óleo, osso, ferro e cera como números inteiros. [SAVE-01]
2. The save SHALL guardar o nível de cada status do padre, pelo ID do status. [SAVE-02]
3. The save SHALL guardar o nível de luz da fogueira. [SAVE-03]
4. The save SHALL guardar a lista de IDs das cartas liberadas. [SAVE-04]
5. The save SHALL guardar a lista de IDs das cartas do deck, na ordem em que o jogador montou. [SAVE-05]
6. The save SHALL guardar o nível de cada arma, pelo ID da arma. [SAVE-06]
7. The save SHALL guardar o ID da arma equipada. [SAVE-07]
8. The save SHALL guardar um número de versão, que começa em 1. [SAVE-08]
9. The save SHALL ser uma única instância, a mesma lida pelo hub e pela run. [SAVE-09]

**Independent Test**: Mudar ouro e deck por um script de teste, fechar o Play Mode, abrir de novo e ver os mesmos valores.

---

### P1: Carregar e gravar nos momentos certos ⭐ MVP

**User Story**: Como jogador, quero que o jogo grave sozinho, para nunca perder uma compra ou o resultado de uma run.

**Why P1**: Não existe botão de salvar; a gravação automática é a única forma de persistir.

**Acceptance Criteria**:

1. WHEN o jogo abrir e não existir arquivo de save THEN o save SHALL começar com moedas em 0, todos os níveis em 0, as cartas padrão liberadas, o deck padrão e a arma inicial equipada. [SAVE-10]
2. WHEN o jogo abrir e existir arquivo de save THEN o save SHALL carregar os valores do arquivo. [SAVE-11]
3. WHEN uma run terminar THEN o save SHALL gravar o arquivo. [SAVE-12]
4. WHEN o jogador fizer uma compra no hub THEN o save SHALL gravar o arquivo. [SAVE-13]
5. WHEN o jogador mudar o deck THEN o save SHALL gravar o arquivo. [SAVE-14]
6. The save SHALL gravar primeiro num arquivo temporário e só então substituir `save.json`. [SAVE-15]

**Independent Test**: Jogar uma run até o fim, fechar o jogo e abrir: o ouro mostrado no hub é o da tela de resultado.

---

### P2: Apagar o save no Editor

**User Story**: Como desenvolvedor, quero apagar o save por um menu do Editor para testar o começo do jogo.

**Why P2**: Acelera os testes, mas o jogador não precisa.

**Acceptance Criteria**:

1. WHERE o jogo roda no Editor the menu "Last Wick/Apagar save" SHALL apagar `save.json`. [SAVE-16]

**Independent Test**: Usar o menu, entrar em Play Mode e ver o estado inicial.

---

## Edge Cases

- IF o arquivo de save não puder ser lido como um save válido THEN o save SHALL renomeá-lo para `save.corrupt.json`. [SAVE-17]
- IF o arquivo de save não puder ser lido como um save válido THEN o save SHALL começar no estado inicial de SAVE-10 e registrar um aviso no Console. [SAVE-18]
- IF o save tiver um ID de carta ou de arma sem asset correspondente THEN o save SHALL ignorar esse ID, manter os outros valores e registrar um aviso no Console. [SAVE-19]
- IF o save tiver uma moeda negativa THEN o save SHALL carregá-la como 0. [SAVE-20]
- IF a gravação do arquivo falhar THEN o save SHALL registrar um erro no Console e manter o jogo rodando com os valores em memória. [SAVE-21]
- IF a arma equipada do arquivo não existir THEN o save SHALL equipar a arma inicial. [SAVE-22]

Dimensões implícitas: validação (SAVE-19, SAVE-20, SAVE-22), falha parcial (SAVE-15, SAVE-21), corrupção (SAVE-17, SAVE-18) e ciclo de vida (SAVE-16) estão cobertas. Concorrência N/A: um único processo grava, sempre na thread principal. Autenticação, limites de taxa e dependências externas N/A: o save é um arquivo local. Observabilidade: os avisos e erros no Console acima.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| SAVE-01 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-02 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-03 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-04 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-05 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-06 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-07 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-08 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-09 | P1: Progresso guardado entre sessões | - | Pending |
| SAVE-10 | P1: Carregar e gravar nos momentos certos | - | Pending |
| SAVE-11 | P1: Carregar e gravar nos momentos certos | - | Pending |
| SAVE-12 | P1: Carregar e gravar nos momentos certos | - | Pending |
| SAVE-13 | P1: Carregar e gravar nos momentos certos | - | Pending |
| SAVE-14 | P1: Carregar e gravar nos momentos certos | - | Pending |
| SAVE-15 | P1: Carregar e gravar nos momentos certos | - | Pending |
| SAVE-16 | P2: Apagar o save no Editor | - | Pending |
| SAVE-17 | Edge Cases | - | Pending |
| SAVE-18 | Edge Cases | - | Pending |
| SAVE-19 | Edge Cases | - | Pending |
| SAVE-20 | Edge Cases | - | Pending |
| SAVE-21 | Edge Cases | - | Pending |
| SAVE-22 | Edge Cases | - | Pending |

**Coverage:** 22 total, 0 mapped to tasks, 22 unmapped (antes do Design)

---

## Sources

- `Docs/Last Wick - One-Pager.md` - pilar "Cada run deixa algo", recursos (ouro, óleo, osso, ferro, cera), padre, fogueira, ferreiro, deck de 10 a 15 cartas.
- `.specs/STATE.md` - AD-001 a AD-007.
- `Assets/_Project/Data/Cards/Deck.asset` - as 15 cartas do deck atual.
- `Assets/_Project/Code/Runtime/Cards/DeckData.cs` - regras atuais do deck (10 a 15, sem repetição).

---

## Success Criteria

- [ ] Fechar e abrir o jogo depois de uma run e de uma compra mostra os mesmos valores.
- [ ] Um `save.json` com texto inválido não impede o jogo de abrir.
