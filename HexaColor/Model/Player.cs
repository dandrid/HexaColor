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
        EASY,
        MEDIUM,
        HARD
    }

    public class AiPlayer : Player
    {
        public readonly AiDifficulty difficulty;

        public AiPlayer(AiDifficulty difficulty,Position startingPosition, string name) : base(startingPosition, name)
        {
            this.difficulty = difficulty;
        }

        public Color chooseColor(MapLayout mapLayout, HashSet<Color> availableColors)
        {
            Dictionary<Color, int> colorsToOccurance = new Dictionary<Color, int>();
            foreach(Color color in availableColors)
            {
                colorsToOccurance.Add(color, 0);
            }
            mapLayout.visitContiniousNeighbours( pos =>
            {
                foreach(Position neighbourPos in mapLayout.getNeighbourCellPositions(pos))
                {
                    Color neighbourColor = mapLayout.cells[neighbourPos].color;
                    if (availableColors.Contains(neighbourColor))
                    {
                        colorsToOccurance[neighbourColor]++;
                    }
                }
            }, startingPosition);

            var orderedChoices = colorsToOccurance.OrderBy(pair => pair.Value).ToList();
            Color chosenColor;
            if (difficulty == AiDifficulty.HARD)
            {
                chosenColor = orderedChoices.Last().Key;
            } else if (difficulty == AiDifficulty.MEDIUM)
            {
                chosenColor = orderedChoices.ElementAt(orderedChoices.Count/2).Key;
            } else
            {
                chosenColor = orderedChoices.First().Key;
            }
            return chosenColor;
        }
    }
}
