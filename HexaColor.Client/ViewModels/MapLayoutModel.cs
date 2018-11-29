using HexaColor.Client.Connections;
using HexaColor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.ViewModels
{
    public delegate void MapLayoutUpdatedHandler();
    public class MapLayoutModel : AbstractViewModel
    {
        private int playerCount;
        private int aiCount;
        private List<KeyValuePair<string, AiDifficulty>> list;

        public string PlayerName { get; private set; }
        public int MapColorCount { get; private set; }
        public int MapSize { get; private set; }
        public int HumanPlayerNumber { get; private set; }
        public List<KeyValuePair<string, AiDifficulty>> AIPlayers { get; private set; }

        public Model.MapUpdate GameModel { get; set; }

        public event MapLayoutUpdatedHandler MapLayoutUpdatedEvent;


        public MapLayoutModel(string playerName, int playerNumber, List<KeyValuePair<string, AiDifficulty>> aiPlayers, int mapColorCount, int mapSize)
        {
            PlayerName = playerName;
            HumanPlayerNumber = playerNumber;
            AIPlayers = aiPlayers;
            MapColorCount = mapColorCount;
            MapSize = mapSize;

            WebSocketConnection.StartListening();
            WebSocketConnection.MapUpdatEvent += WebSocketConnection_MapUpdate;
        }

        private void WebSocketConnection_MapUpdate(object sender, MapUpdateEventArgs e)
        {
            GameModel = e.mapUpdate;
            MapLayoutUpdatedEvent();
        }

        public async void InitMapLayout()
        {
            await WebSocketConnection.Send(new Model.NewGame(HumanPlayerNumber - AIPlayers.Count, AIPlayers, MapColorCount, MapSize, MapSize));
            await WebSocketConnection.Send(new Model.JoinGame(PlayerName));
        }
    }

}
