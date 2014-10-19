using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Entities;
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

    public abstract class Character : Entity
    {
        public Texture2D Sprite { get; set; }
        protected PlayerIndex PlayerIndex { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected PlayerManager PlayerManager { get; private set; }
        protected bool HasBall { get; set; }
        protected bool IsInvincible { get; set; }
        protected Pass CurrentPass { get; set; }
        public CharacterStates CharacterState { get; protected set; }

        public Character(PlayerIndex playerIndex, InputManager inputManager, PlayerManager playerManager)
            : base()
        {
            PlayerIndex = playerIndex;
            InputManager = inputManager;
            PlayerManager = playerManager;
            IsInvincible = false;
            CurrentPass = Pass.StraightPass;
            HasBall = false;
            CharacterState = CharacterStates.DefaultState;
            Position.Y = 180 * ((int)playerIndex + 1);
        }

        protected void ProcessMovement(int speed)
        {
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Released
                && InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Released)
            {
                Velocity.Y = 0;
            }
            else if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Held)
            {
                if (Position.Y >= 720/4)
                {
                    Velocity.Y = -speed;
                }
                else
                {
                    Velocity.Y = 0;
                }
            }
            else if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Held)
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

            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Released
                && InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Released)
            {
                Velocity.X = 0;
            }
            else if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Pressed
                 || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Held)
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
            else if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Held)
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
