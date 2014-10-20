using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Characters;
using YellowMamba.Entities;
using YellowMamba.Managers;

namespace YellowMamba.Players
{
    public class Player
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public Character Character { get; set; }
        public ShootTarget Target { get; set; }
        private InputManager inputManager;
        private PlayerManager playerManager;

        public Player(PlayerIndex playerIndex, InputManager inputManager, PlayerManager playerManager)
        {
            PlayerIndex = playerIndex;
            this.inputManager = inputManager;
            this.playerManager = playerManager;
            Target = new ShootTarget(inputManager, playerIndex);
        }

        public void LoadContent(ContentManager contentManager)
        {
            Character.LoadContent(contentManager);
        }

        public void Update(GameTime gameTime)
        {
            Character.Update(gameTime);
            if (Target.Visible == true)
            {
                Target.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Character.Draw(gameTime, spriteBatch);
            if (Target.Visible == true)
            {
                Target.Draw(gameTime, spriteBatch);
            }
        }

        public void CreateCharacter(Type type)
        {
            if (type.IsAssignableFrom(typeof(BlackMamba)))
            {
                Character = new BlackMamba(this, inputManager, playerManager);
            }
            else if (type.IsAssignableFrom(typeof(Jimothy)))
            {
                Character = new Jimothy(this, inputManager, playerManager);
            }
            else
            {
                throw new InvalidOperationException("Bad type");
            }
        }
    }
}
