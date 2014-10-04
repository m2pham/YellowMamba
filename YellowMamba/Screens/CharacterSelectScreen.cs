using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using YellowMamba.Managers;
using YellowMamba.Players;
using YellowMamba.Characters;

namespace YellowMamba.Screens
{
    public class CharacterSelectScreen : Screen
    {
        private Texture2D background;
        private SpriteFont spriteFont;

        private TimeSpan TransitionInTime = new TimeSpan(0, 0, 1);
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 1);

        public CharacterSelectScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {

        }

        public override void LoadContent()
        {
            background = ContentManager.Load<Texture2D>("CharacterSelectScreenBackground");
            spriteFont = ContentManager.Load<SpriteFont>("TestFont");
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
                        ScreenManager.RemoveScreen(this);
                        ScreenManager.AddScreen(NextScreen);
                    }
                    break;
                default:
                    if (InputManager.GetMenuActionState(PlayerIndex.One, MenuActions.Start) == ActionStates.Pressed)
                    {
                        NextScreen = new StageOnePartOneScreen(ContentManager.ServiceProvider, ContentManager.RootDirectory,
                            InputManager, ScreenManager, PlayerManager);
                        TransitionStartTime = gameTime.TotalGameTime;
                        ScreenState = ScreenStates.TransitionOut;
                    }

                    foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
                    {
                        if (InputManager.GetMenuActionState(playerIndex, MenuActions.Select) == ActionStates.Pressed)
                        {
                            Player player = PlayerManager.GetPlayer(playerIndex);
                            if (player == null)
                            {
                                PlayerManager.Players.Add(new Player(playerIndex));
                            }
                            else if (player.Character == null)
                            {
                                if (playerIndex == PlayerIndex.One)
                                {
                                    player.Character = new BlackMamba(PlayerIndex.One, InputManager, PlayerManager, ContentManager.ServiceProvider, ContentManager.RootDirectory);
                                }
                                if (playerIndex == PlayerIndex.Two)
                                {
                                    player.Character = new Jimothy(PlayerIndex.Two, InputManager, PlayerManager, ContentManager.ServiceProvider, ContentManager.RootDirectory);
                                }
                            }
                        }
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
                    spriteBatch.Draw(background, new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight), Color.White);
                    foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
                    {
                        Player player = PlayerManager.GetPlayer(playerIndex);
                        String text = "";
                        if (player == null)
                        {
                            text = "Press A to Join";
                        }
                        else if (player.Character == null)
                        {
                            text = "Choose Your Character:\nBlack Mamba\nJimothy";
                        }
                        else
                        {

                        }
                        spriteBatch.DrawString(spriteFont, text, new Vector2((float)playerIndex / 4 * ScreenManager.ScreenWidth + ScreenManager.ScreenWidth / 15, ScreenManager.ScreenHeight / 2), Color.White);
                    }
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
