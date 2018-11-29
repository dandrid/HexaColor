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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HexaColor.Client.Connections
{
    public class WebSocketConnection : IConnection
    {
        private static Uri ConnectionUri = new Uri("ws://localhost:4280/HexaColor/");
        private ClientWebSocket webSocket;

        public event EventHandler<GameErrorEventArgs> GameErrorEvent;
        public event EventHandler<MapUpdateEventArgs> MapUpdatEvent;
        public event EventHandler<NextPlayerEventArgs> NextPlayerEvent;

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
                if (tryParseEvent<NextPlayer>(buffer, packet, out nextPlayer))
                {
                    NextPlayerEvent(this, new NextPlayerEventArgs(nextPlayer));
                    continue;
                }

                GameWon gameWon;
                if (tryParseEvent<GameWon>(buffer, packet, out gameWon))
                {
                    // TODO handle game won
                    continue;
                }

                GameError gameError;
                if (tryParseEvent<GameError>(buffer, packet, out gameError))
                {
                    // TODO handle game error
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
    /*
    public class PositionConverter : JsonConverter<Position>
    {
        public override void WriteJson(JsonWriter writer, Position value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Position ReadJson(JsonReader reader, Type objectType, Position existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            return new Position(s);
        }
    }*/

    public class MapUpdateEventArgs : System.EventArgs
    {
        public readonly MapUpdate mapUpdate;

        public MapUpdateEventArgs(MapUpdate mapUpdate)
        {
            this.mapUpdate = mapUpdate;
        }
    }
    public class GameErrorEventArgs : System.EventArgs
    {
        public readonly GameError gameError;

        public GameErrorEventArgs(GameError gameError)
        {
            this.gameError = gameError;
        }
    }
    public class NextPlayerEventArgs : System.EventArgs
    {
        public readonly NextPlayer nextPlayer;

        public NextPlayerEventArgs(NextPlayer nextPlayer)
        {
            this.nextPlayer = nextPlayer;
        }
    }
}
