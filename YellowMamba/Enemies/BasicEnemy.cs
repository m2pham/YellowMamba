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
        private const int HitboxDisplacement = 50;
        private int waitTime, surroundDistance, inGoodPositionTimer;
        private bool waitFlag, attackFlag;
        public Animation SeePlayerAnimation { get; private set; }

        public BasicEnemy(PlayerManager playermanager)
            : base(playermanager)
        {
            surroundDistance = 300;
            Speed = 2;
            Health = 25;
            FacingLeft = true;
            Attack = 5;
            AttackWaitTime = 0;
            inGoodPositionTimer = 0;
            waitTime = GetRandomInt(80, 160);
            waitFlag = false;
            EnemyState = EnemyStates.Chase;
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
            DamagedAnimation = new Animation(SpriteSheet, 7, 1, 20);
            CurrentAnimation = StandingAnimation;
            //Hitbox.Width = animatedSprite.FrameWidth;
            //Hitbox.Height = animatedSprite.FrameHeight;
            Hitbox.Width = 72;
            Hitbox.Height = 72;
        }

        public override void Update(GameTime gameTime)
        {
            CurrentAnimation.Update(gameTime);
            if (EnemyState != EnemyStates.Damaged)
            {
                ProcessDamage();
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
            Hitbox.X = (int)Position.X + HitboxDisplacement;
            Hitbox.Y = (int)Position.Y;
            switch (EnemyState)
            {
                case EnemyStates.Chase:
                    if (Velocity.X != 0 || Velocity.Y != 0)
                    {
                        inGoodPositionTimer = 0;
                        SelectAnimation(RunningAnimation);
                    }
                    else if (inGoodPositionTimer <= 3)
                    {
                        inGoodPositionTimer++;
                    }
                    else
                    {
                        SelectAnimation(StandingAnimation);
                    }
                    if (waitTime > 0)
                    {
                        waitTime--;
                        return;
                    }
                    if (Position.X + Velocity.X >= 0 && Position.X + Hitbox.Width + Velocity.X <= 1280)
                    {
                        Position.X += Velocity.X;
                    }
                    if (Position.Y + Hitbox.Height + Velocity.Y >= 720/2 && Position.Y + Hitbox.Height + Velocity.Y <= 720)
                    {
                        Position.Y += Velocity.Y;
                    }
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

                    if (StateTimer > 0)
                    {
                        StateTimer -= 1;
                        if (StateTimer <= 0)
                        {
                            waitFlag = true;
                        }
                        MoveTowardsPlayer(Speed);
                    }
                    else if (attackFlag)
                    {
                        if (AttackHitbox.Intersects(focusedPlayer.Character.Hitbox) && focusedPlayer.Character.Hitbox.Bottom >= Hitbox.Bottom
                         && focusedPlayer.Character.Hitbox.Bottom < Hitbox.Bottom + 25)
                        {
                            Velocity.X = 0;
                            Velocity.Y = 0;
                            attackFlag = false;
                            EnemyState = EnemyStates.Attack;
                            break;
                        }
                        MoveTowardsPlayer(Speed);
                    }
                    else if (waitFlag)
                    {
                        waitTime = GetRandomInt(80, 160);
                        Velocity.X = 0;
                        Velocity.Y = 0;
                        waitFlag = false;
                        return;
                    }
                    else if (Vector2.Distance(Position, focusedPlayer.Character.Position) <= surroundDistance)
                    {
                        if (GetRandomInt(0, 120) == 1)
                        {
                            attackFlag = true;
                        }
                        else
                        {
                            bool enemyTooClose = false;
                            Vector2 distanceFromPlayer = new Vector2(Position.X - focusedPlayer.Character.Position.X, Position.Y - focusedPlayer.Character.Position.Y);
                            float xSpeedFactor = (surroundDistance - distanceFromPlayer.X) / (surroundDistance * 2);
                            float ySpeedFactor = (surroundDistance - distanceFromPlayer.Y) / (surroundDistance * 2);
                            foreach (Enemy enemy in PlayerManager.EnemyManager.Enemies)
                            {
                                if (!enemy.Equals(this) && Vector2.Distance(enemy.Position, Position) <= 75)
                                {
                                    enemyTooClose = true;
                                    if ((distanceFromPlayer.X > 0 && distanceFromPlayer.Y > 0)
                                        || (distanceFromPlayer.X < 0 && distanceFromPlayer.Y < 0))
                                    {

                                        Velocity.Y = Math.Sign(Position.Y - enemy.Position.Y) * Speed * ySpeedFactor;
                                        Velocity.X = Math.Sign(Velocity.Y) * Speed * xSpeedFactor;
                                    }
                                    else if ((distanceFromPlayer.X > 0 && distanceFromPlayer.Y < 0)
                                        || (distanceFromPlayer.X < 0 && distanceFromPlayer.Y > 0))
                                    {
                                        Velocity.Y = Math.Sign(Position.Y - enemy.Position.Y) * Speed * ySpeedFactor;
                                        Velocity.X = -Math.Sign(Velocity.Y) * Speed * xSpeedFactor;
                                    }
                                    else if (distanceFromPlayer.X == 0)
                                    {
                                        if (GetRandomInt(0, 1) == 1)
                                        {
                                            Velocity.X = -Speed * xSpeedFactor;
                                        }
                                        else
                                        {
                                            Velocity.X = Speed * xSpeedFactor;
                                        }
                                    }
                                    else if (distanceFromPlayer.Y == 0)
                                    {
                                        if (GetRandomInt(0, 1) == 1)
                                        {
                                            Velocity.Y = -Speed * ySpeedFactor;
                                        }
                                        else
                                        {
                                            Velocity.Y = Speed * ySpeedFactor;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (!enemyTooClose)
                            {
                                if (Vector2.Distance(Position, focusedPlayer.Character.Position) <= surroundDistance / 2)
                                {
                                    Velocity.X = Math.Sign(distanceFromPlayer.X) * Speed * xSpeedFactor;
                                    Velocity.Y = Math.Sign(distanceFromPlayer.Y) * Speed * ySpeedFactor;
                                }
                                else
                                {
                                    inGoodPositionTimer++;
                                    Velocity.X = 0;
                                    Velocity.Y = 0;
                                }
                            }
                        }
                    }
                    else if (AttackHitbox.Intersects(focusedPlayer.Character.Hitbox) && focusedPlayer.Character.Hitbox.Bottom >= Hitbox.Bottom
                         && focusedPlayer.Character.Hitbox.Bottom < Hitbox.Bottom + 25)
                    {
                        Velocity.X = 0;
                        Velocity.Y = 0;
                        attackFlag = false;
                        EnemyState = EnemyStates.Attack;
                        break;
                    }
                    else
                    {
                        StateTimer = GetRandomInt(45, 90);
                    }
                    break;

                case EnemyStates.Attack:
                    if (!AttackHitbox.Intersects(focusedPlayer.Character.Hitbox))
                    {
                        EnemyState = EnemyStates.Chase;
                    }
                    else
                    {
                        AttackWaitTime -= (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                        if (AttackWaitTime <= 0)
                        {
                            AttackWaitTime = PlayerManager.EnemyManager.RandomGen.Next(30, 90);
                            SelectAnimation(AttackingAnimation);
                            EnemyState = EnemyStates.Attacking;
                        }
                    }
                    break;
                case EnemyStates.Attacking:
                    StateTimer += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (StateTimer == AttackingAnimation.NumFrames * AttackingAnimation.Frequency / 2)
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
                        if (GetRandomInt(0, 60) == 1)
                        {
                            SelectAnimation(StandingAnimation);
                            EnemyState = EnemyStates.Attack;
                        }
                        {
                            EnemyState = EnemyStates.Chase;
                        }
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
                    StateTimer += 1;
                    if (FacingLeft)
                    {
                        Position.X += 5;
                    }
                    else
                    {
                        Position.X -= 5;
                    }
                    if (StateTimer > DamagedAnimation.NumFrames * DamagedAnimation.Frequency)
                    {
                        StateTimer = 0;
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

        private int GetRandomInt(int low, int high)
        {
            return PlayerManager.EnemyManager.RandomGen.Next(low, high);
        }

        private void MoveTowardsPlayer(float Speed)
        {
            if ((FacingLeft && AttackHitbox.Left >= focusedPlayer.Character.Hitbox.Right)
                || (!FacingLeft && AttackHitbox.Left >= focusedPlayer.Character.Hitbox.Right))
            {
                Velocity.X = -Speed;
            }
            else if ((!FacingLeft && AttackHitbox.Right <= focusedPlayer.Character.Hitbox.Left)
                || (FacingLeft && AttackHitbox.Right <= focusedPlayer.Character.Hitbox.Left))
            {
                Velocity.X = Speed;
            }

            if (Hitbox.Bottom >= focusedPlayer.Character.Hitbox.Bottom + 10)
            {
                Velocity.Y = -Speed;
            }
            else if (Hitbox.Bottom <= focusedPlayer.Character.Hitbox.Bottom - 10)
            {
                Velocity.Y = Speed;
            }
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
                        waitTime = 0;
                        StateTimer = 0;
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
                                waitTime = 0;
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
