using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowMamba.Managers
{
    public class InputManager : GameComponent
    {
        private Dictionary<MenuActions, Buttons> playerOneMenuButtonsMap;
        private Dictionary<MenuActions, ActionStates> playerOneMenuActionStatesMap;
        private Dictionary<PlayerIndex, Dictionary<CharacterActions, Buttons>> playersCharacterButtonsMap;
        private Dictionary<PlayerIndex, Dictionary<CharacterActions, ActionStates>> playersCharacterActionStatesMap;

        public InputManager(YellowMamba yellowMamba) : base(yellowMamba)
        {
            playerOneMenuButtonsMap = new Dictionary<MenuActions, Buttons>();
            playerOneMenuActionStatesMap = new Dictionary<MenuActions, ActionStates>();
            playersCharacterButtonsMap = new Dictionary<PlayerIndex, Dictionary<CharacterActions, Buttons>>();
            playersCharacterActionStatesMap = new Dictionary<PlayerIndex, Dictionary<CharacterActions, ActionStates>>();
        }

        public override void Initialize()
        {
            playerOneMenuButtonsMap.Add(MenuActions.Select, Buttons.A);
            playerOneMenuButtonsMap.Add(MenuActions.Back, Buttons.B);

            foreach (MenuActions menuAction in Enum.GetValues(typeof(MenuActions)))
            {
                playerOneMenuActionStatesMap.Add(menuAction, ActionStates.Released);
            }

            Dictionary<CharacterActions, Buttons> defaultCharacterButtonsMap = new Dictionary<CharacterActions, Buttons>();
            defaultCharacterButtonsMap.Add(CharacterActions.Jump, Buttons.A);
            defaultCharacterButtonsMap.Add(CharacterActions.Attack, Buttons.X);
            defaultCharacterButtonsMap.Add(CharacterActions.Pass, Buttons.LeftTrigger);
            defaultCharacterButtonsMap.Add(CharacterActions.ShootMode, Buttons.RightTrigger);

            Dictionary<CharacterActions, ActionStates> defaultCharacterActionStatesMap = new Dictionary<CharacterActions, ActionStates>();
            foreach (CharacterActions characterAction in Enum.GetValues(typeof(CharacterActions)))
            {
                defaultCharacterActionStatesMap.Add(characterAction, ActionStates.Released);
            }

            foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
            {
                playersCharacterButtonsMap.Add(playerIndex, new Dictionary<CharacterActions, Buttons>(defaultCharacterButtonsMap));
                playersCharacterActionStatesMap.Add(playerIndex, new Dictionary<CharacterActions, ActionStates>(defaultCharacterActionStatesMap));
            }
            base.Initialize();
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

        public override void Update(GameTime gameTime)
        {
            foreach (PlayerIndex playerIndex in Enum.GetValues(typeof(PlayerIndex)))
            {
                Dictionary<CharacterActions, Buttons> playerCharacterButtonsMap = playersCharacterButtonsMap[playerIndex];
                foreach (CharacterActions characterAction in Enum.GetValues(typeof(CharacterActions)))
                {
                    Buttons button = playerCharacterButtonsMap[characterAction];
                    if (GamePad.GetState(playerIndex).IsConnected && GamePad.GetState(playerIndex).IsButtonDown(button))
                    {
                        UpdatePlayerCharacterActionsMapOnPressed(playerIndex, characterAction);
                    }
                    else
                    {
                        UpdatePlayerCharacterActionsMapOnReleased(playerIndex, characterAction);
                    }
                }
            }

            foreach (MenuActions menuAction in Enum.GetValues(typeof(MenuActions)))
            {
                Buttons button = playerOneMenuButtonsMap[menuAction];
                if (GamePad.GetState(PlayerIndex.One).IsConnected && GamePad.GetState(PlayerIndex.One).IsButtonDown(button))
                {
                    playerOneMenuActionStatesMap[menuAction] = ActionStates.Pressed;
                }
                else
                {
                    playerOneMenuActionStatesMap[menuAction] = ActionStates.Released;
                }
            }
            base.Update(gameTime);
        }

        public ActionStates GetCharacterActionState(PlayerIndex playerIndex, CharacterActions characterAction)
        {
            return playersCharacterActionStatesMap[playerIndex][characterAction];
        }

        public ActionStates GetMenuActionState(MenuActions menuAction)
        {
            return playerOneMenuActionStatesMap[menuAction];
        }

        private void UpdatePlayerCharacterActionsMapOnPressed(PlayerIndex playerIndex, CharacterActions characterAction)
        {
            Dictionary<CharacterActions, ActionStates> playerCharacterActionMap = playersCharacterActionStatesMap[playerIndex];
            if (playerCharacterActionMap[characterAction] == ActionStates.Pressed)
            {
                playerCharacterActionMap[characterAction] = ActionStates.Held;
            }
            else if (playerCharacterActionMap[characterAction] == ActionStates.Released ||
                playerCharacterActionMap[characterAction] == ActionStates.JustReleased)
            {
                playerCharacterActionMap[characterAction] = ActionStates.Pressed;
            }
        }

        private void UpdatePlayerCharacterActionsMapOnReleased(PlayerIndex playerIndex, CharacterActions characterAction)
        {
            Dictionary<CharacterActions, ActionStates> playerCharacterActionMap = playersCharacterActionStatesMap[playerIndex];
            if (playerCharacterActionMap[characterAction] == ActionStates.Pressed ||
                playerCharacterActionMap[characterAction] == ActionStates.Held)
            {
                playerCharacterActionMap[characterAction] = ActionStates.JustReleased;
            }
            else if (playerCharacterActionMap[characterAction] == ActionStates.Released ||
                playerCharacterActionMap[characterAction] == ActionStates.JustReleased)
            {
                playerCharacterActionMap[characterAction] = ActionStates.Released;
            }
        }
    }

    public enum MenuActions
    {
        Select, Back
    }

    public enum ActionStates
    {
        Pressed, Held, JustReleased, Released
    }

    public enum CharacterActions
    {
        Jump, Attack, Pass, ShootMode
    }
}
