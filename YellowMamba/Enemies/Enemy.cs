using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Utility;
using YellowMamba.Entities;
using YellowMamba.Managers;
using YellowMamba.Players;

namespace YellowMamba.Enemies
{
    public enum EnemyStates
    {
        Idle, SeePlayer, Attack, Attacking, SpecialAttack, Chase, Retreat, Damaged, Dead
    }
    public abstract class Enemy : Entity
    {
        public SpriteSheet SpriteSheet { get; set; }
        public Animation CurrentAnimation { get; protected set; }
        public Animation StandingAnimation { get; protected set; }
        public Animation RunningAnimation { get; protected set; }
        public Animation AttackingAnimation { get; protected set; }
        public Animation DamagedAnimation { get; protected set; }
        protected bool IsInvincible { get; set; }
        protected int Health { get; set; } // added health
        protected int Damage { get; set; }
        public EnemyStates EnemyState { get; set; }
        protected PlayerManager PlayerManager { get; set; }
        protected Player focusedPlayer { get; set; }
        public Rectangle AttackHitbox;
        public bool AttackVisible;
        protected bool FacingLeft { get; set; }
        public int Attack { get; set; }
        protected Vector2 AttackRange { get; set; }
        protected int AttackWaitTime { get; set; }
        protected int StateTimer { get; set; }
        public bool Ranged { get; set; }

        public Enemy(PlayerManager playerManager) 
             : base()
        {
            Ranged = false;
            PlayerManager = playerManager;
        }

        public void SelectAnimation(Animation animation)
        {
            CurrentAnimation = animation;
            //CurrentAnimation.Reset();
        }
    }

    

}
