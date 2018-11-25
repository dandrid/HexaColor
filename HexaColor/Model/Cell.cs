using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class Cell
    {
        public Cell(Color color)
        {
            this.color = color;
        }
        public Color color { get; internal set; }
        public List<Cell> neighbourCells { get; set; }

        public void changeContinousColors(Color newColor)
        {
            Color oldColor = color;
            color = newColor;

            foreach (Cell cell in neighbourCells)
            {
                if (cell.color == oldColor)
                {
                    cell.changeContinousColors(newColor);
                }
            }
        }

        public HashSet<Color> getContinousNeighbourColors()
        {
            return getContinousNeighbourColorsHelper(color, new HashSet<Cell>(), new HashSet<Color>());
        }

        private HashSet<Color> getContinousNeighbourColorsHelper(Color startingColor, HashSet<Cell> visiteCells, HashSet<Color> differentColors)
        {
            if (color != startingColor)
            {
                differentColors.Add(color);
            }
            visiteCells.Add(this);

            foreach (Cell neighbourCell in neighbourCells)
            {
                if (neighbourCell.color == startingColor && !visiteCells.Contains(neighbourCell))
                {
                    getContinousNeighbourColorsHelper(startingColor, visiteCells, differentColors);
                }
            }
            return differentColors;
        }
    }
}
