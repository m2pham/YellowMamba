using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Managers;
using YellowMamba.Players;

namespace YellowMamba.Entities
{
    public class ShootTarget : Entity
    {
        public static Texture2D Sprite { get; set; }
        public bool InFlight { get; set; }
        public Player SourcePlayer { get; set; }
        private InputManager inputManager;
        public bool Visible { get; set; }

        public ShootTarget(InputManager inputManager, Player sourcePlayer)
            : base()
        {
            this.inputManager = inputManager;
            SourcePlayer = sourcePlayer;
            Hitbox = new Rectangle();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("ShootTarget");
        }

        public override void Update(GameTime gameTime)
        {
            Hitbox.Width = Sprite.Width;
            Hitbox.Height = Sprite.Height;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
            if (Hitbox.Center.X < SourcePlayer.Character.Hitbox.X)
            {
                SourcePlayer.Character.FacingLeft = true;
            }
            else if (Hitbox.Center.X > SourcePlayer.Character.Hitbox.X)
            {
                SourcePlayer.Character.FacingLeft = false;
            }
            Vector2 leftThumbstick = inputManager.GetLeftThumbstickPos(SourcePlayer.PlayerIndex);
            ProcessMovement(leftThumbstick.X * 10, leftThumbstick.Y * 10);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
        }

        protected void ProcessMovement(float xSpeed, float ySpeed)
        {
            PlayerIndex sourcePlayerIndex = SourcePlayer.PlayerIndex;
            if (inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveUp) == ActionStates.Released
                && inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveDown) == ActionStates.Released)
            {
                Velocity.Y = 0;
            }
            else if (inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveUp) == ActionStates.Held)
            {
                if (Position.Y >= 720 / 2 - 50)
                {
                    Velocity.Y = -ySpeed;
                }
                else
                {
                    Velocity.Y = 0;
                }
            }
            else if (inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveDown) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveDown) == ActionStates.Held)
            {
                if (Position.Y + Sprite.Height <= 720)
                {
                    Velocity.Y = -ySpeed;
                }
                else
                {
                    Velocity.Y = 0;
                }
            }

            if (inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveLeft) == ActionStates.Released
                && inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveRight) == ActionStates.Released)
            {
                Velocity.X = 0;
            }
            else if (inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveLeft) == ActionStates.Pressed
                 || inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveLeft) == ActionStates.Held)
            {
                if (Position.X >= 0)
                {
                    Velocity.X = xSpeed;
                }
                else
                {
                    Velocity.X = 0;
                }
            }
            else if (inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveRight) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(sourcePlayerIndex, CharacterActions.MoveRight) == ActionStates.Held)
            {
                if (Position.X + Sprite.Width <= 1280)
                {
                    Velocity.X = xSpeed;
                }
                else
                {
                    Velocity.X = 0;
                }
            }

            Position += Velocity;
        }
    }
}
