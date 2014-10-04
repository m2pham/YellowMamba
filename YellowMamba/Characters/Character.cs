using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Managers;

namespace YellowMamba.Characters
{
    public enum CharacterStates
    {
        PassState, ShootState, PickState, DefaultState
    }

    public enum Pass
    {
        StraightPass, LobPass, BouncePass
    }

    public abstract class Character
    {
        protected PlayerIndex PlayerIndex { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected PlayerManager PlayerManager { get; private set; }
        protected ContentManager ContentManager { get; private set; }
        protected Texture2D Sprite { get; set; }
        protected bool HasBall { get; set; }
        protected bool IsInvincible { get; set; }
        protected Pass CurrentPass { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        protected int Speed { get; set; }
        public CharacterStates CharacterState { get; protected set; }

        public Character(PlayerIndex playerIndex, InputManager inputManager, PlayerManager playerManager, IServiceProvider serviceProvider, String contentRootDirectory)
        {
            PlayerIndex = playerIndex;
            InputManager = inputManager;
            PlayerManager = playerManager;
            ContentManager = new ContentManager(serviceProvider, contentRootDirectory);
            IsInvincible = false;
            CurrentPass = Pass.StraightPass;
            HasBall = false;
            Position = new Vector2();
            Velocity = new Vector2();
            CharacterState = CharacterStates.DefaultState;
        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        protected void ProcessMovement(int speed)
        {
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Held)
            {
                Velocity.Y = -speed;
                Position.Y -= speed;
            }
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Held)
            {
                Velocity.Y = speed;
                Position.Y += speed;
            }
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Held)
            {
                Velocity.X = -speed;
                Position.X -= speed;
            }
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Held)
            {
                Velocity.Y = speed;
                Position.X += speed;
            }
        }
    }
}
