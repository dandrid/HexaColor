using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexaColor.Model;

namespace HexaColor.Client.ViewModels
{
    public delegate void NextPlayerHandler();
    class InGameControlsModel : AbstractViewModel
    {
        private string playerName;
        private List<Model.Color> availableColors;

        public List<Color> AvailableColors { get => availableColors; }

        public event NextPlayerHandler NextPlayerEvent;

        public InGameControlsModel(string playerName)
        {
            this.playerName = playerName;
            availableColors = new List<Model.Color>();
            WebSocketConnection.NextPlayerEvent += WebSocketConnection_NextPlayerEvent;
        }

        private void WebSocketConnection_NextPlayerEvent(object sender, Connections.NextPlayerEventArgs e)
        {
            if(e.nextPlayer.player.name.Equals(playerName))
            {
                //this is my turn
                availableColors = e.nextPlayer.avilableColors;
                NextPlayerEvent();
            }
        }
    }
}
