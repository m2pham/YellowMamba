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
using YellowMamba.Utility;
using YellowMamba.Enemies;
using Microsoft.Xna.Framework.Input;

namespace YellowMamba.Characters
{
    public class Jimothy : Character
    {

        public Jimothy(Player player, InputManager inputManager, PlayerManager playerManager)
            : base(player, inputManager, playerManager)
        {
            Speed = 5;
            Health = 30;
            MaxHealth = 30;
            PickHealth = 30;
            MaxPickHealth = 30;
            PickHealthRegenerationTimer = 0;
            PickHealthRegenerationRate = 5;
            Attack = 10;
            ShootAttack = 5;
            FacingLeft = false;
            AttackRange = new Vector2(40, 10);
            Tint = Color.White;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            ContentManager = contentManager;
            PickHealthBarSprite = contentManager.Load<Texture2D>("PickHealthBar");
            SpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimwalk"), 1, 4);
            SpriteSheet tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimwalk"), 1, 4);
            StandingAnimation = new Animation(tempSpriteSheet, 5);
            JumpingAnimation = new Animation(tempSpriteSheet, 5);
            tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimrun"), 1, 4);
            RunningAnimation = new Animation(tempSpriteSheet, 5);
            tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimshoot"), 1, 4);
            ShootingAnimation = new Animation(tempSpriteSheet, 5);
            tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimattack"), 1, 3);
            AttackingAnimation = new Animation(tempSpriteSheet, 5);
            tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimshoot"), 1, 4);
            PrimeShotAnimation = new Animation(tempSpriteSheet, 1, 1, 5);
            tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/Jimattack"), 1, 3);
            PickingAnimation = new Animation(tempSpriteSheet, 2, 1, 5);
            tempSpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("Jimothy/hurt"), 1, 1);
            DamagedAnimation = new Animation(tempSpriteSheet, 5);
            CurrentAnimation = StandingAnimation;
            PickAggroBox.Width = Hitbox.Width + 100;
            PickAggroBox.Height = Hitbox.Height + 100;
            PickDefendingBox.Width = Hitbox.Width + 50;
            PickDefendingBox.Height = Hitbox.Height + 50;
            HitboxXDisplacement = 80;
            HitboxYDisplacement = 20;
            Hitbox.Width = SpriteSheet.FrameWidth - 2 * HitboxXDisplacement;
            Hitbox.Height = SpriteSheet.FrameHeight - 2 * HitboxYDisplacement;
            // 87 is distance from top of frame to arm
            AttackHitbox = new Rectangle((int)(Position.X + HitboxXDisplacement), (int)Position.Y + 70, (int)AttackRange.X, (int)AttackRange.Y);
        }

        public override void Update(GameTime gameTime)
        {
            // really just 1?
            //int framesPassed = (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
            int framesPassed = 1;
            Hitbox.X = (int)Position.X + HitboxXDisplacement;
            Hitbox.Y = (int)Position.Y + HitboxYDisplacement - (int)PositionZ;
            PickAggroBox.X = (int)Hitbox.X - 50;
            PickAggroBox.Y = (int)Hitbox.Y - 50;
            PickDefendingBox.X = (int)Hitbox.X - 25;
            PickDefendingBox.Y = (int)Hitbox.Y - 25;
            if (PassEffectTimer > 0 && (PassEffectTimer -= framesPassed) <= 0)
            {
                PassEffectTimer = 0;
                ShootAttack /= 5;
            }
            if (!(CharacterState == CharacterStates.PickState || CharacterState == CharacterStates.StunnedState) && PickHealth < MaxPickHealth)
            {
                PickHealthRegenerationTimer -= framesPassed;
            }
            if (PickHealthRegenerationTimer <= 0)
            {
                PickHealthRegenerationTimer = 60;
                PickHealth += PickHealthRegenerationRate;
                if (PickHealth > MaxPickHealth)
                {
                    PickHealth = MaxPickHealth;
                }
            }
            CheckCollision();
            if (CharacterState != CharacterStates.DamagedState)
            {
                ProcessIncomingDamage();
            }
            if (FacingLeft)
            {
                AttackHitbox.X = (int)(Hitbox.X - AttackRange.X);
            }
            else
            {
                AttackHitbox.X = (int)Position.X + HitboxXDisplacement + Hitbox.Width;
            }
            AttackHitbox.Y = (int)Position.Y + 70;
            CurrentAnimation.Update(gameTime);
            switch (CharacterState)
            {
                case CharacterStates.ShootState:
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.ShootMode) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Visible = false;
                        break;
                    }
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Attack) == ActionStates.Pressed)
                    {
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Visible = false;
                        SelectAnimation(ShootingAnimation);
                        CharacterState = CharacterStates.ShootingState;
                    }
                    break;
                case CharacterStates.ShootingState:
                    StateTimer += framesPassed;
                    if (StateTimer == (ShootingAnimation.NumFrames - 1) * ShootingAnimation.Frequency)
                    {
                        ShootBall shootBall = new ShootBall(Tint);
                        int disp = 150;
                        if (FacingLeft)
                        {
                            disp = 50;
                        }
                        shootBall.SourcePosition = new Vector2(Position.X + disp, Hitbox.Top);
                        shootBall.DestinationPosition = new Vector2(PlayerManager.GetPlayer(Player.PlayerIndex).Target.Hitbox.Center.X, PlayerManager.GetPlayer(Player.PlayerIndex).Target.Hitbox.Center.Y);
                        shootBall.Velocity.X = (shootBall.DestinationPosition.X - shootBall.SourcePosition.X) / 60;
                        shootBall.Velocity.Y = -(shootBall.DestinationPosition.Y - .5F * 60 * 60 / 2 - shootBall.SourcePosition.Y) / 60;
                        shootBall.ReleaseTime = gameTime.TotalGameTime;
                        PlayerManager.EntityManager.Entities.Add(shootBall);
                    }
                    if (StateTimer >= ShootingAnimation.NumFrames * ShootingAnimation.Frequency - 1)
                    {
                        StateTimer = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.PickState:
                    Defending = false;
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pick) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        break;
                    }

                    foreach (Player player in PlayerManager.Players)
                    {
                        if (player.Character.Hitbox.Intersects(PickDefendingBox))
                        {
                            Defending = true;
                            break;
                        }
                    }
                    break;
                case CharacterStates.PassState:
                    ProcessMovement(Speed);
                    Position += Velocity;
                    if (Velocity.X != 0 || Velocity.Y != 0)
                    {
                        SelectAnimation(RunningAnimation);
                    }
                    else
                    {
                        SelectAnimation(StandingAnimation);
                    }
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pass) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        break;
                    }

                    LinkedListNode<Buttons> passButtonNode = InputManager.PassButtons.First;
                    foreach (Player targetPlayer in PlayerManager.Players)
                    {
                        if (targetPlayer.PlayerIndex == Player.PlayerIndex)
                        {
                            continue;
                        }
                        if (targetPlayer.Character.CharacterState == CharacterStates.StunnedState)
                        {
                            passButtonNode = passButtonNode.Next;
                            continue;
                        }
                        if (InputManager.GetButtonActionState(Player.PlayerIndex, passButtonNode.Value) == ActionStates.Pressed)
                        {
                            PassBall ball = new PassBall(Tint);
                            ball.Position = new Vector2(Hitbox.Center.X, Hitbox.Center.Y);
                            ball.SourcePlayer = Player;
                            ball.TargetPlayer = targetPlayer;
                            ball.ReleaseTime = gameTime.TotalGameTime;
                            PlayerManager.EntityManager.Entities.Add(ball);
                            HasBall = false;
                            CharacterState = CharacterStates.DefaultState;
                            passButtonNode = passButtonNode.Next;
                        }
                    }
                    break;
                case CharacterStates.AttackState:
                    StateTimer += framesPassed;
                    if (StateTimer >= AttackingAnimation.NumFrames * AttackingAnimation.Frequency)
                    {
                        StateTimer = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.DamagedState:
                    StateTimer += framesPassed;
                    //if (StateTimer >= DamagedAnimation.NumFrames * DamagedAnimation.Frequency - 1)
                    if (StateTimer >= 20)
                    {
                        StateTimer = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.StunnedState:
                    StateTimer += framesPassed;
                    if (StateTimer >= AttackingAnimation.NumFrames * AttackingAnimation.Frequency)
                    {
                        StateTimer = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.JumpingState:
                    StateTimer += framesPassed;
                    ProcessMovement(Speed);
                    Position.X += Velocity.X;
                    PositionZ += VelocityZ;
                    VelocityZ -= 1F;
                    if (PassEffectTimer > 180 - JumpingAnimation.NumFrames * JumpingAnimation.Frequency * (2 / 3)
                        && InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.ShootMode) == ActionStates.Pressed)
                    {
                        // alley oop or dunk or some crazy thing
                    }
                    if (StateTimer > JumpingAnimation.NumFrames * JumpingAnimation.Frequency)
                    {
                        PositionZ = 0;
                        StateTimer = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.DeadState:
                    MarkForDelete = true;
                    break;
                case CharacterStates.DefaultState:
                    ProcessMovement(Speed);
                    Position += Velocity;
                    if (Velocity.X < 0)
                    {
                        FacingLeft = true;
                    }
                    else if (Velocity.X > 0)
                    {
                        FacingLeft = false;
                    }
                    if (Velocity.X != 0 || Velocity.Y != 0)
                    {
                        SelectAnimation(RunningAnimation);
                    }
                    else
                    {
                        SelectAnimation(StandingAnimation);
                    }
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pass) == ActionStates.Pressed
                        && PlayerManager.Players.Count > 1 && HasBall)
                    {
                        CharacterState = CharacterStates.PassState;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.ShootMode) == ActionStates.Pressed)
                    {
                        CharacterState = CharacterStates.ShootState;
                        SelectAnimation(PrimeShotAnimation);
                        int disp;
                        if (FacingLeft)
                        {
                            disp = -5;
                        }
                        else
                        {
                            disp = 5;
                        }
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position = new Vector2(Hitbox.Center.X - 100/2 + disp, Hitbox.Center.Y - 100/2);
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Visible = true;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Attack) == ActionStates.Pressed)
                    {
                        SelectAnimation(AttackingAnimation);
                        CharacterState = CharacterStates.AttackState;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pick) == ActionStates.Pressed)
                    {
                        SelectAnimation(PickingAnimation);
                        CharacterState = CharacterStates.PickState;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Jump) == ActionStates.Pressed)
                    {
                        SelectAnimation(JumpingAnimation);
                        VelocityZ = 1F * JumpingAnimation.NumFrames * JumpingAnimation.Frequency / 2;
                        CharacterState = CharacterStates.JumpingState;
                    }
                    break;
            }
        }

        private void CheckCollision()
        {
            foreach (Entity entity in PlayerManager.EntityManager.Entities.ToList())
            {
                if (Hitbox.Intersects(entity.Hitbox) && entity.PositionZ <= PositionZ + 10 && entity.PositionZ >= PositionZ - 10)
                {
                    if (entity.GetType() == typeof(PassBall))
                    {
                        PassBall ball = (PassBall)entity;
                        if (!ball.InFlight || ball.SourcePlayer.PlayerIndex != Player.PlayerIndex)
                        {
                            HasBall = true;
                            entity.MarkForDelete = true;
                            PassEffectTimer = 180;
                            ShootAttack *= 5;
                        }
                    }
                }
            }
        }

        private void ProcessIncomingDamage()
        {
            foreach (Enemy enemy in PlayerManager.EnemyManager.Enemies.ToList())
            {
                if (enemy.Ranged == true)
                {
                    continue;
                }
                if (Hitbox.Intersects(enemy.AttackHitbox) && enemy.AttackVisible)
                {
                    if (CharacterState == CharacterStates.PickState)
                    {
                        PickHealth -= enemy.Attack;
                        if (PickHealth <= 0)
                        {
                            PickHealth = 0;
                            CharacterState = CharacterStates.StunnedState;
                        }
                    }
                    else
                    {
                        if (CharacterState == CharacterStates.ShootState)
                        {
                            PlayerManager.GetPlayer(Player.PlayerIndex).Target.Visible = false;
                        }
                        Health -= enemy.Attack;
                        if (Health <= 0)
                        {
                            CharacterState = CharacterStates.DeadState;
                            StateTimer = 0;
                            break;
                        }
                        SelectAnimation(DamagedAnimation);
                        CharacterState = CharacterStates.DamagedState;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentAnimation.Draw(spriteBatch, Position, PositionZ, FacingLeft, Tint);
            if (CharacterState == CharacterStates.PickState)
            {
                spriteBatch.Draw(PickHealthBarSprite, new Rectangle(Hitbox.Center.X - PickHealthBarSprite.Bounds.Center.X, (int)Position.Y - 20, PickHealthBarSprite.Width * PickHealth / MaxPickHealth, PickHealthBarSprite.Height), Color.White);
            }
            else if (CharacterState == CharacterStates.PassState)
            {
                LinkedListNode<Buttons> passButtonNode = InputManager.PassButtons.First;
                foreach (Player targetPlayer in PlayerManager.Players)
                {
                    if (targetPlayer.PlayerIndex == Player.PlayerIndex)
                    {
                        continue;
                    }
                    if (targetPlayer.Character.CharacterState == CharacterStates.StunnedState)
                    {
                        passButtonNode = passButtonNode.Next;
                        continue;
                    }

                    Texture2D buttonTexture = ContentManager.Load<Texture2D>("Button" + passButtonNode.Value.ToString());
                    int yPos = 40;
                    if (targetPlayer.Character.CharacterState == CharacterStates.PickState)
                    {
                        yPos += 10;
                    }
                    spriteBatch.Draw(buttonTexture, new Vector2(targetPlayer.Character.Hitbox.Center.X - buttonTexture.Width * .5F / 2F, (int)targetPlayer.Character.Position.Y - (int)targetPlayer.Character.PositionZ - yPos),
                        null, Color.White, 0, new Vector2(0, 0), .5F, SpriteEffects.None, 0);
                    passButtonNode = passButtonNode.Next;
                }
            }

            if (DrawHealthBar)
            {
                spriteBatch.Draw(PickHealthBarSprite, new Rectangle(Hitbox.Center.X - PickHealthBarSprite.Bounds.Center.X, (int)Position.Y - 10 - (int)PositionZ, PickHealthBarSprite.Width * Health / MaxHealth, PickHealthBarSprite.Height), Color.Green);
            }            //spriteBatch.Draw(ContentManager.Load<Texture2D>("Black"), Hitbox, Color.Blue);
            //spriteBatch.Draw(ContentManager.Load<Texture2D>("Black"), AttackHitbox, Color.Red);
        }
    }
}
