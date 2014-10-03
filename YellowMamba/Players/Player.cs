using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Characters;

namespace YellowMamba.Players
{
    public class Player
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public Character Character { get; set; }

        public Player(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public void LoadContent()
        {
            Character.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            Character.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Character.Draw(gameTime, spriteBatch);
        }
    }
}
