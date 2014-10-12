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
                Velocity.Y = -speed;
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveDown) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveDown) == ActionStates.Held)
            {
                Velocity.Y = speed;
            }
            if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveLeft) == ActionStates.Released
                && inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveRight) == ActionStates.Released)
            {
                Velocity.X = 0;
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveLeft) == ActionStates.Pressed
                 || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveLeft) == ActionStates.Held)
            {
                Velocity.X = -speed;
            }
            else if (inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveRight) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(SourcePlayer, CharacterActions.MoveRight) == ActionStates.Held)
            {
                Velocity.X = speed;
            }
            Position += Velocity;
        }
    }
}
