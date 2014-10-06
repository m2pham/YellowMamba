using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Entities;
using YellowMamba.Players;

namespace YellowMamba.Managers
{
    public class EntityManager
    {
        public List<Entity> Entities { get; private set; }

        public EntityManager()
        {
            Entities = new List<Entity>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Entity entity in Entities)
            {
                entity.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Entity entity in Entities)
            {
                entity.Draw(gameTime, spriteBatch);
            }
        }
    }
}
