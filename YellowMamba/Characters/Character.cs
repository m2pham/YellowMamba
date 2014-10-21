using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;

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
        protected Player Player { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected PlayerManager PlayerManager { get; private set; }
        protected bool HasBall { get; set; }
        protected bool IsInvincible { get; set; }
        protected Pass CurrentPass { get; set; }
        protected int Health { get; set; } // added health
        public CharacterStates CharacterState { get; protected set; }

        public Character(Player player, InputManager inputManager, PlayerManager playerManager)
            : base()
        {
            Player = player;
            InputManager = inputManager;
            PlayerManager = playerManager;
            IsInvincible = false;
            CurrentPass = Pass.StraightPass;
            HasBall = false;
            CharacterState = CharacterStates.DefaultState;
            Position.Y = 180 * ((int)Player.PlayerIndex + 1);
        }

        protected void ProcessMovement(int speed)
        {
            if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveUp) == ActionStates.Released
                && InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveDown) == ActionStates.Released)
            {
                Velocity.Y = 0;
            }
            else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveUp) == ActionStates.Held)
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
            else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveDown) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveDown) == ActionStates.Held)
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

            if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Released
                && InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveRight) == ActionStates.Released)
            {
                Velocity.X = 0;
            }
            else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Pressed
                 || InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Held)
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
            else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveRight) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveRight) == ActionStates.Held)
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
