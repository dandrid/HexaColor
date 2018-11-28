using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public interface GameUpdate { }

    public class MapUpdate : GameUpdate
    {
        public MapLayout mapLayout { get; set; }
        public List<Player> players { get; set; }

        public MapUpdate() { }

        public MapUpdate(MapLayout mapLayout, List<Player> players)
        {
            this.mapLayout = mapLayout;
            this.players = players;
        }
    }

    public class NextPlayer : GameUpdate
    {
        public Player player { get; set; }
        public List<Color> avilableColors { get; set; }

        public NextPlayer() { }

        public NextPlayer(Player player, List<Color> avilableColors)
        {
            this.player = player;
            this.avilableColors = avilableColors;
        }
    }

    public class GameWon : GameUpdate
    {
        /*
         * Players ordered by the points. Index 0 is the last player.
        */
        public List<Player> players { get; set; }

        public GameWon() { }

        public GameWon(List<Player> players)
        {
            this.players = players;
        }
    }

    public class GameError : GameUpdate
    {
        public string error { get; set; }

        public GameError() { }

        public GameError(Exception exception)
        {
            this.error = exception.Message;
        }
    }
}
