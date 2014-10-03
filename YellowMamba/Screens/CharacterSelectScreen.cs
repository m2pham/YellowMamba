using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using YellowMamba.Managers;

namespace YellowMamba.Screens
{
    public class CharacterSelectScreen : Screen
    {
        private Texture2D background;

        private TimeSpan TransitionInTime = new TimeSpan(0, 0, 1);
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 1);

        public CharacterSelectScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {

        }

        public override void LoadContent()
        {
            background = contentManager.Load<Texture2D>("CharacterSelectScreenBackground");
        }

        public override void Update(GameTime gameTime)
        {
            switch (ScreenState)
            {
                case ScreenStates.TransitionIn:
                    if (!IsTransitionDone(gameTime.TotalGameTime, TransitionInTime))
                    {

                    }
                    else
                    {
                        ScreenState = ScreenStates.Active;
                    }
                    break;
                case ScreenStates.TransitionOut:
                    if (!IsTransitionDone(gameTime.TotalGameTime, TransitionOutTime))
                    {
                        // transition out logic
                    }
                    else
                    {
                        screenManager.RemoveScreen(this);
                        screenManager.AddScreen(nextScreen);
                    }
                    break;
                default:
                    if (inputManager.GetMenuActionState(MenuActions.Select) == ActionStates.Pressed)
                    {
                        Console.WriteLine("CHARACTER TRANSITION NOW");
                        nextScreen = new StageOnePartOneScreen(contentManager.ServiceProvider, contentManager.RootDirectory,
                            inputManager, screenManager, playerManager);
                        transitionStartTime = gameTime.TotalGameTime;
                        ScreenState = ScreenStates.TransitionOut;
                    }
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            switch (ScreenState)
            {
                case ScreenStates.TransitionIn:
                    // transition in animation here
                    break;
                case ScreenStates.TransitionOut:
                    // transition out animation here
                    break;
                case ScreenStates.Active:
                    spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
                    break;
            }
            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            contentManager.Unload();
        }
    }
}
