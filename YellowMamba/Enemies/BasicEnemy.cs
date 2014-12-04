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
    public class BasicEnemy : Enemy
    {
        public float timeToChange;
        private const int HitboxDisplacement = 50;
        public Animation SeePlayerAnimation { get; private set; }

        public BasicEnemy(PlayerManager playermanager)
            : base(playermanager)
        {
            Speed = 3;
            Health = 25;
            timeToChange = 0F;
            DamagedTime = 0;
            FacingLeft = true;
            Attack = 5;
            AttackWaitTime = 0;
            AttackRange = new Vector2(40, 40);
            AttackHitbox = new Rectangle((int)(Position.X - AttackRange.X), (int)Position.Y - 22, (int)AttackRange.X, (int)AttackRange.Y);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            SpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("BasicEnemy"), 1, 7);
            StandingAnimation = new Animation(SpriteSheet, 5, 2, 5);
            RunningAnimation = new Animation(SpriteSheet, 3, 1, 5);
            AttackingAnimation = new Animation(SpriteSheet, 1, 2, 5);
            SeePlayerAnimation = new Animation(SpriteSheet, 4, 1, 5);
            DamagedAnimation = new Animation(SpriteSheet, 7, 1, 5);
            CurrentAnimation = StandingAnimation;
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
            foreach (Player player in PlayerManager.Players)
            {
                if (player.Character.PickAggroBox.Intersects(Hitbox) && player.Character.Defending)
                {
                    focusedPlayer = player;
                }
            }
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
            CurrentAnimation.Update(gameTime);
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
                            SelectAnimation(SeePlayerAnimation);
                            EnemyState = EnemyStates.SeePlayer;
                        }
                    }
                    break;
                case EnemyStates.SeePlayer:
                    //input delay from SeePlayer state to Chase state
                    timeToChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeToChange >= 2F)
                    {
                        SelectAnimation(RunningAnimation);
                        EnemyState = EnemyStates.Chase;
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

                    if (AttackHitbox.Intersects(focusedPlayer.Character.Hitbox) && focusedPlayer.Character.Hitbox.Bottom > Hitbox.Bottom
                        && focusedPlayer.Character.Hitbox.Bottom < Hitbox.Bottom + 25)
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

                    if (Hitbox.Bottom >= focusedPlayer.Character.Hitbox.Bottom + 25)
                    {
                        Velocity.Y = -2;
                    }
                    else if (Hitbox.Bottom <= focusedPlayer.Character.Hitbox.Bottom - 25)
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
                    AttackingTime += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (AttackingTime == 5)
                    {
                        AttackVisible = true;
                    }
                    else
                    {
                        AttackVisible = false;
                    }
                    if (AttackingTime > 10)
                    {
                        AttackingTime = 0;
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
                        SelectAnimation(StandingAnimation);
                        EnemyState = EnemyStates.Chase;
                    }
                    break;
                case EnemyStates.Dead:
                    //input animation
                    //animatedSprite.SelectAnimation(x, x);
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
            CurrentAnimation.Draw(spriteBatch, Position, FacingLeft);
        }

        private void ProcessDamage()
        {
            foreach (Player player in PlayerManager.Players)
            {
                if (player.Character.AttackHitbox.Intersects(Hitbox) && player.Character.CharacterState == CharacterStates.AttackState
                    && player.Character.Hitbox.Bottom > Hitbox.Bottom && player.Character.Hitbox.Bottom < Hitbox.Bottom + 25)
                {
                    Health -= player.Character.Attack;
                    if (Health <= 0)
                    {
                        EnemyState = EnemyStates.Dead;
                    }
                    else
                    {
                        SelectAnimation(DamagedAnimation);
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
                            Health -= PlayerManager.GetPlayer(ball.SourcePlayer).Character.ShootAttack;
                            if (Health <= 0)
                            {
                                EnemyState = EnemyStates.Dead;
                            }
                            else
                            {
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
