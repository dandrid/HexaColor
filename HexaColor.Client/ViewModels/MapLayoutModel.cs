using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.ViewModels
{
    public class MapLayoutModel
    {

        public int MapSize { get; set; }
        public double CanvasSize { get; set; }
        public MapLayoutModel()
        {
            CanvasSize = 300;
        }
    }
}
