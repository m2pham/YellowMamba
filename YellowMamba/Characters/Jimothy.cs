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
    public class Jimothy : Character
    {
        public Jimothy(PlayerIndex playerIndex, InputManager inputManager, IServiceProvider serviceProvider, String contentRootDirectory)
            : base(playerIndex, inputManager, serviceProvider, contentRootDirectory)
        {

        }

        public override void LoadContent()
        {
            Sprite = ContentManager.Load<Texture2D>("Jimothy");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveUp) == ActionStates.Held)
            {
                Position.Y -= 5;
            }
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveDown) == ActionStates.Held)
            {
                Position.Y += 5;
            }
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveLeft) == ActionStates.Held)
            {
                Position.X -= 5;
            }
            if (InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Pressed
                || InputManager.GetCharacterActionState(PlayerIndex, CharacterActions.MoveRight) == ActionStates.Held)
            {
                Position.X += 5;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
        }
    }
}
