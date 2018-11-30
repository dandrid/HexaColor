using HexaColor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Server.ModelManipulation
{
    class AiPlayerManipulation
    {
        public Color chooseColor(MapLayoutManipulation mapLayoutManipulation, AiPlayer aiPlayer, HashSet<Color> availableColors)
        {
            Dictionary<Color, int> colorsToOccurance = new Dictionary<Color, int>();
            foreach (Color color in availableColors)
            {
                colorsToOccurance.Add(color, 0);
            }
            mapLayoutManipulation.visitContiniousNeighbours(pos =>
            {
                foreach (Position neighbourPos in mapLayoutManipulation.getNeighbourCellPositions(pos))
                {
                    Color neighbourColor = mapLayoutManipulation.mapLayout.cells[neighbourPos].color;
                    if (availableColors.Contains(neighbourColor))
                    {
                        colorsToOccurance[neighbourColor]++;
                    }
                }
            }, aiPlayer.startingPosition);

            var orderedChoices = colorsToOccurance.OrderBy(pair => pair.Value).ToList();
            Color chosenColor;
            if (aiPlayer.difficulty == AiDifficulty.HARD)
            {
                chosenColor = orderedChoices.Last().Key;
            }
            else if (aiPlayer.difficulty == AiDifficulty.MEDIUM)
            {
                chosenColor = orderedChoices.ElementAt(orderedChoices.Count / 2).Key;
            }
            else
            {
                chosenColor = orderedChoices.First().Key;
            }
            return chosenColor;
        }
    }
}
