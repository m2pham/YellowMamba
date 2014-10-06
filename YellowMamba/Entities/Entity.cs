using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Entities;
using YellowMamba.Managers;

namespace YellowMamba.Entities
{

    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;
        protected int Speed { get; set; }
        public Rectangle Hitbox;

        public Entity()
        {
            Position = new Vector2();
            Velocity = new Vector2();
        }

        public abstract void LoadContent(ContentManager contentManager);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
