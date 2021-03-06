﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using YellowMamba.Enemies;

namespace YellowMamba.Managers
{
    public class EnemyManager
    {
        public List<Enemy> Enemies { get; private set; }
        public EntityManager EntityManager { get; set; }
        public PlayerManager PlayerManager { get; set; }
        public Random RandomGen { get; private set; }

        public EnemyManager(PlayerManager playerManager, EntityManager entityManager)
        {
            Enemies = new List<Enemy>();
            PlayerManager = playerManager;
            EntityManager = entityManager;
            RandomGen = new Random();
        }
        
        public void LoadContent(ContentManager contentManager)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.LoadContent(contentManager);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Enemy enemy in Enemies.ToList())
            {
                if (enemy.MarkForDelete)
                {
                    Enemies.Remove(enemy);
                }
                else
                {
                    enemy.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }
        }
    }


}
