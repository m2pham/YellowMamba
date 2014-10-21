﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Players;

namespace YellowMamba.Entities
{
    public class PassBall : Entity
    {
        public static Texture2D Sprite { get; set; }
        public bool InFlight { get; set; }
        public TimeSpan ReleaseTime { get; set; }
        public Player SourcePlayer { get; set; }
        public Player TargetPlayer { get; set; }

        public PassBall() : base()
        {
            InFlight = true;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("PassBall");
        }

        public override void Update(GameTime gameTime)
        {
            float diff = 30 - (float) gameTime.TotalGameTime.Subtract(ReleaseTime).TotalSeconds * 60;
            if (diff <= 0)
            {
                diff = .01f;
            }
            Velocity.X = (TargetPlayer.Character.Position.X - Position.X) / diff;
            Velocity.Y = (TargetPlayer.Character.Position.Y - Position.Y) / diff;
            Position += Velocity;
            Hitbox.Width = Sprite.Width;
            Hitbox.Height = Sprite.Height;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height), Color.White);
        }
    }
}
