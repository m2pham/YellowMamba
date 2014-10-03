using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Screens;

namespace YellowMamba.Managers
{
    public class ScreenManager
    {
        LinkedList<Screen> screens;
        InputManager inputManager;

        public ScreenManager(InputManager inputManager)
        {
            screens = new LinkedList<Screen>();
            this.inputManager = inputManager;
        }

        public void Update(GameTime gameTime)
        {
            screens.First.Value.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Screen screen in screens)
            {
                if (!(screen.ScreenState == ScreenStates.Hidden))
                {
                    screen.Draw(gameTime, spriteBatch);
                }
            }
        }

        public void AddScreen(Screen screen)
        {
            screen.ScreenState = ScreenStates.TransitionIn;
            screen.LoadContent();
            screens.AddFirst(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            screen.UnloadContent();
            screens.Remove(screen);
        }
    }
}
