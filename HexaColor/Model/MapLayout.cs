using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class MapLayout
    {
        public int usedColors { get; set; }
        public int mapSize { get; set; }
        public Dictionary<Position, Cell> cells { get; set; }

        public MapLayout()
        {

        }
    }
}
