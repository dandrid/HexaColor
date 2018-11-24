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
    }
}
