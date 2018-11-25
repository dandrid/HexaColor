using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class Cell
    {
        public Color color { get; set; }

        public Cell(Color color)
        {
            this.color = color;
        }
    }
}
