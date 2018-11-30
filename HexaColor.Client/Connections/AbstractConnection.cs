using System;
using System.Threading.Tasks;
using HexaColor.Model;

namespace HexaColor.Client.Connections
{
    public abstract class AbstractConnection : IConnection
    {
        public event EventHandler<GameWonEventArgs> GameWonEvent;
        public event EventHandler<GameErrorEventArgs> GameErrorEvent;
        public event EventHandler<MapUpdateEventArgs> MapUpdatEvent;
        public event EventHandler<NextPlayerEventArgs> NextPlayerEvent;

        public abstract Task Connect();
        public abstract Task Send(GameChange message);
        public abstract void StartListening();

        protected virtual void OnGameWonEvent(GameWonEventArgs e)
        {
            GameWonEvent?.Invoke(this, e);
        }
        protected virtual void OnGameErrorEvent(GameErrorEventArgs e)
        {
            GameErrorEvent?.Invoke(this, e);
        }
        protected virtual void OnMapUpdatEvent(MapUpdateEventArgs e)
        {
            MapUpdatEvent?.Invoke(this, e);
        }
        protected virtual void OnNextPlayerEvent(NextPlayerEventArgs e)
        {
            NextPlayerEvent?.Invoke(this, e);
        }
    }

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

    public class GameWonEventArgs : System.EventArgs
    {
        public readonly GameWon gameWon;

        public GameWonEventArgs(GameWon gameWon)
        {
            this.gameWon = gameWon;
        }
    }
}