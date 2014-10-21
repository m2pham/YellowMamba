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

namespace YellowMamba.Screens
{
    public class StageOnePartOneScreen : Screen
    {
        private Texture2D background;
        private EntityManager entityManager;
        private EnemyManager enemyManager;

        private TimeSpan TransitionInTime = new TimeSpan(0, 0, 1);
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 1);

        public StageOnePartOneScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {
            entityManager = new EntityManager();
            playerManager.EntityManager = entityManager;
            enemyManager = new EnemyManager(playerManager, entityManager);
        }

        public override void Initialize()
        {
            entityManager.Entities.Add(new PassBall());
            entityManager.Entities.Add(new ShootBall());
            entityManager.Entities.Add(new ShootTarget(InputManager, PlayerIndex.One));
        }

        public override void LoadContent()
        {
            background = ContentManager.Load<Texture2D>("StageOnePartOneScreenBackground");
            PlayerManager.LoadContent(ContentManager);
            foreach (Entity entity in entityManager.Entities)
                entity.LoadContent(ContentManager);

            // foreach (Enemy enemy in enemyManager.Enemies)
            
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
                case ScreenStates.Active:
                    PlayerManager.Update(gameTime);
                    entityManager.Update(gameTime);
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
                case ScreenStates.Active:
                    spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), Color.White);
                    PlayerManager.Draw(gameTime, spriteBatch);
                    entityManager.Draw(gameTime, spriteBatch);
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
