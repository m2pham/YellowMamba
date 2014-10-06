using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using YellowMamba.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using YellowMamba.Players;
using YellowMamba.Entities;

namespace YellowMamba.Characters
{
    public class Jimothy : Character
    {
        public Jimothy(PlayerIndex playerIndex, InputManager inputManager, PlayerManager playerManager)
            : base(playerIndex, inputManager, playerManager)
        {
            Speed = 5;
            HasBall = false;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("Jimothy");
            Hitbox.Width = Sprite.Width;
            Hitbox.Height = Sprite.Height;
        }

        public override void Update(GameTime gameTime)
        {
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
            CheckCollision();
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
                            Ball ball = new Ball();
                            ball.Position = Position;
                            ball.Velocity.X = receivingCharacterVelocity.X + (receivingCharacterPosition.X - Position.X) / 30;
                            ball.Velocity.Y = receivingCharacterVelocity.Y + (receivingCharacterPosition.Y - Position.Y) / 30;
                            ball.InFlight = true;
                            ball.SourcePlayer = PlayerIndex;
                            PlayerManager.EntityManager.Entities.Add(ball);
                            //HasBall = false;
                            CharacterState = CharacterStates.DefaultState;
                        }
                    }
                    break;
                case CharacterStates.DefaultState:
                    ProcessMovement(Speed);
                    if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.Pass) == ActionStates.Pressed
                        && PlayerManager.Players.Count > 1 && HasBall)
                    {
                        CharacterState = CharacterStates.PassState;
                    }
                    break;
            }
        }

        private void CheckCollision()
        {
            foreach (Entity entity in PlayerManager.EntityManager.Entities.ToList())
            {
                if (Hitbox.Intersects(entity.Hitbox))
                {
                    if (entity.GetType() == typeof(Ball))
                    {
                        Ball ball = (Ball)entity;
                        if (!ball.InFlight || ball.SourcePlayer != PlayerIndex)
                        {
                            Console.WriteLine("BALL RECEIVED BY: " + PlayerIndex);
                            HasBall = true;
                            PlayerManager.EntityManager.Entities.Remove(entity);
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
        }
    }
}
