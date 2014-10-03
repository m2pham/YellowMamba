using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Managers;

namespace YellowMamba.Characters
{
    public abstract class Character
    {
        protected PlayerIndex PlayerIndex { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected ContentManager ContentManager { get; private set; }
        protected Texture2D Sprite { get; set; }
        protected Vector2 Position;
        protected Vector2 Velocity;

        public Character(PlayerIndex playerIndex, InputManager inputManager, IServiceProvider serviceProvider, String contentRootDirectory)
        {
            PlayerIndex = playerIndex;
            InputManager = inputManager;
            ContentManager = new ContentManager(serviceProvider, contentRootDirectory);
            Position = new Vector2();
            Velocity = new Vector2();
        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
