using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Utility;
using YellowMamba.Characters;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;


namespace YellowMamba.Enemies
{
    public class Chaser : Enemy
    {
        private float scale = 4.0f;
        private ContentManager contentManager;

        public Chaser(PlayerManager playermanager)
            : base(playermanager)
        {
            Speed = 3;
            Health = 25;
            FacingLeft = false;
            Attack = 3;
            AttackWaitTime = 0;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            SpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("mon4/mon4_sprite_base"), 6, 14);
            StandingAnimation = new Animation(SpriteSheet, 1, 4, 5);
            RunningAnimation = new Animation(SpriteSheet, 15, 9, 5);
            AttackingAnimation = new Animation(SpriteSheet, 29, 5, 5);
            DamagedAnimation = new Animation(SpriteSheet, 57, 3, 5);
            DeathAnimation = new Animation(SpriteSheet, 57, 8, 5);
            CurrentAnimation = StandingAnimation;
            //Hitbox.Width = animatedSprite.FrameWidth;
            //Hitbox.Height = animatedSprite.FrameHeight;
            HitboxXDisplacement = 22 * (int)scale;
            HitboxYDisplacement = 25 * (int)scale;
            Hitbox.Width = SpriteSheet.FrameWidth * (int)scale - HitboxXDisplacement * 2;
            Hitbox.Height = 25 * (int)scale;
            AttackRange = new Vector2(12 * scale, 15 * scale);
            AttackHitbox = new Rectangle((int)(Position.X + HitboxXDisplacement), (int)Position.Y + 28, (int)AttackRange.X, (int)AttackRange.Y);
        }

        public override void Update(GameTime gameTime)
        {
            CurrentAnimation.Update(gameTime);
            if (!(EnemyState == EnemyStates.Damaged || EnemyState == EnemyStates.Dead))
            {
                ProcessDamage();
                Position.X += Velocity.X;
                Position.Y += Velocity.Y;
            }
            foreach (Player player in PlayerManager.Players)
            {
                if (player.Character.PickAggroBox.Intersects(Hitbox) && player.Character.Defending)
                {
                    focusedPlayer = player;
                }
            }
            if (focusedPlayer == null)
            {
                FacingLeft = false;
            }
            else if (focusedPlayer.Character.Hitbox.Center.X < Hitbox.Center.X)
            {
                FacingLeft = false;
            }
            else if (focusedPlayer.Character.Hitbox.Center.X > Hitbox.Center.X)
            {
                FacingLeft = true;
            }

            if (!FacingLeft)
            {
                AttackHitbox.X = (int)(Hitbox.X - AttackRange.X);
            }
            else
            {
                AttackHitbox.X = (int)Position.X + HitboxXDisplacement + Hitbox.Width;
            }
            AttackHitbox.Y = (int)Hitbox.Y;
            Hitbox.X = (int)Position.X + HitboxXDisplacement;
            Hitbox.Y = (int)Position.Y + HitboxYDisplacement - (int)PositionZ;
            switch (EnemyState)
            {
                case EnemyStates.Idle:
                    foreach (Player player in PlayerManager.Players)
                    {
                        if (Vector2.Distance(new Vector2(player.Character.Hitbox.Center.X, player.Character.Hitbox.Center.Y), new Vector2(Hitbox.Center.X, Hitbox.Center.Y)) <= 300)
                        {
                            focusedPlayer = player;
                            SelectAnimation(RunningAnimation);
                            EnemyState = EnemyStates.Chase;
                        }
                    }
                    break;
                case EnemyStates.Chase:
                    float distance = float.MaxValue;
                    foreach (Player player in PlayerManager.Players)
                    {
                        if (player.Character.PickAggroBox.Intersects(Hitbox) && player.Character.Defending)
                        {
                            focusedPlayer = player;
                            break;
                        }

                        float playerDistance = Vector2.Distance(player.Character.Position, Position);
                        if (distance > playerDistance)
                        {
                            distance = playerDistance;
                            focusedPlayer = player;
                        }
                    }

                    if (AttackHitbox.Intersects(focusedPlayer.Character.Hitbox) && focusedPlayer.Character.Hitbox.Bottom > Hitbox.Bottom - 50
                        && focusedPlayer.Character.Hitbox.Bottom < Hitbox.Bottom + 50)
                    {
                        Velocity.X = 0;
                        Velocity.Y = 0;
                        SelectAnimation(StandingAnimation);
                        EnemyState = EnemyStates.Attack;
                        break;
                    }

                    if ((FacingLeft && AttackHitbox.Left >= focusedPlayer.Character.Hitbox.Right)
                        || (!FacingLeft && AttackHitbox.Left >= focusedPlayer.Character.Hitbox.Right))
                    {
                        Velocity.X = -2;
                    }
                    else if ((!FacingLeft && AttackHitbox.Right <= focusedPlayer.Character.Hitbox.Left)
                        || (FacingLeft && AttackHitbox.Right <= focusedPlayer.Character.Hitbox.Left))
                    {
                        Velocity.X = 2;
                    }

                    if (Hitbox.Bottom >= focusedPlayer.Character.Hitbox.Bottom + 50)
                    {
                        Velocity.Y = -2;
                    }
                    else if (Hitbox.Bottom <= focusedPlayer.Character.Hitbox.Bottom - 50)
                    {
                        Velocity.Y = 2;
                    }
                    break;

                case EnemyStates.Attack:
                    if (!AttackHitbox.Intersects(focusedPlayer.Character.Hitbox))
                    {
                        SelectAnimation(RunningAnimation);
                        EnemyState = EnemyStates.Chase;
                    }
                    else
                    {
                        AttackWaitTime -= (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                        if (AttackWaitTime <= 0)
                        {
                            Random rnd = new Random();
                            AttackWaitTime = rnd.Next(30, 90);
                            SelectAnimation(AttackingAnimation);
                            EnemyState = EnemyStates.Attacking;
                        }
                    }
                    break;
                case EnemyStates.Attacking:
                    StateTimer += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (StateTimer == 20)
                    {
                        AttackVisible = true;
                    }
                    else
                    {
                        AttackVisible = false;
                    }
                    if (StateTimer > AttackingAnimation.NumFrames * AttackingAnimation.Frequency)
                    {
                        StateTimer = 0;
                        SelectAnimation(StandingAnimation);
                        EnemyState = EnemyStates.Attack;
                    }
                    break;
                case EnemyStates.SpecialAttack:

                    break;

                case EnemyStates.Retreat:
                    /* if( Health <  30)
                     {
                         velocity of enemy is opposite direction of focusedPlayer
                     }*/
                    break;

                case EnemyStates.Damaged:
                    StateTimer += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (StateTimer > DamagedAnimation.NumFrames * DamagedAnimation.Frequency)
                    {
                        StateTimer = 0;
                        SelectAnimation(StandingAnimation);
                        EnemyState = EnemyStates.Chase;
                    }
                    break;
                case EnemyStates.Dead:
                    StateTimer += 1;
                    if (StateTimer > DeathAnimation.NumFrames * DeathAnimation.Frequency - 1)
                    {
                        StateTimer = 0;
                        MarkForDelete = true;
                    }
                    break;
            }

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentAnimation.Draw(spriteBatch, Position, FacingLeft, scale);
            //spriteBatch.Draw(contentManager.Load<Texture2D>("Black"), Hitbox, Color.Blue);
            //spriteBatch.Draw(contentManager.Load<Texture2D>("Black"), AttackHitbox, Color.Red);
        }

        private void ProcessDamage()
        {
            foreach (Player player in PlayerManager.Players)
            {
                if (player.Character.AttackHitbox.Intersects(Hitbox) && player.Character.CharacterState == CharacterStates.AttackState
                    && player.Character.Hitbox.Bottom > Hitbox.Bottom - 50 && player.Character.Hitbox.Bottom < Hitbox.Bottom + 50)
                {
                    Health -= player.Character.Attack;
                    if (Health <= 0)
                    {
                        PlayerManager.Score++;
                        SelectAnimation(DeathAnimation);
                        EnemyState = EnemyStates.Dead;
                        StateTimer = 0;
                    }
                    else
                    {
                        StateTimer = 0;
                        SelectAnimation(DamagedAnimation);
                        StateTimer = 0;
                        EnemyState = EnemyStates.Damaged;
                    }
                }
            }
            foreach (Entity entity in PlayerManager.EntityManager.Entities.ToList())
            {
                if (Hitbox.Intersects(entity.Hitbox))
                {
                    if (entity.GetType() == typeof(ShootBall))
                    {
                        ShootBall ball = (ShootBall)entity;
                        if (Hitbox.Intersects(PlayerManager.GetPlayer(ball.SourcePlayer).Target.Hitbox))
                        {
                            // select damaged animation
                            Health -= PlayerManager.GetPlayer(ball.SourcePlayer).Character.ShootAttack;
                            if (Health <= 0)
                            {
                                PlayerManager.Score++;
                                StateTimer = 0;
                                SelectAnimation(DeathAnimation);
                                EnemyState = EnemyStates.Dead;
                            }
                            else
                            {
                                StateTimer = 0;
                                SelectAnimation(DamagedAnimation);
                                EnemyState = EnemyStates.Damaged;
                            }
                            PlayerManager.EntityManager.Entities.Remove(ball);
                        }
                    }
                }
            }
        }
    }

}
