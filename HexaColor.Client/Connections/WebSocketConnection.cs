using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private ClientWebSocket webSocket;

        public WebSocketConnection()
        {
            webSocket = new ClientWebSocket();
            
        }

        public async Task Connect()
        {
            if (webSocket.State != WebSocketState.Open && webSocket.State != WebSocketState.Connecting)
            {
                await webSocket.ConnectAsync(ConnectionUri, CancellationToken.None);
            }
        }

        public async Task Send(GameChange message)
        {
            byte[] buffer;
            string json = new JavaScriptSerializer().Serialize(message);
            buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async void StartListening()
        {
            await Connect();
            var buffer = new byte[10000];

            // handle events from server
            while (true)
            {
                var packet = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), CancellationToken.None);

                if (packet.MessageType == WebSocketMessageType.Close)
                {
                    throw new ServerDisconnectedException("Web socket is closed!");
                }

                MapUpdate mapUpdate = tryParseEvent<MapUpdate>(buffer, packet);
                if (mapUpdate != null)
                {
                    // TODO map update
                    continue;
                }

                NextPlayer nextPlayer = tryParseEvent<NextPlayer>(buffer, packet);
                if (nextPlayer != null)
                {
                    // TODO handle next player
                    continue;
                }

                GameWon gameWon = tryParseEvent<GameWon>(buffer, packet);
                if (gameWon != null)
                {
                    // TODO handle game won
                    continue;
                }

                GameError gameError = tryParseEvent<GameError>(buffer, packet);
                if (gameError != null)
                {
                    // TODO handle game error
                    continue;
                }
            }
        }
        private EventType tryParseEvent<EventType>(byte[] buffer, WebSocketReceiveResult packet) where EventType : GameUpdate
        {
            try
            {
                EventType deserializedEvent = new JavaScriptSerializer().Deserialize<EventType>(Encoding.UTF8.GetString(buffer, 0, packet.Count));
                return deserializedEvent;
            }
            catch (SystemException e)
            {

            }
            return default(EventType);
        }

        public class ServerDisconnectedException : Exception
        {
            public ServerDisconnectedException(string message) : base(message) { }
        }
    }
}
