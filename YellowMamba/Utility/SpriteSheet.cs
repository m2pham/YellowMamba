using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowMamba.Utility
{
    public class SpriteSheet
    {
        public Texture2D Texture { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int FrameHeight { get; private set; }
        public int FrameWidth { get; private set; }

        public SpriteSheet(Texture2D sheet, int rows, int cols)
        {
            Texture = sheet;
            this.Rows = rows;
            this.Columns = cols;
            FrameHeight = sheet.Height / rows;
            FrameWidth = sheet.Width / cols;
        }
    }
}
