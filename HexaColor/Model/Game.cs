using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class Game
    {
        private MapLayout mapLayout;
        private List<Player> players;
        private Queue<Position> availableStartingPositions;

        public Game(int playerNumber, int usedColors, int rows, int columns)
        {
            if (playerNumber != 2 || playerNumber != 4 || playerNumber != 8)
            {
                throw new ArgumentException("Player size must be 2, 4 or 8.");
            }
            if (usedColors < playerNumber + 2 || 20 < usedColors)
            {
                throw new ArgumentException("Invalid color number!");
            }
            players = new List<Player>();

            mapLayout = new MapLayout(rows, columns, usedColors);
            availableStartingPositions = mapLayout.getPlayerStartingPositions(playerNumber);
        }

        public Player addNewPlayer(string name = "default player name")
        {
            if (availableStartingPositions.Count != 0)
            {
                Position startingPosition = availableStartingPositions.Dequeue();
                Player newPlayer = new Player(startingPosition, name);
                players.Add(newPlayer);
                return newPlayer;
            }
            throw new InvalidOperationException(string.Format("All player slots are filled! Current players: {0}", players.Count));
        }

        public bool isGameFull()
        {
            return availableStartingPositions.Count == 0;
        }

        public List<GameUpdate> handleChanges(GameChange change, Player player)
        {
            if (change is ColorChange)
            {
                ColorChange colorChange = (ColorChange)change;
                mapLayout.changeContinousColors(player.startingPosition, colorChange.newColor);
            }

            if (isGameWon())
            {
                calculatePoints();
                //throw new NotImplementedException(); // TODO Handle the game won, send to clients
                return new GameWon(...);
            }

            // Calculate next player
            // TODO check if player can choose color
            int nextPlayerIndex = (players.IndexOf(player) + 1) % players.Count;
            Player nextPlayer = players.ElementAt(nextPlayerIndex);

            //throw new NotImplementedException(); // TODO Send next player to client
            return new NextPlayer(player, ...);
        }

        public bool isGameWon()
        {
            ISet<Color> differentColorsInGame = new HashSet<Color>();
            foreach (var pair in mapLayout.cells)
            {
                Cell cell = pair.Value;
                differentColorsInGame.Add(cell.color);
            }
            return differentColorsInGame.Count == players.Count;
        }

        public void calculatePoints()
        {
            Dictionary<Color, Player> colorToPlayer = new Dictionary<Color, Player>();
            foreach (Player player in players)
            {
                Cell playerCell;
                if (mapLayout.cells.TryGetValue(player.startingPosition, out playerCell))
                {
                    colorToPlayer.Add(playerCell.color, player);
                }
                else
                {
                    throw new InvalidOperationException("Invalid state, player must have a starting position!");
                }
            }

            foreach (var pair in mapLayout.cells)
            {
                Cell cell = pair.Value;
                Player player;
                if (colorToPlayer.TryGetValue(cell.color, out player))
                {
                    player.points++;
                }
                else
                {
                    throw new InvalidOperationException("Invalid state, game is not won!");
                }
            }
        }

        public MapUpdate createMapUpdate()
        {
            return new MapUpdate(mapLayout, players);
        }
    }

}
