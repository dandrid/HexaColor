using HexaColor.Client.Connections;
using HexaColor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.ViewModels
{
    public abstract class AbstractViewModel
    {
        private static WebSocketConnection webSocketConnection;
        public static WebSocketConnection WebSocketConnection {
            get {
                if (webSocketConnection == null)
                    webSocketConnection = new WebSocketConnection();
                return webSocketConnection;
            }
        }
    }
}
