﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowMamba.Entities
{
    public class Ball : Entity
    {
        public static Texture2D Sprite { get; set; }
        public bool InFlight { get; set; }
        public PlayerIndex SourcePlayer { get; set; }

        public Ball() : base()
        {

        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("Ball");
        }

        public override void Update(GameTime gameTime)
        {
            Position += Velocity;
            Hitbox.Width = Sprite.Width;
            Hitbox.Height = Sprite.Height;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Hitbox.X, (int)Hitbox.Y, Hitbox.Width, Hitbox.Height), Color.White);
        }
    }
}
