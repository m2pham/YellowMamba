using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using YellowMamba.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using YellowMamba.Players;

namespace YellowMamba.Characters
{
    public class BlackMamba : Character
    {

        public BlackMamba(PlayerIndex playerIndex, InputManager inputManager, PlayerManager playerManager, IServiceProvider serviceProvider, String contentRootDirectory)
            : base(playerIndex, inputManager, playerManager, serviceProvider, contentRootDirectory)
        {
            Speed = 5;
        }

        public override void LoadContent()
        {
            Sprite = ContentManager.Load<Texture2D>("BlackMamba");
        }

        public override void Update(GameTime gameTime)
        {
            switch (CharacterState)
            {
                case CharacterStates.ShootState:

                    break;
                case CharacterStates.PickState:

                    break;
                case CharacterStates.PassState:
                    ProcessMovement(Speed);
                    if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.Pass) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        break;
                    }

                    LinkedListNode<CharacterActions> passButtonNode = InputManager.PassButtons.First;
                    foreach (Player player in PlayerManager.Players)
                    {
                        if (player.PlayerIndex == PlayerIndex)
                        {
                            continue;
                        }
                        if (InputManager.GetCharacterActionState(PlayerIndex, passButtonNode.Value) == ActionStates.Pressed)
                        {
                            // passing code goes here
                            Vector2 receivingCharacterPosition = PlayerManager.GetPlayer(player.PlayerIndex).Character.Position;
                            Vector2 receivingCharacterVelocity = PlayerManager.GetPlayer(player.PlayerIndex).Character.Velocity;
                            float characterDistance = Vector2.Distance(receivingCharacterPosition, Position);

                        }
                    }
                    break;
                case CharacterStates.DefaultState:
                    if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.Pass) == ActionStates.Pressed
                        && PlayerManager.Players.Count > 1 && HasBall)
                    {
                        CharacterState = CharacterStates.PassState;
                    }
                    ProcessMovement(Speed);
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
        }
    }
}
