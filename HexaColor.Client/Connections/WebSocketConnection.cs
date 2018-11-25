using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexaColor.Model;

namespace HexaColor.Client.Connections
{
    public class WebSocketConnection : IConnection
    {
        private static string ConnectionUri = "";
        private bool isOpen;

        public WebSocketConnection()
        {
            isOpen = false;
        }

        public void Open()
        {
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }

        public GameUpdate Receive()
        {
            return null;
        }

        public void Send(GameChange gameChangeEvent)
        {
            if(isOpen)
            {
                // TODO
            }
            return;
        }

        public bool IsOpen()
        {
            return isOpen;
        }
    }
}
