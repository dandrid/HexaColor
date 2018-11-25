using HexaColor.Client.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.ViewModels
{
    public class MapLayoutModel
    {

        public string MapName { get; private set; }
        public int MapPlayerCount { get; set; }
        public int MapColorCount { get; private set; }
        public int MapSize { get; private set; }

        public Model.Game GameModel { get; set; }

        public WebSocketConnection Connection { get; private set; }

        public MapLayoutModel(string mapName, int mapPlayerCount, int mapColorCount, int mapSize)
        {
            MapName = mapName;
            MapPlayerCount = mapPlayerCount;
            MapColorCount = mapColorCount;
            MapSize = mapSize;
            // TODO
            GameModel = new Model.Game(mapPlayerCount, mapColorCount, mapSize, mapSize);
            //Connection = new WebSocketConnection();
            //InitMapLayout();
        }


        public async Task InitMapLayout()
        {
            //await Connection.Send(new Model.NewGame(new Random(1000).Next(), MapColorCount, MapSize, MapSize));
        }
    }
}
