using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YellowMamba.Screens
{
    public class MainScreen : DrawableGameComponent
    {
        private ContentManager contentManager;
        private Texture2D background;
        private YellowMamba yellowMamba;

        public MainScreen(YellowMamba yellowMamba) : base(yellowMamba)
        {
            this.yellowMamba = yellowMamba;
            contentManager = new ContentManager(Game.Content.ServiceProvider, Game.Content.RootDirectory);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            background = contentManager.Load<Texture2D>("MainScreenBackground");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            yellowMamba.SpriteBatch.Begin();
            yellowMamba.SpriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            yellowMamba.SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            contentManager.Unload();
        }
    }
}
