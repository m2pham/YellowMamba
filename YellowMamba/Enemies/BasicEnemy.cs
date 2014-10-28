using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.AnimatedSprites;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;


namespace YellowMamba.Enemies
{
    public class BasicEnemy : Enemy
    {
        public float timeToChange;
        public BasicEnemy(PlayerManager playermanager)
            : base(playermanager)
        {
            Speed = 3;
            Health = 100;
            timeToChange = 0F;
            AttackRange = 50;
        }




        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("EnemySpriteSheet");
            animatedSprite = new AnimatedSprite(Sprite, 7, 1, 12, 30, 2);
            // Hitbox.Width = Sprite.Width;
            //Hitbox.Height = Sprite.Height;
        }


        public override void Update(GameTime gameTime)
        {
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            animatedSprite.Update();
            switch (EnemyState)
            {
                case EnemyStates.Idle:
                    foreach (Player player in PlayerManager.Players)
                    {
                        if (Math.Abs((player.Character.Position.X + player.Character.Sprite.Width) / 2 - (Position.X + 130) / 2) <= 200
                            && Math.Abs((player.Character.Position.Y + player.Character.Sprite.Height) / 2 - (Position.Y + 130) / 2) <= 200)
                        {
                            focusedPlayer = player;
                            EnemyState = EnemyStates.SeePlayer;
                        }
                    }

                    break;
                case EnemyStates.SeePlayer:

                    //input delay from SeePlayer state to Chase state
                    timeToChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeToChange >= 2F)
                        EnemyState = EnemyStates.Chase;

                    break;

                case EnemyStates.Chase:
                    if (Position.X > focusedPlayer.Character.Position.X + focusedPlayer.Character.Sprite.Width + AttackRange)
                    {
                        Velocity.X = -2;
                    }
                    else if (Position.X + 130 + AttackRange < focusedPlayer.Character.Position.X)
                    {
                        Velocity.X = 2;
                    }
                    else if ((Position.X < focusedPlayer.Character.Position.X + focusedPlayer.Character.Sprite.Width && Position.X > focusedPlayer.Character.Position.X)
                        || (Position.X + 130 > focusedPlayer.Character.Position.X && Position.X + 130 < focusedPlayer.Character.Position.X + focusedPlayer.Character.Sprite.Width))
                    {
                        if ((Position.X + 130) / 2 < (focusedPlayer.Character.Position.X + focusedPlayer.Character.Sprite.Width) / 2)
                        {
                            Velocity.X = -2;
                        }
                        else
                        {
                            Velocity.X = 2;
                        }
                    }
                    else
                    {
                        Velocity.X = 0;
                    }

                    if (Position.Y > focusedPlayer.Character.Position.Y + 50)
                    {
                        Velocity.Y = -2;
                    }
                    else if (Position.Y < focusedPlayer.Character.Position.Y)
                    {
                        Velocity.Y = 2;
                    }
                    else
                    {
                        Velocity.Y = 0;
                    }

                    if (Velocity.Y == 0 && Velocity.X == 0)
                    {
                        EnemyState = EnemyStates.Attack;
                    }
                    break;

                case EnemyStates.Attack:
                    if (!((Position.X <= focusedPlayer.Character.Position.X + focusedPlayer.Character.Sprite.Width + AttackRange && Position.X >= focusedPlayer.Character.Position.X)
                        || (Position.X + 130 + AttackRange >= focusedPlayer.Character.Position.X && Position.X + 130 <= focusedPlayer.Character.Position.X + focusedPlayer.Character.Sprite.Width)))
                    {
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

                case EnemyStates.Dead:
                    //if health == 0
                    //play death animation
                    //get rid of character
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
            switch (EnemyState)
            {
                case EnemyStates.Idle:
                    animatedSprite.Draw(spriteBatch, Position);
                    break;

                case EnemyStates.SeePlayer:
                    //if character is to the right, orient enemy towards player
                    if ((int)focusedPlayer.Character.Position.X > (int)Position.X)
                    {
                        animatedSprite.SelectAnimation(12, 1);
                    }
                    else
                    {
                        animatedSprite.SelectAnimation(11, 1);
                    }
                    animatedSprite.Draw(spriteBatch, Position);
                    break;

                case EnemyStates.Chase:
                    if ((int)focusedPlayer.Character.Position.X > (int)Position.X)
                    {
                        animatedSprite.SelectAnimation(6, 1);
                    }
                    else
                    {
                        animatedSprite.SelectAnimation(5, 1);
                    }
                    animatedSprite.Draw(spriteBatch, Position);
                    break;

                case EnemyStates.Attack:
                    if ((int)focusedPlayer.Character.Position.X > (int)Position.X)
                    {
                        animatedSprite.SelectAnimation(3, 2);
                    }
                    else
                    {
                        animatedSprite.SelectAnimation(1, 2);
                    }
                    animatedSprite.Draw(spriteBatch, Position);

                    break;

                case EnemyStates.SpecialAttack:

                    break;

                case EnemyStates.Retreat:

                    break;

                case EnemyStates.Dead:

                    break;
            }

        }
    }

}
