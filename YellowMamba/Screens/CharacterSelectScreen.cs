﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using YellowMamba.Managers;
using YellowMamba.Players;
using YellowMamba.Characters;
using Microsoft.Xna.Framework.Media;

namespace YellowMamba.Screens
{
    public class CharacterSelectScreen : Screen
    {
        private Texture2D background;
        private SpriteFont spriteFont;

        private TimeSpan TransitionInTime = new TimeSpan(0, 0, 1);
        private TimeSpan TransitionOutTime = new TimeSpan(0, 0, 1);

        private HashSet<Type> availableChars;
        private LinkedList<Type> chars;
        private Dictionary<PlayerIndex, LinkedListNode<Type>> selectedChar;
        private Dictionary<PlayerIndex, int> inputWaitTime;
        public Song menuSong;
        public CharacterSelectScreen(IServiceProvider serviceProvider, String contentRootDirectory, InputManager inputManager,
            ScreenManager screenManager, PlayerManager playerManager)
            : base(serviceProvider, contentRootDirectory, inputManager, screenManager, playerManager)
        {
            availableChars = new HashSet<Type>();
            chars = new LinkedList<Type>();
            selectedChar = new Dictionary<PlayerIndex, LinkedListNode<Type>>();
            inputWaitTime = new Dictionary<PlayerIndex, int>();
        }

        public override void Initialize()
        {
            availableChars.Add(typeof(BlackMamba));
            availableChars.Add(typeof(Jimothy));
            chars.AddFirst(typeof(BlackMamba));
            chars.AddLast(typeof(Jimothy));
            selectedChar.Add(PlayerIndex.One, chars.First);
            selectedChar.Add(PlayerIndex.Two, chars.First);
            selectedChar.Add(PlayerIndex.Three, chars.First);
            selectedChar.Add(PlayerIndex.Four, chars.First);
            inputWaitTime.Add(PlayerIndex.One, 0);
            inputWaitTime.Add(PlayerIndex.Two, 0);
            inputWaitTime.Add(PlayerIndex.Three, 0);
            inputWaitTime.Add(PlayerIndex.Four, 0);
        }

        public override void LoadContent()
        {
            background = ContentManager.Load<Texture2D>("CharacterSelectScreenBackground");
            spriteFont = ContentManager.Load<SpriteFont>("TestFont");
            menuSong = ContentManager.Load<Song>("Music/menuSong");
            MediaPlayer.Play(menuSong);
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
                        MediaPlayer.Stop();
                        ScreenManager.AddScreen(NextScreen);
                    }
                    break;
                case ScreenStates.Active:
                    if (InputManager.GetMenuActionState(PlayerIndex.One, MenuActions.Start) == ActionStates.Pressed)
                    {
                        bool charNull = false;
                        foreach (Player player in PlayerManager.Players)
                        {
                            if (player.Character == null)
                            {
                                charNull = true;
                            }
                        }
                        if (!charNull)
                        {
                            NextScreen = new StageOnePartOneScreen(ContentManager.ServiceProvider, ContentManager.RootDirectory,
                                InputManager, ScreenManager, PlayerManager);
                            TransitionStartTime = gameTime.TotalGameTime;
                            ScreenState = ScreenStates.TransitionOut;
                        }
                    }

                    foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
                    {
                        if (inputWaitTime[playerIndex] > 0)
                        {
                            inputWaitTime[playerIndex] -= (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                            continue;
                        }
                        Player player = PlayerManager.GetPlayer(playerIndex);
                        if (InputManager.GetMenuActionState(playerIndex, MenuActions.Select) == ActionStates.Pressed)
                        {
                            // "press a to join"
                            if (player == null)
                            {
                                PlayerManager.Players.Add(new Player(playerIndex, InputManager, PlayerManager));
                                inputWaitTime[playerIndex] = 30;
                            }
                            // choose your char
                            else if (player.Character == null)
                            {
                                player.Character = CreateCharacter(player, selectedChar[playerIndex].Value);
                                PlayerManager.Characters.Add(player.Character);
                            }
                        }
                    }

                    foreach (Player player in PlayerManager.Players.ToList())
                    {
                        if (inputWaitTime[player.PlayerIndex] > 0)
                        {
                            inputWaitTime[player.PlayerIndex] -= (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds * 60F);
                            continue;
                        }
                        if (InputManager.GetMenuActionState(player.PlayerIndex, MenuActions.MoveUp) == ActionStates.Pressed)
                        {
                            selectedChar[player.PlayerIndex] = selectedChar[player.PlayerIndex].Previous ?? chars.Last;
                            inputWaitTime[player.PlayerIndex] = 20;
                        }
                        else if (InputManager.GetMenuActionState(player.PlayerIndex, MenuActions.MoveDown) == ActionStates.Pressed)
                        {
                            selectedChar[player.PlayerIndex] = selectedChar[player.PlayerIndex].Next ?? chars.First;
                            inputWaitTime[player.PlayerIndex] = 20;
                        }
                        else if (InputManager.GetMenuActionState(player.PlayerIndex, MenuActions.Back) == ActionStates.Pressed)
                        {
                            if (player != null && player.Character == null)
                            {
                                PlayerManager.RemovePlayer(player.PlayerIndex);
                                inputWaitTime[player.PlayerIndex] = 20;
                            }
                            else if (player != null && player.Character != null)
                            {
                                PlayerManager.Characters.Remove(player.Character);
                                player.Character = null;
                                inputWaitTime[player.PlayerIndex] = 20;
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
                            text = "Choose Your Character:";
                            foreach (Type type in chars)
                            {
                                string charText = type.ToString().Split('.')[2];
                                if (type == selectedChar[playerIndex].Value)
                                {
                                    text += "\n>" + charText;
                                }
                                else
                                {
                                    text += "\n" + charText;
                                }
                            }
                        }
                        else
                        {
                            text = "Ready!";
                            spriteBatch.DrawString(spriteFont, "Player One Press Start", new Vector2(ScreenManager.ScreenWidth / 2 - 100, ScreenManager.ScreenHeight - 200), Color.White);
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

        public Character CreateCharacter(Player player, Type type)
        {
            if (type.IsAssignableFrom(typeof(BlackMamba)))
            {
                return new BlackMamba(player, InputManager, PlayerManager);
            }
            else if (type.IsAssignableFrom(typeof(Jimothy)))
            {
                return new Jimothy(player, InputManager, PlayerManager);
            }
            else
            {
                throw new InvalidOperationException("Bad type");
            }
        }
    }
}
