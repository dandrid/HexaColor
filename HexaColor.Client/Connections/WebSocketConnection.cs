using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using HexaColor.Model;
using HexaColor.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HexaColor.Client.Connections
{
    public class WebSocketConnection : AbstractConnection
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
            string json = JsonConvert.SerializeObject(message, new KeyValuePairConverter());
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

                MapUpdate mapUpdate;
                if (tryParseEvent<MapUpdate>(buffer, packet, out mapUpdate) && mapUpdate.mapLayout != null)
                {
                    MapUpdatEvent.Invoke(this, new MapUpdateEventArgs(mapUpdate));
                    continue;
                }

                NextPlayer nextPlayer;
                if (tryParseEvent<NextPlayer>(buffer, packet, out nextPlayer) && nextPlayer.player != null)
                {
                    NextPlayerEvent(this, new NextPlayerEventArgs(nextPlayer));
                    continue;
                }

                GameWon gameWon;
                if (tryParseEvent<GameWon>(buffer, packet, out gameWon) && gameWon.players != null)
                {
                    string message = "Game won. Points:";
                    foreach(Player player in gameWon.players)
                    {
                        message += "\n" + player.name + ": " + player.points;
                    }
                    MessageBox.Show(message);
                    GameWonEvent(this, new GameWonEventArgs(gameWon));
                    continue;
                }

                GameError gameError;
                if (tryParseEvent<GameError>(buffer, packet, out gameError))
                {
                    MessageBox.Show(gameError.error);
                    continue;
                }
            }
        }


        private bool tryParseEvent<EventType>(byte[] buffer, WebSocketReceiveResult packet, out EventType gameUpdate) where EventType : GameUpdate
        {
            try
            {
                string json = Encoding.UTF8.GetString(buffer, 0, packet.Count);
                
                EventType deserializedEvent = JsonConvert.DeserializeObject<EventType>(json);
                gameUpdate = deserializedEvent;
                return true;
            }
            catch (SystemException e)
            {

            }
            gameUpdate = default(EventType);
            return false;
        }

        public class ServerDisconnectedException : Exception
        {
            public ServerDisconnectedException(string message) : base(message) { }
        }
    }
}
