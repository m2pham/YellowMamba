using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;



namespace YellowMamba.Utility
{
    public class Animation
    {
        private SpriteSheet spriteSheet;
        public int Frequency { get; private set; }
        public int NumFrames { get; private set; }
        private int startingFrame, currentFrequency, currentFrame;
        public Animation(SpriteSheet spriteSheet, int startingFrame, int numFrames, int frequency)
        {
            this.spriteSheet = spriteSheet;
            this.Frequency = frequency;
            this.startingFrame = startingFrame;
            this.NumFrames = numFrames;

            currentFrame = startingFrame;
            currentFrequency = 0;
        }

        public Animation(SpriteSheet spriteSheet, int frequency)
        {
            this.spriteSheet = spriteSheet;
            this.Frequency = frequency;
            this.startingFrame = 1;
            this.NumFrames = spriteSheet.Rows * spriteSheet.Columns;

            currentFrame = startingFrame;
            currentFrequency = 0;
        }

        public void Update(GameTime gameTime)
        {
            currentFrequency++;
            if (currentFrequency == Frequency)
            {
                currentFrame++;
                currentFrequency = 0;
            }
            if (currentFrame == startingFrame + NumFrames)
            {
                currentFrame = startingFrame;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, bool facingLeft)
        {
            //need to arrange each animation of a sprite in one row
            //still need to change code in accordance to this
            int row = (int)Math.Ceiling((float)currentFrame / (float)spriteSheet.Columns);
            int column = currentFrame % spriteSheet.Columns;
            if (column == 0)
            {
                column = spriteSheet.Columns;
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (facingLeft)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Rectangle sourceRectangle = new Rectangle(spriteSheet.FrameWidth * (column - 1), spriteSheet.FrameHeight * (row - 1), spriteSheet.FrameWidth, spriteSheet.FrameHeight);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, spriteSheet.FrameWidth, spriteSheet.FrameHeight);

            spriteBatch.Draw(spriteSheet.Texture, destinationRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), spriteEffects, 0);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, float positionZ, bool facingLeft)
        {
            //need to arrange each animation of a sprite in one row
            //still need to change code in accordance to this
            int row = (int)Math.Ceiling((float)currentFrame / (float)spriteSheet.Columns);
            int column = currentFrame % spriteSheet.Columns;
            if (column == 0)
            {
                column = spriteSheet.Columns;
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (facingLeft)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Rectangle sourceRectangle = new Rectangle(spriteSheet.FrameWidth * (column - 1), spriteSheet.FrameHeight * (row - 1), spriteSheet.FrameWidth, spriteSheet.FrameHeight);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)(location.Y - positionZ), spriteSheet.FrameWidth, spriteSheet.FrameHeight);

            spriteBatch.Draw(spriteSheet.Texture, destinationRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), spriteEffects, 0);

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, bool facingLeft, float scale)
        {
            //need to arrange each animation of a sprite in one row
            //still need to change code in accordance to this
            int row = (int)Math.Ceiling((float)currentFrame / (float)spriteSheet.Columns);
            int column = currentFrame % spriteSheet.Columns;
            if (column == 0)
            {
                column = spriteSheet.Columns;
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (facingLeft)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Rectangle sourceRectangle = new Rectangle(spriteSheet.FrameWidth * (column - 1), spriteSheet.FrameHeight * (row - 1), spriteSheet.FrameWidth, spriteSheet.FrameHeight);

            spriteBatch.Draw(spriteSheet.Texture, location, sourceRectangle, Color.White, 0, new Vector2(0, 0), scale, spriteEffects, 0);
        }

        public void Reset()
        {
            currentFrame = startingFrame;
            currentFrequency = 0;
        }
    }
}
