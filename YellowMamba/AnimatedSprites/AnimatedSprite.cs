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
        public int currentFrame;
        public int currentFrequency = 0;
        public int spriteFrequency;
        public int startingFrame;

        public AnimatedSprite(Texture2D texture, int inputFrame, int rows, int columns, int frequency, int numFrames)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = inputFrame;
            startingFrame = inputFrame;
            endingFrame = numFrames;
            spriteFrequency = frequency;
        }

        public void Update()
        {
            if (currentFrequency  == spriteFrequency)
              currentFrame++;
            if (currentFrame == endingFrame)
                currentFrame = startingFrame;
           currentFrequency++;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            //need to arrange each animation of a sprite in one row
            //still need to change code in accordance to this
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Rows);
            //testing value of row
            Console.WriteLine("row = " + row);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

        }
    }
}
