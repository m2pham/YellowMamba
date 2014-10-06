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

        public override void Initialize()
        {

        }

        public override void LoadContent()
        {
            background = ContentManager.Load<Texture2D>("MainScreenBackground");
            black = ContentManager.Load<Texture2D>("Black");
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
                        alpha += 127 / (float) (TransitionOutTime.TotalMilliseconds / gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    else
                    {
                        ScreenManager.RemoveScreen(this);
                        ScreenManager.AddScreen(NextScreen);
                    }
                    break;
                default:
                    if (InputManager.GetMenuActionState(PlayerIndex.One, MenuActions.Start) == ActionStates.Pressed)
                    {
                        NextScreen = new CharacterSelectScreen(ContentManager.ServiceProvider, ContentManager.RootDirectory,
                            InputManager, ScreenManager, PlayerManager);
                        TransitionStartTime = gameTime.TotalGameTime;
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
                    spriteBatch.Draw(black, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), new Color(255, 255, 255, (byte)alpha));
                    break;
                case ScreenStates.Active:
                    spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), Color.White);
                    break;
            }
            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            ContentManager.Unload();
        }
    }
}
