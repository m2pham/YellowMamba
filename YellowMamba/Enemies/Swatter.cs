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


//animations have not been changed.
//attack hitbox values and regular hotbox values not changed.
//need to make "swat" enemystate for swatter
namespace YellowMamba.Enemies
{
    public class Swatter : Enemy
    {
        private const int HitboxDisplacement = 50;
        public Rectangle DisruptPassHitbox;
        public Swatter(PlayerManager playermanager)
            : base(playermanager)
        {
            Speed = 3;
            Health = 30;
            FacingLeft = true;
            AttackRange = new Vector2(50, 50);
            AttackHitbox = new Rectangle((int)(Position.X - AttackRange.X), (int)Position.Y - 22, (int)AttackRange.X, (int)AttackRange.Y);
            //Hitbox that will swat passes away
            Vector2 DisruptPassRange = new Vector2(20, 100);
            DisruptPassHitbox = new Rectangle((int)(Position.X + DisruptPassRange.X), (int)(Position.Y + DisruptPassRange.Y), (int)DisruptPassRange.X, (int)DisruptPassRange.Y);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            SpriteSheet = new SpriteSheet(contentManager.Load<Texture2D>("BasicEnemy"), 1, 7);
            StandingAnimation = new Animation(SpriteSheet, 5, 2, 5);
            RunningAnimation = new Animation(SpriteSheet, 4, 1, 5);
            AttackingAnimation = new Animation(SpriteSheet, 1, 2, 5);
            DamagedAnimation = new Animation(SpriteSheet, 7, 1, 5);
            CurrentAnimation = StandingAnimation;
            //Hitbox.Width = animatedSprite.FrameWidth;
            //Hitbox.Height = animatedSprite.FrameHeight;
            
            //have to change according to sprite
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
                            //animation will be changed
                            //animatedSprite.SelectAnimation(4, 1);
                            EnemyState = EnemyStates.SeePlayer;
                        }
                    }
                    break;
                case EnemyStates.SeePlayer:
                    //input delay from SeePlayer state to Chase state
                    StateTimer += 1;
                    if (StateTimer >= 120)
                    {
                        //animatedSprite.SelectAnimation(3, 1);
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
                        //animatedSprite.SelectAnimation(1, 2);
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
                        if(Position.Y == focusedPlayer.Character.Position.Y)
                        {
                            Velocity.Y = 0;
                        }

                    }
                    else if (AttackHitbox.Bottom <= focusedPlayer.Character.Hitbox.Top)
                    {
                        Velocity.Y = 2;
                        if (Position.Y == focusedPlayer.Character.Position.Y)
                        {
                            Velocity.Y = 0;
                        }
                    }
                    break;

                case EnemyStates.Attack:
                    if (!AttackHitbox.Intersects(focusedPlayer.Character.Hitbox))
                    {
                        //animatedSprite.SelectAnimation(3, 1);
                        EnemyState = EnemyStates.Chase;
                    }
                    else
                    {
                        // attack here
                    }

                    break;
                /*case EnemyStates.Swat:
                    if ( )
                    {
                        ball.SourcePosition = Position;
                            ball.DestinationPosition = PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position;
                            ball.Velocity.X = (PlayerManager.GetPlayer(Player.PlayerIndex.get()).Target.Position.X - Position.X) / 60;
                            ball.Velocity.Y = -(PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position.Y - .5F * 60 * 60 / 2 - Position.Y) / 60;
                    }

                    break;*/

                case EnemyStates.Retreat:
                    /* if( Health <  30)
                     {
                         velocity of enemy is opposite direction of focusedPlayer
                     }*/
                    break;

                case EnemyStates.Damaged:
                    StateTimer += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (FacingLeft)
                    {
                        Position.X += 5;
                    }
                    else
                    {
                        Position.X -= 5;
                    }
                    if (StateTimer > 20)
                    {
                        StateTimer = 0;
                        //animatedSprite.SelectAnimation(3, 1);
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
            //spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
            CurrentAnimation.Draw(spriteBatch, Position, FacingLeft);
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
                        }
                    }

                    //bounce passball 
                    if (entity.GetType() == typeof(PassBall))
                    {
                        PassBall ball = (PassBall)entity;
                        Point ballCenter = PassBall.Sprite.Bounds.Center;
                        if(Hitbox.Contains(ball.Position.X + ballCenter.X, ball.Position.Y + ballCenter.Y))
                        {
                            //change angle of incidence
                            //maybe swap?
                            float tempX = ball.Velocity.X;
                            float tempY = ball.Velocity.Y;
                            ball.Velocity.X = tempX;
                            ball.Velocity.Y = tempY;

                        }
                    }
                }

                if(DisruptPassHitbox.Intersects(entity.Hitbox))
                {
                    //checks if the passball intersects with swatter's arm
                    if (entity.GetType() == typeof(PassBall))
                    {
                        PassBall ball = (PassBall)entity;
                        Point ballCenter = PassBall.Sprite.Bounds.Center;
                        /*if(DisruptPassHitbox.Contains(ball.Position.X + ballCenter.X, ball.Position.Y + ballCenter.Y) && ball.Knocked == false)
                        {
                            EnemyState = EnemyStates.Swat;
                            ball.Knocked = true;
                            ball.SourcePosition = Position;
                            ball.DestinationPosition = PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position;
                            ball.Velocity.X = (PlayerManager.GetPlayer(Player.PlayerIndex.get()).Target.Position.X - Position.X) / 60;
                            ball.Velocity.Y = -(PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position.Y - .5F * 60 * 60 / 2 - Position.Y) / 60;

                        }*/
                    }
                }
            }
        }
    }

}