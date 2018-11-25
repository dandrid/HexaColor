using HexaColor.Model;
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
            httpListener.Prefixes.Add("http://localhost:8377/Design_Time_Addresses/G/");
            httpListener.Start();
            httpListener.BeginGetContext(OnHttpRequest, httpListener);
            Console.WriteLine("Service is up");
            using (var timer = new Timer(OnTimer, null, 0, 100))
                Console.ReadLine();
            httpListener.Stop();
        }

        private static void OnTimer(object state)
        {
            byte[] buffer;
            Session[] tempSessions;
            lock (SyncRoot)
            {
                string json = new JavaScriptSerializer().Serialize(game);
                buffer = Encoding.UTF8.GetBytes(json);
                tempSessions = sessions.ToArray();
            }
            foreach (var s in tempSessions)
            {
                s.WsContext.WebSocket.SendAsync(new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text, true, CancellationToken.None);
            }
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
            }
        }

        private static async void HandleWebSocketClient(HttpListenerContext context)
        {
            var ws = await context.AcceptWebSocketAsync(null);
            var buffer = new byte[10000];

            // wait for new game event, if the game is null
            if (game == null)
            {

                NewGame newGame = await waitForEvent<NewGame>(ws, buffer);
                lock (SyncRoot)
                {
                    // Create a new game
                    game = new Game(newGame.playerNumber, newGame.usedColors, newGame.rows, newGame.columns);
                }
            }

            Session currentSession;
            JoinGame joinGameEvent = await waitForEvent<JoinGame>(ws, buffer);
            lock (SyncRoot)
            {
                // Add player to game
                sessions.Add(currentSession = new Session(game.addNewPlayer(joinGameEvent.playerName), ws));
            }

            while (true)
            {
                ColorChange change = await waitForEvent<ColorChange>(ws, buffer);
                lock (SyncRoot)
                {
                    game.handleChanges(change, currentSession.Player);
                }
                updatePlayers();
            }
        }

        private static async Task<EventType> waitForEvent<EventType>(HttpListenerWebSocketContext ws, byte[] buffer) where EventType : GameChange
        {
            while (true)
            {
                var packet = await ws.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), CancellationToken.None);

                if (packet.MessageType == WebSocketMessageType.Close)
                {
                    throw new SystemException("Web socket is closed!");
                }

                try
                {
                    EventType deserializedEvent = new JavaScriptSerializer().Deserialize<EventType>(Encoding.UTF8.GetString(buffer, 0, packet.Count));
                    return deserializedEvent;
                }
                catch (SystemException e)
                {
                    throw new NotImplementedException(); // TODO send error to client, wrong event received
                }
            }
        }

        private static void updatePlayers()
        {
            byte[] buffer;
            Session[] tempSessions;
            lock (SyncRoot)
            {
                string json = new JavaScriptSerializer().Serialize(game.createMapUpdate());
                buffer = Encoding.UTF8.GetBytes(json);
                tempSessions = sessions.ToArray();
            }
            foreach (var s in tempSessions)
            {
                s.WsContext.WebSocket.SendAsync(new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
