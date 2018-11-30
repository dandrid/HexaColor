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

    public enum AiDifficulty
    {
        EASY = 1,
        MEDIUM = 2,
        HARD = 3
    }

    public class AiPlayer : Player
    {
        public readonly AiDifficulty difficulty;

        public AiPlayer(AiDifficulty difficulty, Position startingPosition, string name) : base(startingPosition, name)
        {
            this.difficulty = difficulty;
        }
    }
}
