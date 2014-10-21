using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Managers;

namespace YellowMamba.Entities
{
    public class ShootTarget : Entity
    {
        public static Texture2D Sprite { get; set; }
        public bool InFlight { get; set; }
        public PlayerIndex SourcePlayer { get; set; }
        private InputManager inputManager;
        public bool Visible { get; set; }

        public ShootTarget(InputManager inputManager, PlayerIndex sourcePlayer)
            : base()
        {
            this.inputManager = inputManager;
            SourcePlayer = sourcePlayer;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("ShootTarget");
        }

        public override void Update(GameTime gameTime)
        {
            ProcessMovement(Speed);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
        }

        protected void ProcessMovement(int speed)
        {
            if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveUp) == ActionStates.Released
                && inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveDown) == ActionStates.Released)
            {
                Velocity.Y = 0;
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveUp) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveUp) == ActionStates.Held)
            {
                if (Position.Y >= 720 / 2 - 50)
                {
                    Velocity.Y = -speed;
                }
                else
                {
                    Velocity.Y = 0;
                }
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveDown) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveDown) == ActionStates.Held)
            {
                if (Position.Y + Sprite.Height <= 720)
                {
                    Velocity.Y = speed;
                }
                else
                {
                    Velocity.Y = 0;
                }
            }

            if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveLeft) == ActionStates.Released
                && inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveRight) == ActionStates.Released)
            {
                Velocity.X = 0;
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveLeft) == ActionStates.Pressed
                 || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveLeft) == ActionStates.Held)
            {
                if (Position.X >= 0)
                {
                    Velocity.X = -speed;
                }
                else
                {
                    Velocity.X = 0;
                }
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveRight) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveRight) == ActionStates.Held)
            {
                if (Position.X + Sprite.Width <= 1280)
                {
                    Velocity.X = speed;
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
