using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private Texture2D background;
        private EntityManager entityManager;
        private EnemyManager enemyManager;

        private TimeSpan TransitionInTime = new TimeSpan(0, 0, 1);
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 1);

        private Rectangle backgroundRectangle;
        private int areaCounter = 1;

        public StageOnePartOneScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {
            entityManager = new EntityManager();
            playerManager.EntityManager = entityManager;
            enemyManager = new EnemyManager(playerManager, entityManager);
            playerManager.EnemyManager = enemyManager;
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
                basicEnemy.Position.Y = (i * 100) + 375;
                enemyManager.Enemies.Add(basicEnemy);
            }
            for (int i = 0; i < 4; i++)
            {
                BasicEnemy basicEnemy = new BasicEnemy(PlayerManager);
                basicEnemy.Position.X = 1100;
                basicEnemy.Position.Y = (i * 100) + 325;
                enemyManager.Enemies.Add(basicEnemy);
            }
        }

        public override void LoadContent()
        {
            background = ContentManager.Load<Texture2D>("StageOnePartOneScreenBackground");
            PlayerManager.LoadContent(ContentManager);
            foreach (Entity entity in entityManager.Entities)
                entity.LoadContent(ContentManager);

            foreach (Enemy enemy in enemyManager.Enemies)
                enemy.LoadContent(ContentManager);
            
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
                        // transition out logic
                    }
                    else
                    {
                        ScreenManager.RemoveScreen(this);
                        ScreenManager.AddScreen(NextScreen);
                    }
                    break;
                case ScreenStates.TransitionNextArea:
                    backgroundRectangle.X += 10;
                    foreach (Player player in PlayerManager.Players)
                    {
                        // move player back to left
                        player.Character.Position.X -= 7.5F;
                        player.Character.CurrentAnimation.Update(gameTime);
                    }
                    foreach (Enemy enemy in enemyManager.Enemies)
                    {
                        enemy.Position.X -= 10;
                    }
                    if (backgroundRectangle.X == 1280 * (areaCounter - 1))
                    {
                        ScreenState = ScreenStates.Active;
                    }
                    break;
                case ScreenStates.Active:
                    bool allPlayersOnRightEdge = true;
                    foreach (Player player in PlayerManager.Players)
                    {
                        if (player.Character.Position.X < 1000)
                        {
                            allPlayersOnRightEdge = false;
                            break;
                        }
                    }
                    if (enemyManager.Enemies.Count == 0 && allPlayersOnRightEdge && areaCounter < 2)
                    {
                        areaCounter++;
                        foreach (Player player in PlayerManager.Players)
                        {
                            player.Character.FacingLeft = false;
                            player.Character.SelectAnimation(player.Character.RunningAnimation);
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            Chaser chaser = new Chaser(PlayerManager);
                            chaser.Position.X = (i * 50) + 800 + 1280;
                            chaser.Position.Y = (i * 100) + 375;
                            enemyManager.Enemies.Add(chaser);
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            Chaser chaser = new Chaser(PlayerManager);
                            chaser.Position.X = (i * 50) + 1000 + 1280;
                            chaser.Position.Y = (i * 100) + 325;
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
                    // transition out animation here
                    break;
                case ScreenStates.TransitionNextArea:
                    {
                        spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), backgroundRectangle, Color.White);
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
                        allEntities = allEntities.OrderBy(e => e.Position.Y).ToList();
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
            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            ContentManager.Unload();
        }
    }
}
