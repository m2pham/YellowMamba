using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowMamba.Entities
{
    public class ShootBall : Entity
    {
        public static Texture2D Sprite { get; set; }
        public bool InFlight { get; set; }
        public TimeSpan ReleaseTime { get; set;}
        public PlayerIndex SourcePlayer { get; set; }
        public Vector2 SourcePosition { get; set; }
        public Vector2 DestinationPosition { get; set; }

        public ShootBall()
            : base()
        {

        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("ShootBall");
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Subtract(ReleaseTime).TotalSeconds * 60 >= 60)
            {
                MarkForDelete = true;
            }
            Position.X = SourcePosition.X + Velocity.X * (float)gameTime.TotalGameTime.Subtract(ReleaseTime).TotalSeconds * 60;
            Position.Y = SourcePosition.Y + Velocity.Y * (float) ReleaseTime.Subtract(gameTime.TotalGameTime).TotalSeconds * 60 + .5F * (float) Math.Pow(ReleaseTime.Subtract(gameTime.TotalGameTime).TotalSeconds * 60, 2) / 2F;
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
