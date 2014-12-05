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
    public class BlackMamba : Character
    {
        private const int HitboxDisplacement = 16;

        public BlackMamba(Player player, InputManager inputManager, PlayerManager playerManager)
            : base(player, inputManager, playerManager)
        {
            Speed = 5;
            PickHealth = 30;
            MaxPickHealth = 30;
            PickHealthRegenerationTimer = 0;
            PickHealthRegenerationRate = 5;
            Attack = 10;
            ShootAttack = 5;
            FacingLeft = false;
            AttackRange = new Vector2(30, 30);
            AttackHitbox = new Rectangle((int)(Position.X + HitboxDisplacement + 36), (int)Position.Y - 14, (int)AttackRange.X, (int)AttackRange.Y);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            ContentManager = contentManager;
            SpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("BlackMambaSpriteSheet"), 11, 7);
            PickHealthBarSprite = contentManager.Load<Texture2D>("PickHealthBar");
            StandingAnimation = new Animation(SpriteSheet, 8, 4, 5);
            RunningAnimation = new Animation(SpriteSheet, 22, 5, 5);
            ShootingAnimation = new Animation(SpriteSheet, 1, 4, 5);
            AttackingAnimation = new Animation(SpriteSheet, 15, 3, 5);
            JumpingAnimation = new Animation(SpriteSheet, 57, 7, 5);
            PrimeShotAnimation = new Animation(SpriteSheet, 64, 1, 5);
            PickingAnimation = new Animation(SpriteSheet, 71, 1, 5);
            DamagedAnimation = new Animation(SpriteSheet, 50, 5, 5);
            CurrentAnimation = StandingAnimation;
            Hitbox.Width = 30;
            Hitbox.Height = 70;
            PickAggroBox.Width = Hitbox.Width + 100;
            PickAggroBox.Height = Hitbox.Height + 100;
            PickDefendingBox.Width = Hitbox.Width + 50;
            PickDefendingBox.Height = Hitbox.Height + 50;
        }

        public override void Update(GameTime gameTime)
        {
            // really just 1?
            //int framesPassed = (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
            int framesPassed = 1;
            Hitbox.X = (int) Position.X + 17;
            Hitbox.Y = (int) Position.Y + 14 - (int)PositionZ;
            PickAggroBox.X = (int)Position.X - 50;
            PickAggroBox.Y = (int)Position.Y - 50;
            PickDefendingBox.X = (int)Position.X - 25;
            PickDefendingBox.Y = (int)Position.Y - 25;
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
                // 36 is the width that the image actually takes up in the frame
                AttackHitbox.X = (int)Position.X + HitboxDisplacement + 36;
            }
            AttackHitbox.Y = Hitbox.Y;
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
                        ShootBall shootBall = new ShootBall();
                        shootBall.SourcePosition = new Vector2(Hitbox.Right, Hitbox.Top);
                        shootBall.DestinationPosition = PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position;
                        shootBall.Velocity.X = (PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position.X - Position.X) / 60;
                        shootBall.Velocity.Y = -(PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position.Y - .5F * 60 * 60 / 2 - Position.Y) / 60;
                        shootBall.ReleaseTime = gameTime.TotalGameTime;
                        PlayerManager.EntityManager.Entities.Add(shootBall);
                    }
                    if (StateTimer >= ShootingAnimation.NumFrames * ShootingAnimation.Frequency)
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
                            PassBall ball = new PassBall();
                            ball.Position = Position;
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
                    if (StateTimer >= DamagedAnimation.NumFrames * DamagedAnimation.Frequency - 1)
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
                    VelocityZ -= .3F;
                    if (PassEffectTimer > 180 - JumpingAnimation.NumFrames * JumpingAnimation.Frequency * (2/3)
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
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position = Position;
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
                        VelocityZ = .3F * JumpingAnimation.NumFrames * JumpingAnimation.Frequency / 2;
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
                        Health -= enemy.Attack;
                        SelectAnimation(DamagedAnimation);
                        CharacterState = CharacterStates.DamagedState;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentAnimation.Draw(spriteBatch, Position, PositionZ, FacingLeft);
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
                    spriteBatch.Draw(buttonTexture, new Vector2(targetPlayer.Character.Hitbox.Center.X - buttonTexture.Width * .5F / 2F, (int)targetPlayer.Character.Position.Y - (int) targetPlayer.Character.PositionZ - yPos),
                        null, Color.White, 0, new Vector2(0,0), .5F, SpriteEffects.None, 0);
                    passButtonNode = passButtonNode.Next;
                }
            }
        }
    }
}
