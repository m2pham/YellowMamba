using Microsoft.Xna.Framework;
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
        private List<Player> players;
        public int NumPlayers { get { return players.Count; } }

        public PlayerManager()
        {
            players = new List<Player>();
            players.Add(new Player(PlayerIndex.One));
        }

        public void LoadContent()
        {
            foreach (Player player in players)
            {
                player.LoadContent();
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Player player in players)
            {
                player.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Player player in players)
            {
                player.Draw(gameTime, spriteBatch);
            }
        }

        public Player GetPlayer(PlayerIndex playerIndex)
        {
            return players.Find(x => x.PlayerIndex == playerIndex);
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
        }
    }
}
