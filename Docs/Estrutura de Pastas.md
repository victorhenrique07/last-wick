# Last Wick · Estrutura de Pastas

24/10/2026 · Victor Henrique Silva Galvão

A organização segue a recomendação oficial da Unity (guia *Best practices for organizing your Unity project*) e é a mais usada em projetos Unity:

1. **Tudo o que é do jogo fica em `Assets/_Project`.** O `_` mantém a pasta no topo e separa o nosso conteúdo de pacotes de terceiros.
2. **Dentro dela, as pastas são divididas por tipo de asset** (Art, Audio, Code, Data, Prefabs, Scenes). Arte, áudio e código têm fluxos e ferramentas diferentes, e é assim que a maioria das equipes trabalha.
3. **Dentro de cada tipo, as subpastas seguem os sistemas do jogo** (Player, Enemies, Bosses, Cards...), com os mesmos nomes em todo lugar.

## Árvore

```
Assets/
├── _Project/
│   ├── Art/
│   │   ├── Animations/   Player, Enemies, Bosses (clips e Animator Controllers)
│   │   ├── Materials/    Materiais de sprite e de luz 2D
│   │   ├── Sprites/      Player, Enemies, Bosses, Weapons, Drops, Environment, Hub, UI, VFX
│   │   ├── Tilemaps/     Palettes e Tiles do cemitério
│   │   └── Fonts/
│   ├── Audio/
│   │   ├── Music/
│   │   ├── SFX/
│   │   └── Mixers/
│   ├── Code/
│   │   ├── Runtime/      Scripts do jogo, um sistema por pasta (ver abaixo)
│   │   ├── Editor/       Ferramentas do Editor (não entram no build)
│   │   └── Tests/        EditMode e PlayMode
│   ├── Data/             ScriptableObjects: Cards, Enemies, Bosses, Weapons, Run, Progression
│   ├── Prefabs/          Player, Enemies, Bosses, Weapons, Projectiles, Drops, Environment, Hub, UI, VFX
│   ├── Scenes/           Cenas principais; Maps (os mapas da run) e Sandbox (testes)
│   └── Shaders/
├── ThirdParty/           Assets comprados ou baixados, sem edição
├── Settings/             Configurações da URP e do Input System (criadas pelo template)
└── Scenes/, Welcome/     Restos do template (ver Próximos passos)
```

## Sistemas em `Code/Runtime`

Cada pasta corresponde a uma parte do one-pager.

| Pasta | O que vai nela |
| --- | --- |
| Core | Bootstrap, gerenciador de cenas, eventos globais, save |
| Player | Movimento, esquiva, vida, status do personagem |
| Combat | Ataque manual, mira, dano, hitbox e hurtbox |
| Enemies | IA e spawn dos inimigos comuns (Soul, Ghoul, Mortician, Revenant) |
| Bosses | Comportamento e ataques de cada boss |
| Cards | Deck, escolha de cartas ao subir de nível, efeitos das cartas |
| Run | Barra de progresso, marcos, relógio de 10:00, tela de resultado |
| Drops | XP, materiais, cura, ouro e orbes de óleo, coleta |
| Light | Raio de visão, silhuetas no escuro |
| Hub | Fogueira, padre, ferreiro e mago |
| Progression | Dados permanentes: nível no padre, nível de luz, mapas e dificuldades |
| UI | HUD, menus, telas |
| Utilities | Código genérico sem relação com gameplay |

## Regras

- **Nomes em inglês e PascalCase**, sem espaços: `SoulEnemy.prefab`, `CardData.cs`, `Boss1_Attack.anim`.
- **Uma pasta por sistema, o mesmo nome em todo lugar.** O sprite, o prefab, o script e os dados do Ghoul ficam em `Sprites/Enemies`, `Prefabs/Enemies`, `Code/Runtime/Enemies` e `Data/Enemies`.
- **Mover e renomear sempre pelo Editor da Unity**, nunca pelo Explorer. Assim o `.meta` acompanha o asset e as referências não quebram.
- **Nada solto na raiz de `Assets`.** Se não houver pasta certa, crie uma dentro de `_Project`.
- **Terceiros em `ThirdParty`**, cada pacote na sua pasta e sem edição. Para mudar algo, copie para `_Project`.
- **Os valores de balanceamento ficam em `Data`**, como ScriptableObjects, e são ajustados no Inspector.
- **Os arquivos `.gitkeep`** existem só para o git guardar pastas vazias. A Unity os ignora, e podem ser apagados quando a pasta tiver conteúdo.

## Próximos passos (no Editor)

- Mover `Assets/Scenes/SampleScene.unity` para `_Project/Scenes` e renomear (por exemplo, `Run.unity`), conferindo depois a lista de cenas em *Build Profiles*.
- Apagar `Assets/Welcome` (é a página de boas-vindas do template 2D) quando não for mais útil.
- Quando o código crescer, criar um Assembly Definition em `Code/Runtime`, outro em `Code/Editor` e um em cada pasta de testes, para compilar mais rápido.
