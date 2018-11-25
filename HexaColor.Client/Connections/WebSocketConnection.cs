using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HexaColor.Model;
using HexaColor.Networking;

namespace HexaColor.Client.Connections
{
    public class WebSocketConnection : IConnection
    {
        private static Uri ConnectionUri = new Uri("ws://localhost:4280/HexaColor/");
        private static ClientWebSocket webSocket;

        public WebSocketConnection()
        {
            webSocket = new ClientWebSocket();
            webSocket.ConnectAsync(ConnectionUri, CancellationToken.None);
        }

        public async Task Close()
        {
            try
            {
                if(webSocket != null)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal_exit", CancellationToken.None);
                }
            }
            finally
            {
                webSocket.Dispose();
            }
        }

        public Task<GameUpdate> Receive()
        {
            return null;
        }

        public async Task Send(GameChange message)
        {
            byte[] buffer;
            string json = new JavaScriptSerializer().Serialize(message);
            buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

    }
}
