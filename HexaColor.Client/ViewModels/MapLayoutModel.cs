using HexaColor.Client.Connections;
using HexaColor.Model;
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
        public int HumanPlayerNumber { get; private set; }
        public int AIPlayerNumber { get; private set; }
        public AiDifficulty AIDifficulty { get; private set; }

        public Model.MapUpdate GameModel { get; set; }

        public event MapLayoutInitializedHandler MapLayoutInitialized;

        public MapLayoutModel(string playerName, int playerNumber, int aiPlayerNumber, string aiDifficulyt, int mapColorCount, int mapSize)
        {
            PlayerName = playerName;
            HumanPlayerNumber = playerNumber;
            AIPlayerNumber = aiPlayerNumber;
            AIDifficulty = (AiDifficulty)Enum.Parse(typeof(AiDifficulty), aiDifficulyt);
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
            await WebSocketConnection.Send(new Model.NewGame(HumanPlayerNumber, AIPlayerNumber, AIDifficulty, MapColorCount, MapSize, MapSize));
            await WebSocketConnection.Send(new Model.JoinGame(PlayerName));
        }
    }

}
