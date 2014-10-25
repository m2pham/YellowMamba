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
        public float timeToChange = 0;
         public BasicEnemy(PlayerManager playermanager) : base(playermanager)
         {
             Speed = 3;
             Health = 100;
         }

         

    
        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("testSpriteSheet");
            animatedSprite = new AnimatedSprite(Sprite, 1, 3, 3, 30, 2);
           // Hitbox.Width = Sprite.Width;
            //Hitbox.Height = Sprite.Height;
        }


        public override void Update(GameTime gameTime)
        {
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
             switch (EnemyState)
            {
                case EnemyStates.Idle:
                    foreach (Player player in PlayerManager.Players)
                    {
                        //play idle animation if 
                        if (Math.Abs((int)player.Character.Position.X - (int)Position.X) <= 300 || Math.Abs(player.Character.Position.Y - Position.Y) <= 300)
                        {
                            focusedPlayer = player;
                            EnemyState = EnemyStates.SeePlayer;
                        }
                    }
 
                    break;
                case EnemyStates.SeePlayer:

                     //input delay from SeePlayer state to Chase state
                    timeToChange += (float) gameTime.ElapsedGameTime.TotalSeconds;
                     if (timeToChange == 2)
                        EnemyState = EnemyStates.Chase;

                    break;

                case EnemyStates.Chase:
                    if (Math.Abs((int)focusedPlayer.Character.Position.X - (int)Position.X) <= 300 || Math.Abs(focusedPlayer.Character.Position.Y - Position.Y) <= 300)
                    {
                        Velocity.X = (focusedPlayer.Character.Position.X - Position.X) / 30;
                        Velocity.Y = (focusedPlayer.Character.Position.Y - Position.Y) / 30;
                    }

                    else if ( (Math.Abs((int)focusedPlayer.Character.Position.X - (int)Position.X) <= 10) && (Math.Abs((int)focusedPlayer.Character.Position.Y - (int)Position.Y) <= 5) )
                    {
                        EnemyState = EnemyStates.Attack;
                    }
                    break;
                
                case EnemyStates.Attack:
                    if(  ( (Math.Abs((int)focusedPlayer.Character.Position.X - (int)Position.X) >= 10) && (Math.Abs((int)focusedPlayer.Character.Position.Y - (int)Position.Y) > 5) ))
                    {
                        EnemyState = EnemyStates.Chase;
                    }
                    else if ((Math.Abs((int)focusedPlayer.Character.Position.X - (int)Position.X) <= 10) && (Math.Abs((int)focusedPlayer.Character.Position.Y - (int)Position.Y) <= 5))
                    {
                        //get this nigga to strike
                    }

                    break;
                case EnemyStates.SpecialAttack:

                    break;

                case EnemyStates.Retreat:
                   /* if( Health <  30)
                    {
                        velocity of enemy is opposite direction of focusedPlayer
                    }*/                    break;

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
                    animatedSprite.currentFrame = 4;
                    animatedSprite.endingFrame = 4;
                    //if character is to the left, orient enemy towards player
                     if((int)focusedPlayer.Character.Position.X > (int)Position.X)
                     {
                         animatedSprite.currentFrame = 8;
                         animatedSprite.endingFrame = 8;
                     }
                     animatedSprite.Draw(spriteBatch, Position);
                break;

                case EnemyStates.Chase:
                    animatedSprite.currentFrame = 2;
                    animatedSprite.endingFrame = 2;
                    if ((int)focusedPlayer.Character.Position.X > (int)Position.X)
                    {
                        animatedSprite.currentFrame = 1;
                        animatedSprite.endingFrame = 1;
                    }

                break;

                case EnemyStates.Attack:

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
