using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Characters;
using YellowMamba.Players;

namespace YellowMamba.Managers
{
    public class PlayerManager
    {
        private SpriteFont spriteFont;
        public List<Player> Players { get; private set; }
        public List<Character> Characters { get; private set; }
        public EntityManager EntityManager { get; set; }
        public EnemyManager EnemyManager { get; set; }
        public int Score { get; set; }

        public PlayerManager(InputManager inputManager)
        {
            Players = new List<Player>();
            Characters = new List<Character>();
            Players.Add(new Player(PlayerIndex.One, inputManager, this));
            Score = 0;
        }

        public void LoadContent(ContentManager contentManager)
        {
            spriteFont = contentManager.Load<SpriteFont>("TestFont");
            foreach (Player player in Players)
            {
                player.LoadContent(contentManager);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Player player in Players.ToList())
            {
                if (player.Character.MarkForDelete)
                {
                    Characters.Remove(player.Character);
                    Players.Remove(player);
                }
                else
                {
                    player.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Player player in Players)
            {
                player.Draw(gameTime, spriteBatch);
            }
        }

        public void DrawScore(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, "Score: " + Score, new Vector2(10, 10), Color.White);
        }

        public Player GetPlayer(PlayerIndex playerIndex)
        {
            return Players.Find(x => x.PlayerIndex == playerIndex);
        }

        public void RemovePlayer(PlayerIndex playerIndex)
        {
            Players.Remove(GetPlayer(playerIndex));
        }
    }
}
