using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowMamba.Characters;

namespace YellowMamba.Players
{
    public class Player
    {
        private YellowMamba yellowMamba;
        public Character Character { get; set; }

        public Player(YellowMamba yellowMamba)
        {
            this.yellowMamba = yellowMamba;
        }
    }
}
