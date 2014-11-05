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



namespace YellowMamba.AnimatedSprites
{
    public class AnimatedSprite
    {
        public int endingFrame;
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int currentFrame;
        public int currentFrequency;
        public int spriteFrequency;
        public int startingFrame;

        public AnimatedSprite(Texture2D texture, int inputFrame, int rows, int columns, int frequency, int numFrames)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = inputFrame;
            startingFrame = inputFrame;
            endingFrame = startingFrame + numFrames - 1;
            spriteFrequency = frequency;
            currentFrequency = 0;
            FrameWidth = texture.Width / Columns;
            FrameHeight = texture.Height / Rows;
        }

        public void SelectAnimation(int startFrame, int numFrames)
        {
            startingFrame = startFrame;
            endingFrame = startingFrame + numFrames - 1;
            if (currentFrame < startingFrame || currentFrame > endingFrame)
            {
                currentFrame = startingFrame;
                currentFrequency = 0;
            }
        }

        public void Update()
        {
            currentFrequency++;
            if (currentFrequency == spriteFrequency)
            {
                currentFrame++;
                currentFrequency = 0;
            }
            if (currentFrame == endingFrame + 1)
            {
                currentFrame = startingFrame;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, bool facingLeft)
        {
            //need to arrange each animation of a sprite in one row
            //still need to change code in accordance to this
            int row = (int)Math.Ceiling((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;
            if (column == 0)
            {
                column = Columns;
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (facingLeft)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Rectangle sourceRectangle = new Rectangle(FrameWidth * (column - 1), FrameHeight * (row - 1), FrameWidth, FrameHeight);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, FrameWidth, FrameHeight);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), spriteEffects, 0);

        }
    }
}
