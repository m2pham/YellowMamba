using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.AnimatedSprites;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;

namespace YellowMamba.Enemies
{
    public enum EnemyStates
    {
        Idle, SeePlayer, Attack, SpecialAttack, Chase, Retreat, Dead
    }
    public abstract class Enemy : Entity
    {
        protected int EnemyIndex; // might not need this 
        public Texture2D Sprite { get; set; }
        protected bool IsInvincible { get; set; }
        protected int Health { get; set; } // added health
        protected int Damage { get; set; }
        public EnemyStates EnemyState { get; protected set; }
        protected PlayerManager PlayerManager { get; set; }
        protected Player focusedPlayer { get; set; }
        protected AnimatedSprite animatedSprite;

        public Enemy(PlayerManager playerManager) 
             : base()
        {
            PlayerManager = playerManager;
        }
    }

    

}
