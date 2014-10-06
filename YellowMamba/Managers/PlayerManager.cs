using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Players;

namespace YellowMamba.Managers
{
    public class PlayerManager
    {
        public List<Player> Players { get; private set; }
        public EntityManager EntityManager { get; set; }

        public PlayerManager()
        {
            Players = new List<Player>();
            Players.Add(new Player(PlayerIndex.One));
        }

        public void LoadContent(ContentManager contentManager)
        {
            foreach (Player player in Players)
            {
                player.LoadContent(contentManager);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Player player in Players)
            {
                player.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Player player in Players)
            {
                player.Draw(gameTime, spriteBatch);
            }
        }

        public Player GetPlayer(PlayerIndex playerIndex)
        {
            return Players.Find(x => x.PlayerIndex == playerIndex);
        }
    }
}
