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

        protected ContentManager contentManager;
        protected InputManager inputManager;
        protected ScreenManager screenManager;
        protected PlayerManager playerManager;

        protected Screen nextScreen;
        protected TimeSpan transitionStartTime;

        public Screen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
        {
            ScreenState = ScreenStates.Hidden;
            contentManager = new ContentManager(serviceProvider, contentRootDirectory);
            this.inputManager = inputManager;
            this.screenManager = screenManager;
            this.playerManager = playerManager;
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void UnloadContent();

        protected bool IsTransitionDone(TimeSpan currentTime, TimeSpan transitionDuration)
        {
            return currentTime.Subtract(transitionStartTime) >= transitionDuration;
        }
    }
}
