using HexaColor.Model;
using HexaColor.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HexaColor.Server
{
    class Program
    {

        static List<Session> sessions = new List<Session>();
        static Game game = null;
        static object SyncRoot = new object();

        static void Main(string[] args)
        {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:4280/HexaColor/");
            httpListener.Start();
            httpListener.BeginGetContext(OnHttpRequest, httpListener);
            Console.WriteLine("Service is up");
            Console.ReadLine();
            httpListener.Stop();
        }

        private static void OnHttpRequest(IAsyncResult ar)
        {
            var httpListener = ar.AsyncState as HttpListener;
            if (httpListener == null || !httpListener.IsListening)
            {
                return;
            }
            var context = httpListener.EndGetContext(ar);
            httpListener.BeginGetContext(OnHttpRequest, httpListener);
            if (context.Request.IsWebSocketRequest)
            {
                HandleWebSocketClient(context);
                //SimpleWebSocketClientHandle(context);
            }
        }

        private static async void SimpleWebSocketClientHandle(HttpListenerContext context)
        {
            var ws = await context.AcceptWebSocketAsync(null);
            var buffer = new byte[1000];
            while (true)
            {
                var packet = await ws.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), CancellationToken.None);

                if (packet.MessageType == WebSocketMessageType.Close)
                {
                    //lock (SyncRoot)
                    //    sessions.Remove(currentSession);
                    break;
                }

                var cd = new JavaScriptSerializer().Deserialize<WsClientMessage>(Encoding.UTF8.GetString(buffer, 0, packet.Count));
                Console.WriteLine(cd.Message);

                //WsServerMessage response = new WsServerMessage
                //{
                //    Message = "Ok"
                //};
                //buffer = new byte[1000];
                //string json = new JavaScriptSerializer().Serialize(response);
                //buffer = Encoding.UTF8.GetBytes(json);
                //await ws.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                //lock (SyncRoot)
                //    game.ProcessClientData(currentSession.Spaceship, cd);
            }
        }

        private static async void HandleWebSocketClient(HttpListenerContext context)
        {
            var ws = await context.AcceptWebSocketAsync(null);
            var buffer = new byte[10000];

            // wait for new game event, if the game is null
            if (game == null)
            {
                try
                {
                    NewGame newGame = await waitForEvent<NewGame>(ws, buffer);
                    lock (SyncRoot)
                    {
                        // Create a new game
                        game = new Game(newGame.playerNumber, newGame.usedColors, newGame.rows, newGame.columns);
                    }
                }
                catch(ClientDisconnectedException e)
                {
                    updatePlayers(new GameError(e));
                    return;
                }
            }

            // wait for player to join
            Session currentSession;
            JoinGame joinGameEvent;
            try
            {
                joinGameEvent = await waitForEvent<JoinGame>(ws, buffer);
            }
            catch (ClientDisconnectedException e)
            {
                updatePlayers(new GameError(e));
                return;
            }
            lock (SyncRoot)
            {
                // Add player to game
                sessions.Add(currentSession = new Session(game.addNewPlayer(joinGameEvent.playerName), ws));
            }
            updatePlayers(game.createMapUpdate());

            // handle game changes
            while (true)
            {
                ColorChange playerChange;
                try
                {
                    playerChange = await waitForEvent<ColorChange>(ws, buffer);
                }
                catch (ClientDisconnectedException e)
                {
                    sessions.Remove(currentSession);
                    updatePlayers(new GameError(e));
                    return;
                }
                lock (SyncRoot)
                {
                    try
                    {
                        GameUpdate gameUpdate = game.handleChanges(playerChange, currentSession.Player);
                        updatePlayers(gameUpdate);

                        gameUpdate = game.createMapUpdate();
                        updatePlayers(gameUpdate);

                    }
                    catch(Exception e)
                    {
                        updatePlayers(new GameError(e));
                    }
                }
            }
        }

        private static async Task<EventType> waitForEvent<EventType>(HttpListenerWebSocketContext ws, byte[] buffer) where EventType : GameChange
        {
            while (true)
            {
                var packet = await ws.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), CancellationToken.None);

                if (packet.MessageType == WebSocketMessageType.Close)
                {
                    throw new ClientDisconnectedException("Web socket is closed!");
                }

                try
                {
                    EventType deserializedEvent = new JavaScriptSerializer().Deserialize<EventType>(Encoding.UTF8.GetString(buffer, 0, packet.Count));
                    return deserializedEvent;
                }
                catch (SystemException e)
                {
                    updatePlayers(new GameError(e));
                }
            }
        }

        private static void updatePlayers(GameUpdate gameUpdate)
        {
            byte[] buffer;
            Session[] tempSessions;
            lock (SyncRoot)
            {
                string json = new JavaScriptSerializer().Serialize(gameUpdate);
                buffer = Encoding.UTF8.GetBytes(json);
                tempSessions = sessions.ToArray();
            }
            foreach (var s in tempSessions)
            {
                s.WsContext.WebSocket.SendAsync(new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text, true, CancellationToken.None);
            }

        }

        public class ClientDisconnectedException : Exception
        {
            public ClientDisconnectedException(string message)  : base(message) { }
        }
    }
}
