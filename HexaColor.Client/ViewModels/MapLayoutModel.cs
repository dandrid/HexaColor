using HexaColor.Client.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.ViewModels
{
    public delegate void MapLayoutInitializedHandler();
    public class MapLayoutModel : AbstractViewModel
    {
        public string PlayerName { get; private set; }
        public int MapColorCount { get; private set; }
        public int MapSize { get; private set; }
        public int PlayerNumber { get; private set; }

        public Model.MapUpdate GameModel { get; set; }

        public event MapLayoutInitializedHandler MapLayoutInitialized;

        public MapLayoutModel(string playerName, int playerNumber, int mapColorCount, int mapSize)
        {
            PlayerName = playerName;
            PlayerNumber = playerNumber;
            MapColorCount = mapColorCount;
            MapSize = mapSize;

            WebSocketConnection.StartListening();
            WebSocketConnection.MapUpdate += WebSocketConnection_MapUpdate;
        }

        private void WebSocketConnection_MapUpdate(object sender, MapUpdateEventArgs e)
        {
            GameModel = e.mapUpdate;
            MapLayoutInitialized();
        }

        public async void InitMapLayout()
        {
            await WebSocketConnection.Send(new Model.NewGame(PlayerNumber, 0, Model.AiDifficulty.EASY, MapColorCount, MapSize, MapSize));
            await WebSocketConnection.Send(new Model.JoinGame(PlayerName));
        }
    }

}
