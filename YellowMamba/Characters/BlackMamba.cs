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
    public class BlackMamba : Character
    {
        public BlackMamba(PlayerIndex playerIndex, InputManager inputManager, PlayerManager playerManager)
            : base(playerIndex, inputManager, playerManager)
        {
            Speed = 5;
            HasBall = true;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("BlackMamba");
            Hitbox.Width = Sprite.Width;
            Hitbox.Height = Sprite.Height;
        }

        public override void Update(GameTime gameTime)
        {
            Hitbox.X = (int) Position.X;
            Hitbox.Y = (int) Position.Y;
            CheckCollision();
            switch (CharacterState)
            {
                case CharacterStates.ShootState:
                    if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.ShootMode) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        PlayerManager.GetPlayer(PlayerIndex).Target.Visible = false;
                        break;
                    }
                    if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.Attack) == ActionStates.Pressed)
                    {
                        ShootBall shootBall = new ShootBall();
                        shootBall.SourcePosition = Position;
                        shootBall.Velocity.X = (PlayerManager.GetPlayer(PlayerIndex).Target.Position.X - Position.X) / 60;
                        shootBall.Velocity.Y = - (PlayerManager.GetPlayer(PlayerIndex).Target.Position.Y - .5F * 60 * 60 / 2 - Position.Y) / 60;
                        shootBall.ReleaseTime = gameTime.TotalGameTime;
                        PlayerManager.EntityManager.Entities.Add(shootBall);
                        PlayerManager.GetPlayer(PlayerIndex).Target.Visible = false;
                        CharacterState = CharacterStates.DefaultState;
                    }
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
                            PassBall ball = new PassBall();
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
                    if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.ShootMode) == ActionStates.Pressed)
                    {
                        CharacterState = CharacterStates.ShootState;
                        PlayerManager.GetPlayer(PlayerIndex).Target.Position = Position;
                        PlayerManager.GetPlayer(PlayerIndex).Target.Visible = true;
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
                    if (entity.GetType() == typeof(PassBall))
                    {
                        PassBall ball = (PassBall)entity;
                        if (!ball.InFlight || ball.SourcePlayer != PlayerIndex)
                        {
                            HasBall = true;
                            entity.MarkForDelete = true;
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
