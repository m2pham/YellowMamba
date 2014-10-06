using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Managers;

namespace YellowMamba.Screens
{
    public enum ScreenStates
    {
        Active, Hidden, TransitionIn, TransitionOut
    }

    public abstract class Screen
    {
        public ScreenStates ScreenState { get; set; }

        protected ContentManager ContentManager { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected ScreenManager ScreenManager { get; private set; }
        protected PlayerManager PlayerManager { get; private set; }

        protected Screen NextScreen { get; set; }
        protected TimeSpan TransitionStartTime { get; set; }

        public Screen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
        {
            ScreenState = ScreenStates.Hidden;
            ContentManager = new ContentManager(serviceProvider, contentRootDirectory);
            InputManager = inputManager;
            ScreenManager = screenManager;
            PlayerManager = playerManager;
        }

        public abstract void Initialize();

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void UnloadContent();

        protected bool IsTransitionDone(TimeSpan currentTime, TimeSpan transitionDuration)
        {
            return currentTime.Subtract(TransitionStartTime) >= transitionDuration;
        }
    }
}
