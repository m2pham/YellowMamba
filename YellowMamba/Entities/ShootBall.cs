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
        public static Texture2D HitboxSprite { get; private set; }
        public bool InFlight { get; set; }
        public TimeSpan ReleaseTime { get; set;}
        public PlayerIndex SourcePlayer { get; set; }
        public Vector2 SourcePosition { get; set; }
        public Vector2 DestinationPosition { get; set; }
        private float rotation;

        public ShootBall()
            : base()
        {
            rotation = 0.0f;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Sprite = contentManager.Load<Texture2D>("Objects/Ball");
            HitboxSprite = contentManager.Load<Texture2D>("Black");
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Subtract(ReleaseTime).TotalSeconds * 60 >= 60)
            {
                MarkForDelete = true;
            }
            Position.X = SourcePosition.X + Velocity.X * (float)gameTime.TotalGameTime.Subtract(ReleaseTime).TotalSeconds * 60;
            Position.Y = SourcePosition.Y + Velocity.Y * (float)ReleaseTime.Subtract(gameTime.TotalGameTime).TotalSeconds * 60 + .5F * (float) Math.Pow(ReleaseTime.Subtract(gameTime.TotalGameTime).TotalSeconds * 60, 2) / 2F;
            Hitbox.Width = Sprite.Width;
            Hitbox.Height = Sprite.Height;
            Hitbox.X = (int)Position.X - 25;
            Hitbox.Y = (int)Position.Y - 25;
            rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 15;
            float circle = MathHelper.Pi * 2;
            rotation = rotation % circle;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(HitboxSprite, Hitbox, Color.Blue);
            spriteBatch.Draw(Sprite, Position, null, Color.White, rotation, new Vector2(Sprite.Width / 2, Sprite.Height/2), 1.0f, SpriteEffects.None, 0);
        }
    }
}
