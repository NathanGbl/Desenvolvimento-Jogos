# O Caminho do Peregrino (Unity 2D)

Base completa de gameplay para um Metroidvania 2D sem assets visuais/sonoros.

## Estrutura de cenas

- `MainMenu`
- `Level_1`
- `Level_2`
- `EndScene`

São adicionadas automaticamente no Build Settings pelo gerador de cenas.

## Geração automática (one-click)

1. Abra o projeto no Unity.
2. Aguarde a compilação dos scripts.
3. Acesse o menu: `Tools > O Caminho do Peregrino > Gerar Cenas Base`.

Isso gera automaticamente:

- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Level_1.unity`
- `Assets/Scenes/Level_2.unity`
- `Assets/Scenes/EndScene.unity`
- `Assets/Prefabs/LightProjectile.prefab`

Também cria hierarquias completas de gameplay com:

- Player, inimigos, coletáveis, gates de virtudes, HUD e parallax.
- Fluxo vitória/derrota e reinício.
- Build Settings pronto.

## Scripts principais

- `Assets/Scripts/Core/GameFlow.cs`: estado global (vidas, pontuação, coletáveis, vitória/derrota).
- `Assets/Scripts/Core/PlayerProgress.cs`: virtudes/habilidades desbloqueadas.
- `Assets/Scripts/Player/PlayerController2D.cs`: andar, pulo, pulo duplo (desbloqueável), dash.
- `Assets/Scripts/Player/PlayerCombat.cs`: disparo de luz à distância.
- `Assets/Scripts/Combat/LightProjectile.cs`: projétil que derrota inimigos.
- `Assets/Scripts/Combat/EnemyController.cs`: inimigo com patrulha e dano por contato.
- `Assets/Scripts/World/CollectibleItem.cs`: itens colecionáveis e pontuação.
- `Assets/Scripts/World/LevelExit.cs`: transição de fase e condição de vitória.
- `Assets/Scripts/World/ParallaxLayer.cs`: parallax de fundo.
- `Assets/Scripts/Interaction/CharityNpc.cs`: interação com NPC para virtude Caridade.
- `Assets/Scripts/World/PhysicalBarrier.cs`: barreira que quebra com Fortaleza + dash.
- `Assets/Scripts/UI/HUDController.cs`: HUD de vidas/pontos/itens/prompt.

## O que você precisa ajustar (somente assets)

- Substituir sprites placeholders de player, inimigos, itens, cenário e UI.
- Ligar `Animator Controller` e animações nos objetos.
- Ajustar SFX/BGM e VFX.
- Refinar layout/level design das fases.

## Setup manual (opcional)

Se preferir não usar o gerador one-click, siga este setup manual.

1. Crie um objeto `GameSystems` na `MainMenu` com:
   - `GameFlow`
   - `PlayerProgress`
2. Crie a UI da `MainMenu` com:
   - Texto de instruções
   - Botão `Iniciar` chamando `MainMenuController.StartGame()`
   - Botão `Sair` chamando `MainMenuController.QuitGame()`
3. Em cada fase (`Level_1`, `Level_2`):
   - Player com `Tag = Player`, `Rigidbody2D (Dynamic)`, `Collider2D`
   - Scripts no player: `PlayerController2D`, `PlayerCombat`, `PlayerHealth`, `PlayerInteractor`
   - Filho `GroundCheck` atribuído no `PlayerController2D`
   - `LightProjectile` prefab com `Collider2D` em `isTrigger`
   - Inimigos com `EnemyController`, `Collider2D`, `Rigidbody2D` (Kinematic ou Dynamic)
   - Coletáveis com `CollectibleItem` e `Collider2D` em `isTrigger`
   - Saída da fase com `LevelExit`
   - Objeto com `LevelCollectibleRegistrar`
   - Camadas de fundo com `ParallaxLayer`
4. Crie virtudes/gates:
   - Pickup de Caridade/Fortaleza/Pulo Duplo com `AbilityPickup`
   - NPC com `CharityNpc` + `Collider2D` trigger
   - Barreira com `Tag = Barreira_Fisica` e script `PhysicalBarrier`
5. `EndScene`:
   - `EndSceneController` com referências de textos TMP e botão para `RestartRun()`

## Inputs padrão

- Movimento: `Horizontal` (A/D, setas)
- Pulo: `Jump` (espaço)
- Dash: `LeftShift`
- Disparo de luz: `J`
- Interação: `E`
- Escudo de Fé (opcional): `K`

## Critérios implementados

- Plataforma 2D com colisão física
- Duas fases + tela inicial + tela final
- Pontuação e vidas
- Derrota quando vidas chegam a zero
- Vitória ao coletar todos os itens requeridos e concluir fase final
- Ability gates (Caridade e Fortaleza)
- Parallax

## Observações

- Animações de player/inimigos/itens ficam prontas para você ligar via `Animator` e clips.
- Assets (sprites, áudio, VFX) ficam totalmente sob sua personalização.
