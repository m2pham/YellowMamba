using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Characters;
using YellowMamba.Enemies;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;

namespace YellowMamba.Screens
{
    public class StageOnePartOneScreen : Screen
    {
        private Texture2D background, black;
        private SpriteFont spriteFont;
        private EntityManager entityManager;
        private EnemyManager enemyManager;

        private TimeSpan TransitionInTime = new TimeSpan(0, 0, 1);
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 2);

        private Rectangle backgroundRectangle;
        private Rectangle wrapAroundRectangle;
        private int areaCounter = 1;
        public Song stage1Song;
        private bool wrapAround;
        private int wrapAroundXDest;

        private float alpha = 0;
        
        public StageOnePartOneScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {
            entityManager = new EntityManager();
            playerManager.EntityManager = entityManager;
            enemyManager = new EnemyManager(playerManager, entityManager);
            playerManager.EnemyManager = enemyManager;
            wrapAroundXDest = 1280;
            wrapAroundRectangle = new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            backgroundRectangle = new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
        }

        public override void Initialize()
        {
            entityManager.Entities.Add(new PassBall());
            entityManager.Entities.Add(new ShootBall());
            entityManager.Entities.Add(new ShootTarget(InputManager, new Player(PlayerIndex.One, InputManager, PlayerManager)));
            for (int i = 0; i < 3; i++)
            {
                BasicEnemy basicEnemy = new BasicEnemy(PlayerManager);
                basicEnemy.Position.X = 900;
                basicEnemy.Position.Y = (i * 60) + 275;
                enemyManager.Enemies.Add(basicEnemy);
            }
            for (int i = 0; i < 4; i++)
            {
                BasicEnemy basicEnemy = new BasicEnemy(PlayerManager);
                basicEnemy.Position.X = 1100;
                basicEnemy.Position.Y = (i * 60) + 225;
                enemyManager.Enemies.Add(basicEnemy);
            }
            /*BasicEnemy basicEnemy = new BasicEnemy(PlayerManager);
            basicEnemy.Position.X = 900;
            basicEnemy.Position.Y = 375;
            enemyManager.Enemies.Add(basicEnemy);*/
        }

        public override void LoadContent()
        {
            background = ContentManager.Load<Texture2D>("StageOnePartOneScreenBackground");
            stage1Song = ContentManager.Load<Song>("Music/stage1Song");
            black = ContentManager.Load<Texture2D>("Black");
            spriteFont = ContentManager.Load<SpriteFont>("TestFont");
            //MediaPlayer.Play(stage1Song); 
           
            PlayerManager.LoadContent(ContentManager);
            foreach (Entity entity in entityManager.Entities)
                entity.LoadContent(ContentManager);

            enemyManager.LoadContent(ContentManager);
            
            entityManager.Entities.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            switch (ScreenState)
            {
                case ScreenStates.TransitionIn:
                    if (!IsTransitionDone(gameTime.TotalGameTime, TransitionInTime))
                    {

                    }
                    else
                    {
                        ScreenState = ScreenStates.Active;
                    }
                    break;
                case ScreenStates.TransitionOut:
                    if (!IsTransitionDone(gameTime.TotalGameTime, TransitionOutTime))
                    {
                        alpha += 127 / (float)(TransitionOutTime.TotalMilliseconds / gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    break;
                case ScreenStates.TransitionNextArea:
                    if (wrapAround)
                    {
                        wrapAroundXDest -= 10;
                    }
                    backgroundRectangle.X += 10;
                    foreach (Player player in PlayerManager.Players)
                    {
                        // move player back to left
                        player.Character.Position.X -= 7.5F;
                        player.Character.DrawHealthBar = false;
                        player.Character.CurrentAnimation.Update(gameTime);
                    }
                    foreach (Enemy enemy in enemyManager.Enemies)
                    {
                        enemy.Position.X -= 10;
                    }
                    if (wrapAround && wrapAroundXDest == 0)
                    {
                        backgroundRectangle = new Rectangle(0, 0, 1280, 720);
                        wrapAround = false;
                        ScreenState = ScreenStates.Active;
                    }
                    else if (backgroundRectangle.X == 1280 * (areaCounter - 1))
                    {
                        ScreenState = ScreenStates.Active;
                    }
                    break;
                case ScreenStates.Active:
                    bool allPlayersDead = true;
                    bool allPlayersOnRightEdge = true;
                    foreach (Player player in PlayerManager.Players)
                    {
                        player.Character.DrawHealthBar = true;
                        if (!player.Character.MarkForDelete)
                        {
                            allPlayersDead = false;
                        }
                        if (player.Character.Position.X < 1000)
                        {
                            allPlayersOnRightEdge = false;
                        }
                    }
                    if(allPlayersDead)
                    {
                        ScreenState = ScreenStates.TransitionOut;
                    }

                    if (enemyManager.Enemies.Count == 0 && allPlayersOnRightEdge && areaCounter == 1)
                    {
                        areaCounter++;
                        foreach (Player player in PlayerManager.Players)
                        {
                            player.Character.FacingLeft = false;
                            player.Character.SelectAnimation(player.Character.RunningAnimation);
                            player.Character.Health = Math.Max(player.Character.Health + player.Character.MaxHealth/3, player.Character.MaxHealth);
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            Chaser basicEnemy = new Chaser(PlayerManager);
                            basicEnemy.Position.X = (i * 50) + 800 + 1280;
                            basicEnemy.Position.Y = (i * 100) + 275;
                            enemyManager.Enemies.Add(basicEnemy);
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            Chaser chaser = new Chaser(PlayerManager);
                            chaser.Position.X = (i * 50) + 1000 + 1280;
                            chaser.Position.Y = (i * 100) + 225;
                            enemyManager.Enemies.Add(chaser);
                        } 
                        enemyManager.LoadContent(ContentManager);
                        ScreenState = ScreenStates.TransitionNextArea;
                        break;
                    }
                    else if (enemyManager.Enemies.Count == 0 && allPlayersOnRightEdge && areaCounter == 2)
                    {
                        areaCounter++;
                        foreach (Player player in PlayerManager.Players)
                        {
                            player.Character.FacingLeft = false;
                            player.Character.SelectAnimation(player.Character.RunningAnimation);
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            BasicEnemy chaser = new BasicEnemy(PlayerManager);
                            chaser.Position.X = (i * 50) + 800 + 1280;
                            chaser.Position.Y = (i * 100) + 275;
                            enemyManager.Enemies.Add(chaser);
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            Chaser chaser = new Chaser(PlayerManager);
                            chaser.Position.X = (i * 50) + 1000 + 1280;
                            chaser.Position.Y = (i * 100) + 225;
                            enemyManager.Enemies.Add(chaser);
                        }
                        enemyManager.LoadContent(ContentManager);
                        ScreenState = ScreenStates.TransitionNextArea;
                        break;
                    }
                    else if (enemyManager.Enemies.Count == 0 && allPlayersOnRightEdge && areaCounter == 3)
                    {
                        areaCounter = 1;
                        wrapAround = true;
                        foreach (Player player in PlayerManager.Players)
                        {
                            player.Character.FacingLeft = false;
                            player.Character.SelectAnimation(player.Character.RunningAnimation);
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            BasicEnemy chaser = new BasicEnemy(PlayerManager);
                            chaser.Position.X = (i * 50) + 800 + 1280;
                            chaser.Position.Y = (i * 100) + 275;
                            enemyManager.Enemies.Add(chaser);
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            BasicEnemy chaser = new BasicEnemy(PlayerManager);
                            chaser.Position.X = (i * 50) + 1000 + 1280;
                            chaser.Position.Y = (i * 100) + 225;
                            enemyManager.Enemies.Add(chaser);
                        }
                        enemyManager.LoadContent(ContentManager);
                        ScreenState = ScreenStates.TransitionNextArea;
                        break;
                    }

                    PlayerManager.Update(gameTime);
                    entityManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            switch (ScreenState)
            {
                case ScreenStates.TransitionIn:
                    // transition in animation here
                    break;
                case ScreenStates.TransitionOut:
                    {
                        spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), backgroundRectangle, Color.White);
                        List<Entity> allEntities = PlayerManager.Characters.Concat<Entity>(enemyManager.Enemies).Concat<Entity>(entityManager.Entities).ToList();
                        allEntities = allEntities.OrderBy(e => e.Hitbox.Bottom).ToList();
                        foreach (Entity entity in allEntities)
                        {
                            entity.Draw(gameTime, spriteBatch);
                        }
                        foreach (Player player in PlayerManager.Players)
                        {
                            if (player.Target.Visible)
                            {
                                player.Target.Draw(gameTime, spriteBatch);
                            }
                        }
                        spriteBatch.Draw(black, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), new Color(255, 255, 255, (byte)alpha * 2));
                        spriteBatch.DrawString(spriteFont, "GAME OVER", new Vector2(ScreenManager.ScreenWidth / 2 - 100, ScreenManager.ScreenHeight / 2), Color.White);
                    }
                    break;
                case ScreenStates.TransitionNextArea:
                    {
                        spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), backgroundRectangle, Color.White);
                        if (wrapAround)
                        {
                            spriteBatch.Draw(background, new Rectangle(wrapAroundXDest, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), wrapAroundRectangle, Color.White);
                        }
                        List<Entity> allEntities = (PlayerManager.Characters.Concat<Entity>(enemyManager.Enemies)).ToList();
                        allEntities = allEntities.OrderBy(e => e.Position.Y).ToList();
                        foreach (Entity entity in allEntities)
                        {
                            entity.Draw(gameTime, spriteBatch);
                        }

                        //PlayerManager.Draw(gameTime, spriteBatch);
                        //enemyManager.Draw(gameTime, spriteBatch);
                    }
                    break;
                case ScreenStates.Active:
                    {
                        spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), backgroundRectangle, Color.White);

                        List<Entity> allEntities = PlayerManager.Characters.Concat<Entity>(enemyManager.Enemies).Concat<Entity>(entityManager.Entities).ToList();
                        allEntities = allEntities.OrderBy(e => e.Hitbox.Bottom).ToList();
                        foreach (Entity entity in allEntities)
                        {
                            entity.Draw(gameTime, spriteBatch);
                        }
                        foreach (Player player in PlayerManager.Players)
                        {
                            if (player.Target.Visible)
                            {
                                player.Target.Draw(gameTime, spriteBatch);
                            }
                        }
                        //PlayerManager.Draw(gameTime, spriteBatch);
                        //enemyManager.Draw(gameTime, spriteBatch);
                        //entityManager.Draw(gameTime, spriteBatch);
                    }
                    break;
            }
            PlayerManager.DrawScore(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            ContentManager.Unload();
        }
    }
}
