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
        public readonly MapLayout mapLayout;
        public readonly List<Player> players;

        public MapUpdate(MapLayout mapLayout, List<Player> players)
        {
            this.mapLayout = mapLayout;
            this.players = players;
        }
    }

    public class NextPlayer : GameUpdate
    {
        public readonly Player player;
        public readonly List<Color> avilableColors;

        public NextPlayer(Player player, List<Color> avilableColors)
        {
            this.player = player;
            this.avilableColors = avilableColors;
        }
    }

    public class GameWon : GameUpdate
    {
        /*
         * Players ordered by the points. Index 0 is the first.
        */
        public readonly List<Player> players;

        public GameWon(List<Player> players)
        {
            this.players = players;
        }
    }

    public class GameError : GameUpdate
    {
        public readonly Exception exception;

        public GameError(Exception exception)
        {
            this.exception = exception;
        }
    }
}
