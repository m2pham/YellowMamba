﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Utility;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;

namespace YellowMamba.Characters
{
    public enum CharacterStates
    {
        PassState, ShootState, ShootingState, PickState, AttackState, DamagedState, StunnedState, JumpingState, DefaultState, DeadState
    }

    public enum Pass
    {
        StraightPass, LobPass, BouncePass
    }

    public abstract class Character : Entity
    {
        public bool DrawHealthBar { get; set; }
        protected int HitboxXDisplacement { get; set; }
        protected int HitboxYDisplacement { get; set; }
        public SpriteSheet SpriteSheet { get; set; }
        public Animation CurrentAnimation { get; protected set; }
        public Animation StandingAnimation { get; protected set; }
        public Animation RunningAnimation { get; protected set; }
        public Animation PrimeShotAnimation { get; protected set; }
        public Animation ShootingAnimation { get; protected set; }
        public Animation PickingAnimation { get; protected set; }
        public Animation AttackingAnimation { get; protected set; }
        public Animation JumpingAnimation { get; protected set; }
        public Animation DamagedAnimation { get; protected set; }
        public Animation PassAnimation { get; protected set; }
        protected Texture2D PickHealthBarSprite { get; set; }
        protected Player Player { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected PlayerManager PlayerManager { get; private set; }
        protected ContentManager ContentManager { get; set; }
        protected bool HasBall { get; set; }
        protected bool IsInvincible { get; set; }
        protected Pass CurrentPass { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int ShootAttack { get; set; }
        public int Defense { get; set; }
        public int PickHealth { get; set; }
        public int MaxPickHealth { get; set; }
        protected int PickHealthRegenerationTimer { get; set; }
        protected int PickHealthRegenerationRate { get; set; } // pickhealth/timer tick
        protected int PassEffectTimer { get; set; }
        public CharacterStates CharacterState { get; protected set; }
        public bool FacingLeft { get; set; }
        protected int StateTimer { get; set; }
        public Rectangle AttackHitbox;
        public Rectangle PickAggroBox;
        public Rectangle PickDefendingBox;
        public bool Defending { get; protected set; }
        protected Vector2 AttackRange { get; set; }
        public Color Tint { get; protected set; }

        public Character(Player player, InputManager inputManager, PlayerManager playerManager)
            : base()
        {
            Player = player;
            HasBall = player.PlayerIndex == PlayerIndex.One;
            InputManager = inputManager;
            PlayerManager = playerManager;
            IsInvincible = false;
            CurrentPass = Pass.StraightPass;
            CharacterState = CharacterStates.DefaultState;
            Position.Y = 310 + 100 * (int)Player.PlayerIndex;
            PickAggroBox = new Rectangle();
            PickDefendingBox = new Rectangle();
        }

        protected void ProcessMovement(float speed)
        {
            if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveUp) == ActionStates.Released
                && InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveDown) == ActionStates.Released)
            {
                Velocity.Y = 0;
            }
            else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.MoveUp) == ActionStates.Held)
            {
                if (Position.Y >= 720/2 - Hitbox.Height)
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
                if (Position.Y + Hitbox.Height <= 720)
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
                if (Position.X + HitboxXDisplacement >= 0)
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
                if (Position.X + Hitbox.Width + HitboxXDisplacement <= 1280)
                {
                    Velocity.X = speed;
                }
                else
                {
                    Velocity.X = 0;
                }
            }
        }

        public void SelectAnimation(Animation animation)
        {
            CurrentAnimation = animation;
            //CurrentAnimation.Reset();
        }
    }
}
