#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using YellowMamba.Screens;
using YellowMamba.Managers;
using YellowMamba.Characters;
using YellowMamba.Players;
using YellowMamba.Entities;
#endregion

namespace YellowMamba
{
    public class YellowMamba : Game
    {
        const int ScreenWidth = 1280;
        const int ScreenHeight = 720;
        const String AssetType = "Test";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Managers
        private InputManager inputManager;
        private ScreenManager screenManager;
        private PlayerManager playerManager;

        public YellowMamba() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;

            Content.RootDirectory = "Content/" + AssetType;

            inputManager = new InputManager();
            screenManager = new ScreenManager(inputManager);
            screenManager.ScreenWidth = ScreenWidth;
            screenManager.ScreenHeight = ScreenHeight;
            playerManager = new PlayerManager(inputManager);
        }

        protected override void Initialize()
        {
            inputManager.Initialize();

            screenManager.AddScreen(new MainScreen(Content.ServiceProvider, Content.RootDirectory, inputManager, screenManager, playerManager));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            inputManager.Update(gameTime);
            screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            screenManager.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

    }
}
