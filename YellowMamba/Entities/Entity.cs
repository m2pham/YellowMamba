using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Utility;

namespace YellowMamba.Entities
{

    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float PositionZ, VelocityZ;
        protected float Speed { get; set; }
        public bool MarkForDelete { get; set; }
        public Rectangle Hitbox;

        public Entity()
        {
            Position = new Vector2();
            Velocity = new Vector2();
            Speed = 10;
            PositionZ = 0;
            VelocityZ = 0;
        }

        public abstract void LoadContent(ContentManager contentManager);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
