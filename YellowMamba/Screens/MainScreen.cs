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
    public class MainScreen : Screen
    {
        private Texture2D background, black;

        private TimeSpan TransitionInTime = TimeSpan.Zero;
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 2);

        private float alpha = 0;

        public MainScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {

        }

        public override void LoadContent()
        {
            background = contentManager.Load<Texture2D>("MainScreenBackground");
            black = contentManager.Load<Texture2D>("Black");
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
                        Console.WriteLine(alpha);
                        alpha += 127 / (float) (TransitionOutTime.TotalMilliseconds / gameTime.ElapsedGameTime.TotalMilliseconds);
                        Console.WriteLine(alpha);
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
                        nextScreen = new CharacterSelectScreen(contentManager.ServiceProvider, contentManager.RootDirectory,
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
                    spriteBatch.Draw(black, new Rectangle(0, 0, 1280, 720), new Color(255, 255, 255, (byte) alpha));
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
