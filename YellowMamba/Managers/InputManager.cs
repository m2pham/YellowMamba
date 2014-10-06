using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowMamba.Managers
{
    public class InputManager
    {
        private Dictionary<MenuActions, Buttons> playerMenuButtonsMap;
        private Dictionary<PlayerIndex, Dictionary<MenuActions, ActionStates>> playersMenuActionStatesMap;
        private Dictionary<PlayerIndex, Dictionary<CharacterActions, Buttons>> playersCharacterButtonsMap;
        private Dictionary<PlayerIndex, Dictionary<CharacterActions, ActionStates>> playersCharacterActionStatesMap;
        public LinkedList<CharacterActions> PassButtons { get; private set; }

        public InputManager()
        {
            playerMenuButtonsMap = new Dictionary<MenuActions, Buttons>();
            playersMenuActionStatesMap = new Dictionary<PlayerIndex, Dictionary<MenuActions, ActionStates>>();
            playersCharacterButtonsMap = new Dictionary<PlayerIndex, Dictionary<CharacterActions, Buttons>>();
            playersCharacterActionStatesMap = new Dictionary<PlayerIndex, Dictionary<CharacterActions, ActionStates>>();
            PassButtons = new LinkedList<CharacterActions>();
        }

        public void Initialize()
        {
            PassButtons.AddLast(CharacterActions.Jump);
            PassButtons.AddLast(CharacterActions.Attack);
            PassButtons.AddLast(CharacterActions.Pick);

            playerMenuButtonsMap.Add(MenuActions.Start, Buttons.Start);
            playerMenuButtonsMap.Add(MenuActions.Select, Buttons.A);
            playerMenuButtonsMap.Add(MenuActions.Back, Buttons.B);
            playerMenuButtonsMap.Add(MenuActions.MoveUp, Buttons.LeftThumbstickUp);
            playerMenuButtonsMap.Add(MenuActions.MoveDown, Buttons.LeftThumbstickDown);
            playerMenuButtonsMap.Add(MenuActions.MoveLeft, Buttons.LeftThumbstickLeft);
            playerMenuButtonsMap.Add(MenuActions.MoveRight, Buttons.LeftThumbstickRight);

            Dictionary<CharacterActions, Buttons> defaultCharacterButtonsMap = new Dictionary<CharacterActions, Buttons>();
            defaultCharacterButtonsMap.Add(CharacterActions.Jump, Buttons.A);
            defaultCharacterButtonsMap.Add(CharacterActions.Attack, Buttons.X);
            defaultCharacterButtonsMap.Add(CharacterActions.Pick, Buttons.Y);
            defaultCharacterButtonsMap.Add(CharacterActions.PreviousPass, Buttons.LeftShoulder);
            defaultCharacterButtonsMap.Add(CharacterActions.NextPass, Buttons.RightShoulder);
            defaultCharacterButtonsMap.Add(CharacterActions.Pass, Buttons.LeftTrigger);
            defaultCharacterButtonsMap.Add(CharacterActions.ShootMode, Buttons.RightTrigger);
            defaultCharacterButtonsMap.Add(CharacterActions.MoveUp, Buttons.LeftThumbstickUp);
            defaultCharacterButtonsMap.Add(CharacterActions.MoveDown, Buttons.LeftThumbstickDown);
            defaultCharacterButtonsMap.Add(CharacterActions.MoveLeft, Buttons.LeftThumbstickLeft);
            defaultCharacterButtonsMap.Add(CharacterActions.MoveRight, Buttons.LeftThumbstickRight);

            Dictionary<CharacterActions, ActionStates> defaultCharacterActionStatesMap = new Dictionary<CharacterActions, ActionStates>();
            foreach (CharacterActions characterAction in Enum.GetValues(typeof(CharacterActions)))
            {
                defaultCharacterActionStatesMap.Add(characterAction, ActionStates.Released);
            }

            Dictionary<MenuActions, ActionStates> defaultMenuActionStatesMap = new Dictionary<MenuActions, ActionStates>();
            foreach (MenuActions menuAction in Enum.GetValues(typeof(MenuActions)))
            {
                defaultMenuActionStatesMap.Add(menuAction, ActionStates.Released);
            }

            foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
            {
                playersMenuActionStatesMap.Add(playerIndex, new Dictionary<MenuActions, ActionStates>(defaultMenuActionStatesMap));
                playersCharacterButtonsMap.Add(playerIndex, new Dictionary<CharacterActions, Buttons>(defaultCharacterButtonsMap));
                playersCharacterActionStatesMap.Add(playerIndex, new Dictionary<CharacterActions, ActionStates>(defaultCharacterActionStatesMap));
            }
        }

        public void UpdateButtonMap(PlayerIndex playerIndex, Buttons button, CharacterActions characterAction)
        {
            Dictionary<CharacterActions, Buttons> playerCharacterButtonsMap = playersCharacterButtonsMap[playerIndex];
            foreach (KeyValuePair<CharacterActions, Buttons> pair in playerCharacterButtonsMap)
            {
                if (pair.Value.Equals(button))
                {
                    playerCharacterButtonsMap[pair.Key] = playerCharacterButtonsMap[characterAction];
                }
            }
            playerCharacterButtonsMap[characterAction] = button;
        }

        public void Update(GameTime gameTime)
        {
            foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
            {
                if (!GamePad.GetState(playerIndex).IsConnected)
                {
                    continue;
                }

                Dictionary<CharacterActions, Buttons> playerCharacterButtonsMap = playersCharacterButtonsMap[playerIndex];
                foreach (CharacterActions characterAction in Enum.GetValues(typeof(CharacterActions)))
                {
                    Buttons button = playerCharacterButtonsMap[characterAction];

                    // Monogame has says up and down are both pressed if either one is pressed. Handle those cases manually
                    if (button == Buttons.LeftThumbstickUp && GamePad.GetState(playerIndex).ThumbSticks.Left.Y > .24f)
                    {
                        UpdatePlayerCharacterActionStatesMapOnPressed(playerIndex, characterAction);
                    }
                    else if (button == Buttons.LeftThumbstickDown && GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -.24f)
                    {
                        UpdatePlayerCharacterActionStatesMapOnPressed(playerIndex, characterAction);
                    }
                    else if (button != Buttons.LeftThumbstickUp && button != Buttons.LeftThumbstickDown && GamePad.GetState(playerIndex).IsButtonDown(button))
                    {
                        UpdatePlayerCharacterActionStatesMapOnPressed(playerIndex, characterAction);
                    }
                    else
                    {
                        playersCharacterActionStatesMap[playerIndex][characterAction] = ActionStates.Released;
                    }
                }

                foreach (MenuActions menuAction in Enum.GetValues(typeof(MenuActions)))
                {
                    Buttons button = playerMenuButtonsMap[menuAction];
                    if (button == Buttons.LeftThumbstickUp && GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > .24f)
                    {
                        playersMenuActionStatesMap[playerIndex][menuAction] = ActionStates.Pressed;
                    }
                    else if (button == Buttons.LeftThumbstickDown && GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -.24f)
                    {
                        playersMenuActionStatesMap[playerIndex][menuAction] = ActionStates.Pressed;
                    }
                    else if (button != Buttons.LeftThumbstickUp && button != Buttons.LeftThumbstickDown && GamePad.GetState(playerIndex).IsButtonDown(button))
                    {
                        playersMenuActionStatesMap[playerIndex][menuAction] = ActionStates.Pressed;
                    }
                    else
                    {
                        playersMenuActionStatesMap[playerIndex][menuAction] = ActionStates.Released;
                    }
                }
            }
        }

        public ActionStates GetCharacterActionState(PlayerIndex playerIndex, CharacterActions characterAction)
        {
            return playersCharacterActionStatesMap[playerIndex][characterAction];
        }

        public ActionStates GetMenuActionState(PlayerIndex playerIndex, MenuActions menuAction)
        {
            return playersMenuActionStatesMap[playerIndex][menuAction];
        }

        private void UpdatePlayerCharacterActionStatesMapOnPressed(PlayerIndex playerIndex, CharacterActions characterAction)
        {
            Dictionary<CharacterActions, ActionStates> playerCharacterActionMap = playersCharacterActionStatesMap[playerIndex];
            if (playerCharacterActionMap[characterAction] == ActionStates.Pressed)
            {
                playerCharacterActionMap[characterAction] = ActionStates.Held;
            }
            else if (playerCharacterActionMap[characterAction] == ActionStates.Released)
            {
                playerCharacterActionMap[characterAction] = ActionStates.Pressed;
            }
        }
    }

    public enum MenuActions
    {
        Start, Select, Back, MoveUp, MoveDown, MoveLeft, MoveRight
    }

    public enum ActionStates
    {
        Pressed, Held, Released
    }

    public enum CharacterActions
    {
        Jump, Attack, Pass, ShootMode, Pick, PreviousPass, NextPass, MoveUp, MoveDown, MoveLeft, MoveRight
    }
}
