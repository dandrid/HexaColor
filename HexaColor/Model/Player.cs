using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class Player
    {
        public readonly Position startingPosition;
        public string name { get; set; }
        public int points { get; set; }

        public Player(Position startingPosition, string name)
        {
            this.startingPosition = startingPosition;
            this.name = name;
            this.points = 0;
        }
    }
}
