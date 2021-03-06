﻿using HexaColor.Model;
using HexaColor.Server.ModelManipulation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    class Server
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
                try
                {
                    HandleWebSocketClient(context);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private static async void HandleWebSocketClient(HttpListenerContext context)
        {

            Session currentSession = null;
            try
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
                        game = new Game(newGame);
                    }
                }

                // wait for player to join
                JoinGame joinGameEvent = await waitForEvent<JoinGame>(ws, buffer);
                lock (SyncRoot)
                {
                    // Add player to game
                    sessions.Add(currentSession = new Session(game.addNewPlayer(joinGameEvent.playerName), ws));
                }
                updatePlayers(game.createMapUpdate());
                lock (SyncRoot)
                {
                    updatePlayers(game.getNextPlayer());
                }
                // handle game changes
                while (true)
                {
                    ColorChange playerChange;
                    playerChange = await waitForEvent<ColorChange>(ws, buffer);
                    lock (SyncRoot)
                    {
                        try
                        {
                            GameUpdate gameUpdate = game.handleChanges(playerChange, currentSession.Player);
                            updatePlayers(gameUpdate);

                            gameUpdate = game.createMapUpdate();
                            updatePlayers(gameUpdate);

                        }
                        catch (Exception e)
                        {
                            updatePlayers(new GameError(e));
                        }
                    }
                }
            }
            catch(ClientDisconnectedException e1)
            {
                if (currentSession != null)
                {
                    Console.WriteLine("Player disconnected: " + currentSession.Player.name);
                    lock (SyncRoot)
                    {
                        sessions.Remove(currentSession);
                    }
                    updatePlayers(new GameError(e1));
                }
            }
            catch (WebSocketException e2)
            {
                if (currentSession != null)
                {
                    Console.WriteLine("Player disconnected: " + currentSession.Player.name);
                    lock (SyncRoot)
                    {
                        sessions.Remove(currentSession);
                    }
                    updatePlayers(new GameError(e2));
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
                    string text = Encoding.UTF8.GetString(buffer, 0, packet.Count);
                    EventType deserializedEvent = JsonConvert.DeserializeObject<EventType>(text);
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
            if(gameUpdate != null)
            {
                byte[] buffer;
                Session[] tempSessions;
                lock (SyncRoot)
                {
                    string json = JsonConvert.SerializeObject(gameUpdate, new KeyValuePairConverter());
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

        public class ClientDisconnectedException : Exception
        {
            public ClientDisconnectedException(string message)  : base(message) { }
        }
    }
}
