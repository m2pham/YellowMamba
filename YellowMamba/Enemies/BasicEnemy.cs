using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.AnimatedSprites;
using YellowMamba.Characters;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;


namespace YellowMamba.Enemies
{
    public class BasicEnemy : Enemy
    {
        public float timeToChange;
        private const int HitboxDisplacement = 50;
        public BasicEnemy(PlayerManager playermanager)
            : base(playermanager)
        {
            Speed = 3;
            Health = 30;
            timeToChange = 0F;
            DamagedTime = 0;
            FacingLeft = true;
            AttackRange = new Vector2(50, 50);
            AttackHitbox = new Rectangle((int)(Position.X - AttackRange.X), (int)Position.Y - 22, (int)AttackRange.X, (int)AttackRange.Y);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("BasicEnemy");
            animatedSprite = new AnimatedSprite(Sprite, 5, 1, 7, 30, 2);
            //Hitbox.Width = animatedSprite.FrameWidth;
            //Hitbox.Height = animatedSprite.FrameHeight;
            Hitbox.Width = 72;
            Hitbox.Height = 72;
        }

        public override void Update(GameTime gameTime)
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            Hitbox.X = (int)Position.X + HitboxDisplacement;
            Hitbox.Y = (int)Position.Y;
            if (focusedPlayer == null)
            {
                FacingLeft = true;
            }
            else if (focusedPlayer.Character.Hitbox.Center.X < Hitbox.Center.X)
            {
                FacingLeft = true;
            }
            else if (focusedPlayer.Character.Hitbox.Center.X > Hitbox.Center.X)
            {
                FacingLeft = false;
            }

            if (FacingLeft)
            {
                AttackHitbox.X = (int)(Hitbox.X - AttackRange.X);
            }
            else
            {
                // 72 is the width that the image actually takes up in the frame
                AttackHitbox.X = (int)Position.X + HitboxDisplacement + 72;
            }
            AttackHitbox.Y = Hitbox.Y;
            animatedSprite.Update();
            if (EnemyState != EnemyStates.Damaged)
            {
                ProcessDamage();
            }
            switch (EnemyState)
            {
                case EnemyStates.Idle:
                    foreach (Player player in PlayerManager.Players)
                    {
                        if (Math.Abs(player.Character.Hitbox.Center.X - Hitbox.Center.X) <= 200
                            && Math.Abs(player.Character.Hitbox.Center.Y - player.Character.Hitbox.Center.Y) <= 200)
                        {
                            focusedPlayer = player;
                            animatedSprite.SelectAnimation(4, 1);
                            EnemyState = EnemyStates.SeePlayer;
                        }
                    }
                    break;
                case EnemyStates.SeePlayer:
                    //input delay from SeePlayer state to Chase state
                    timeToChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeToChange >= 2F)
                    {
                        animatedSprite.SelectAnimation(3, 1);
                        EnemyState = EnemyStates.Chase;
                    }
                    break;
                case EnemyStates.Chase:
                    float distance = float.MaxValue;
                    foreach (Player player in PlayerManager.Players)
                    {
                        float playerDistance = Vector2.Distance(player.Character.Position, Position);
                        if (distance > playerDistance)
                        {
                            distance = playerDistance;
                            focusedPlayer = player;
                        }
                    }

                    if (AttackHitbox.Intersects(focusedPlayer.Character.Hitbox))
                    {
                        Velocity.X = 0;
                        Velocity.Y = 0;
                        animatedSprite.SelectAnimation(1, 2);
                        EnemyState = EnemyStates.Attack;
                        break;
                    }

                    if (FacingLeft && AttackHitbox.Left >= focusedPlayer.Character.Hitbox.Right)
                    {
                        Velocity.X = -2;
                    }
                    else if (!FacingLeft && AttackHitbox.Right <= focusedPlayer.Character.Hitbox.Left)
                    {
                        Velocity.X = 2;
                    }

                    if (AttackHitbox.Top >= focusedPlayer.Character.Hitbox.Bottom)
                    {
                        Velocity.Y = -2;
                    }
                    else if (AttackHitbox.Bottom <= focusedPlayer.Character.Hitbox.Top)
                    {
                        Velocity.Y = 2;
                    }
                    break;

                case EnemyStates.Attack:
                    if (!AttackHitbox.Intersects(focusedPlayer.Character.Hitbox))
                    {
                        animatedSprite.SelectAnimation(3, 1);
                        EnemyState = EnemyStates.Chase;
                    }
                    else
                    {
                        // attack here
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
                    DamagedTime += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (FacingLeft)
                    {
                        Position.X += 5;
                    }
                    else
                    {
                        Position.X -= 5;
                    }
                    if (DamagedTime > 20)
                    {
                        DamagedTime = 0;
                        animatedSprite.SelectAnimation(3, 1);
                        EnemyState = EnemyStates.Chase;
                    }
                    break;
                case EnemyStates.Dead:
                    MarkForDelete = true;
                    break;

                /* case CharacterStates.DefaultState:
                    ProcessMovement(Speed);
                     if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.Pass) == ActionStates.Pressed
                         && PlayerManager.Players.Count > 1 && HasBall)
                     {
                         CharacterState = CharacterStates.PassState;
                     }
                     break; 
                     */
            }

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
            animatedSprite.Draw(spriteBatch, Position, FacingLeft);
        }

        private void ProcessDamage()
        {
            foreach (Player player in PlayerManager.Players)
            {
                if (player.Character.AttackHitbox.Intersects(Hitbox) && player.Character.CharacterState == CharacterStates.AttackState)
                {
                    Health -= 10;
                    if (Health <= 0)
                    {
                        EnemyState = EnemyStates.Dead;
                    }
                    else
                    {
                        animatedSprite.SelectAnimation(7, 1);
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
                        Point targetCenter = ShootTarget.Sprite.Bounds.Center;
                        if (Hitbox.Contains(ball.DestinationPosition.X + targetCenter.X, ball.DestinationPosition.Y + targetCenter.Y))
                        {
                            // select damaged animation
                            Health -= 10;
                            if (Health <= 0)
                            {
                                EnemyState = EnemyStates.Dead;
                            }
                            else
                            {
                                animatedSprite.SelectAnimation(7, 1);
                                EnemyState = EnemyStates.Damaged;
                            }
                        }
                    }
                }
            }
        }
    }

}
