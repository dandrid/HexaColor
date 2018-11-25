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
        private Player nextPlayer;
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
            nextPlayer = null;

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

        public GameUpdate handleChanges(GameChange change, Player player)
        {
            if (change is ColorChange)
            {
                if (nextPlayer != player)
                {
                    throw new InvalidOperationException(string.Format("Not this player turn! Next player is: {0}, but {1} sent a color", nextPlayer.name, player.name));
                }
                ColorChange colorChange = (ColorChange)change;
                mapLayout.changeContinousColors(player.startingPosition, colorChange.newColor);
            }

            if (isGameWon())
            {
                calculatePoints();
                return new GameWon(players.OrderBy(p => p.points).ToList());
            }

            // Calculate next player
            // TODO check if player can choose color
            int nextPlayerIndex = (players.IndexOf(player) + 1) % players.Count;
            nextPlayer = players.ElementAt(nextPlayerIndex);

            Cell playerCell;
            if (mapLayout.cells.TryGetValue(player.startingPosition, out playerCell))
            {
                HashSet<Color> availableColors = playerCell.getContinousNeighbourColors();
                return new NextPlayer(nextPlayer, availableColors.ToList<Color>());
            }
            else
            {
                throw new InvalidOperationException("Invalid state, player must have a starting position!");
            }
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
