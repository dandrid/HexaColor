using HexaColor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Server
{
    public class Session
    {
        public readonly Player Player;
        public readonly HttpListenerWebSocketContext WsContext;

        public Session(Player player, HttpListenerWebSocketContext wsContext)
        {
            Player = player;
            WsContext = wsContext;
        }
    }
}
