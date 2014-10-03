using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using YellowMamba.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YellowMamba.Characters
{
    public class BlackMamba : Character
    {
        private PlayerIndex playerIndex;
        private YellowMamba yellowMamba;
        private Texture2D sprite;
        private Vector2 position, velocity;
        private InputManager inputManager;
        private ContentManager contentManager;
        
        public BlackMamba(YellowMamba yellowMamba)
        {
            this.yellowMamba = yellowMamba;
            inputManager = (InputManager) yellowMamba.Services.GetService(typeof(InputManager));
            contentManager = yellowMamba.Content;
            position = new Vector2();
            velocity = new Vector2();
        }

        public void LoadContent()
        {
            sprite = contentManager.Load<Texture2D>("BlackMamba");
        }

        public void Update(GameTime gameTime)
        {
            if (inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveUp) == ActionStates.Held)
            {
                position.Y -= 5;
            }
            if (inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveDown) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveDown) == ActionStates.Held)
            {
                position.Y += 5;
            }
            if (inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveLeft) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveLeft) == ActionStates.Held)
            {
                position.X -= 5;
            }
            if (inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveRight) == ActionStates.Pressed
                || inputManager.GetCharacterActionState(playerIndex, CharacterActions.MoveRight) == ActionStates.Held)
            {
                position.X += 5;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle((int) position.X, (int) position.Y, sprite.Width, sprite.Height), Color.White);
        }
    }
}
