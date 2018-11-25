using HexaColor.Client.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.ViewModels
{
    public delegate void MapLayoutInitializedHandler();
    public class MapLayoutModel
    {
        public string PlayerName { get; private set; }
        public int MapColorCount { get; private set; }
        public int MapSize { get; private set; }
        public int PlayerNumber { get; private set; }

        public Model.Game GameModel { get; set; }

        public WebSocketConnection Connection { get; private set; }

        public event MapLayoutInitializedHandler MapLayoutInitialized;

        public MapLayoutModel(string playerName, int playerNumber, int mapColorCount, int mapSize)
        {
            PlayerName = playerName;
            PlayerNumber = playerNumber;
            MapColorCount = mapColorCount;
            MapSize = mapSize;
            // TODO
            //GameModel = new Model.Game(playerNumber, mapColorCount, mapSize, mapSize);
            Connection = new WebSocketConnection();
            Connection.StartListening();
            InitMapLayout();
        }

        public async void InitMapLayout()
        {
            await Connection.Send(new Model.NewGame(PlayerNumber, MapColorCount, MapSize, MapSize));
            await Connection.Send(new Model.JoinGame(PlayerName));
            // fire event
            //MapLayoutInitialized();
        }
    }

}
