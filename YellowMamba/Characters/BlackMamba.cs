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
using YellowMamba.AnimatedSprites;
using YellowMamba.Enemies;

namespace YellowMamba.Characters
{
    public class BlackMamba : Character
    {
        private const int HitboxDisplacement = 16;

        public BlackMamba(Player player, InputManager inputManager, PlayerManager playerManager)
            : base(player, inputManager, playerManager)
        {
            Speed = 5;
            HasBall = true;
            FacingLeft = false;
            AttackRange = new Vector2(30, 30);
            AttackHitbox = new Rectangle((int)(Position.X + HitboxDisplacement + 36), (int)Position.Y - 14, (int)AttackRange.X, (int)AttackRange.Y);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("BlackMamba/BlackMamba");
            animatedSprite = new AnimatedSprite(contentManager.Load<Texture2D>("BlackMambaSpriteSheet"), 8, 11, 7, 5, 4);
            Hitbox.Width = 30;
            Hitbox.Height = 70;
        }

        public override void Update(GameTime gameTime)
        {
            Hitbox.X = (int) Position.X + 17;
            Hitbox.Y = (int) Position.Y - 14;
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
            animatedSprite.Update();
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
                        ShootBall shootBall = new ShootBall();
                        shootBall.SourcePosition = Position;
                        shootBall.DestinationPosition = PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position;
                        shootBall.Velocity.X = (PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position.X - Position.X) / 60;
                        shootBall.Velocity.Y = -(PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position.Y - .5F * 60 * 60 / 2 - Position.Y) / 60;
                        shootBall.ReleaseTime = gameTime.TotalGameTime;
                        PlayerManager.EntityManager.Entities.Add(shootBall);
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Visible = false;
                        animatedSprite.SelectAnimation(1, 4);
                        CharacterState = CharacterStates.ShootingState;
                    }
                    break;
                case CharacterStates.ShootingState:
                    ShootingTime += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (ShootingTime > 20)
                    {
                        ShootingTime = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.PickState:
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pick) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        break;
                    }
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pick) == ActionStates.Pressed)
                    {

                        //draw pick animation 
                    }
                    break;
                case CharacterStates.PassState:
                    ProcessMovement(Speed);
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pass) != ActionStates.Held)
                    {
                        CharacterState = CharacterStates.DefaultState;
                        break;
                    }

                    LinkedListNode<CharacterActions> passButtonNode = InputManager.PassButtons.First;
                    foreach (Player targetPlayer in PlayerManager.Players)
                    {
                        if (targetPlayer.PlayerIndex == Player.PlayerIndex)
                        {
                            continue;
                        }
                        if (InputManager.GetCharacterActionState(Player.PlayerIndex, passButtonNode.Value) == ActionStates.Pressed)
                        {
                            PassBall ball = new PassBall();
                            ball.Position = Position;
                            ball.SourcePlayer = Player;
                            ball.TargetPlayer = targetPlayer;
                            ball.ReleaseTime = gameTime.TotalGameTime;
                            PlayerManager.EntityManager.Entities.Add(ball);
                            HasBall = false;
                            CharacterState = CharacterStates.DefaultState;
                        }
                    }
                    break;
                case CharacterStates.AttackState:
                    AttackingTime += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (AttackingTime > 15)
                    {
                        AttackingTime = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.DamagedState:
                    DamagedTime += (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                    if (DamagedTime > 20)
                    {
                        DamagedTime = 0;
                        CharacterState = CharacterStates.DefaultState;
                    }
                    break;
                case CharacterStates.DefaultState:
                    ProcessMovement(Speed);
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
                        animatedSprite.SelectAnimation(22, 5);
                    }
                    else
                    {
                        animatedSprite.SelectAnimation(8, 4);
                    }
                    if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pass) == ActionStates.Pressed
                        && PlayerManager.Players.Count > 1 && HasBall)
                    {
                        CharacterState = CharacterStates.PassState;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.ShootMode) == ActionStates.Pressed)
                    {
                        CharacterState = CharacterStates.ShootState;
                        animatedSprite.SelectAnimation(64, 1);
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Position = Position;
                        PlayerManager.GetPlayer(Player.PlayerIndex).Target.Visible = true;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Attack) == ActionStates.Pressed)
                    {
                        animatedSprite.SelectAnimation(15, 3);
                        CharacterState = CharacterStates.AttackState;
                    }
                    else if (InputManager.GetCharacterActionState(Player.PlayerIndex, CharacterActions.Pick) == ActionStates.Pressed)
                    {
                        animatedSprite.SelectAnimation(71, 1);
                        CharacterState = CharacterStates.PickState;
                    }
                    break;
            }
        }

        private void CheckCollision()
        {
            foreach (Entity entity in PlayerManager.EntityManager.Entities.ToList())
            {
                if (Hitbox.Intersects(entity.Hitbox))
                {
                    if (entity.GetType() == typeof(PassBall))
                    {
                        PassBall ball = (PassBall)entity;
                        if (!ball.InFlight || ball.SourcePlayer.PlayerIndex != Player.PlayerIndex)
                        {
                            HasBall = true;
                            entity.MarkForDelete = true;
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
                if (Hitbox.Intersects(enemy.AttackHitbox) && enemy.EnemyState == EnemyStates.Attack)
                {
                    if (CharacterState == CharacterStates.PickState)
                    {
                        // take some reduced damage
                        // flash pretty sparkles
                    }
                    else
                    {
                        // take regular damage
                        // animatedSprite.SelectAnimation(78, 4);
                        // CharacterState = CharacterStates.DamagedState;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animatedSprite.Draw(spriteBatch, Position, FacingLeft);
        }
    }
}
