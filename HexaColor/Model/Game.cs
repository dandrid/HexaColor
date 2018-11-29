using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class Game
    {
        public MapLayout mapLayout;
        public List<Player> players;
        public Player nextPlayer;
        public Queue<Position> availableStartingPositions;

        public Game(NewGame parameters)
        {
            int allPlayers = parameters.playerNumber + parameters.aiPlayers.Count;
            if (!(allPlayers == 2 || allPlayers == 4 || allPlayers == 8))
            {
                throw new ArgumentException("Player size must be 2, 4 or 8.");
            }
            if (parameters.usedColors < allPlayers + 2 || 20 < parameters.usedColors)
            {
                throw new ArgumentException("Invalid color number!");
            }
            players = new List<Player>();
            nextPlayer = null;

            mapLayout = new MapLayout(parameters.rows, parameters.columns, parameters.usedColors);
            availableStartingPositions = mapLayout.getPlayerStartingPositions(allPlayers);

            foreach(var aiConfig in parameters.aiPlayers)
            {
                AiPlayer aiPlayer = new AiPlayer(aiConfig.Value, availableStartingPositions.Dequeue(), aiConfig.Key);
                players.Add(aiPlayer);
            }
        }

        public Player addNewPlayer(string name = "default player name")
        {
            if (availableStartingPositions.Count != 0)
            {
                Position startingPosition = availableStartingPositions.Dequeue();
                Player newPlayer = new Player(startingPosition, name);
                players.Add(newPlayer);
                if(availableStartingPositions.Count == 0)
                {
                    nextPlayer = newPlayer;
                }
                return newPlayer;
            }
            throw new InvalidOperationException(string.Format("All player slots are filled! Current players: {0}", players.Count));
        }

        public GameUpdate getNextPlayer()
        {
            if(nextPlayer != null)
            {
                return new NextPlayer(nextPlayer, getAvailableColors(nextPlayer).ToList());
            }
            return null;
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
                    if (nextPlayer == null)
                    {
                        throw new InvalidOperationException("The game not started yet!");
                    }
                    throw new InvalidOperationException(string.Format("Not this players turn! Next player is: {0}, but {1} sent a color", nextPlayer.name, player.name));
                }
                ColorChange colorChange = (ColorChange)change;
                mapLayout.changeContinousColors(player.startingPosition, colorChange.newColor);
            }


            for (int i = 1; i <= players.Count; i++) // try to find next player
            {
                // Calculate possible next player
                int nextPlayerIndex = (players.IndexOf(player) + i) % players.Count;
                Player possibleNextPlayer = players.ElementAt(nextPlayerIndex);

                HashSet<Color> availableColors = getAvailableColors(possibleNextPlayer);
                if (availableColors.Count != 0) // If Player cannot choose, so we skipp it
                {
                    nextPlayer = possibleNextPlayer;
                    if(nextPlayer is AiPlayer) // If the player is AI, we do its turn
                    {
                        AiPlayer aiPlayer = nextPlayer as AiPlayer;
                        Color chosenColor = aiPlayer.chooseColor(mapLayout, availableColors);
                        mapLayout.changeContinousColors(aiPlayer.startingPosition, chosenColor);

                        i = 0; // Reset the cycle to find a next player
                        player = aiPlayer;
                        continue;
                    }
                    else
                    {
                        return new NextPlayer(nextPlayer, availableColors.ToList<Color>());
                    }
                }
            }

            // Game is won
            calculatePoints();
            return new GameWon(players.OrderBy(p => p.points).ToList());
        }

        private HashSet<Color> getAvailableColors(Player player)
        {
            HashSet<Color> availableColors = mapLayout.getContinousNeighbourColors(player.startingPosition);
            foreach (Player p in players) // Do not allow the color of the other player, if they are neighbours
            {
                if (p != player)
                {
                    Color playerColor = mapLayout.cells[p.startingPosition].color;
                    if (availableColors.Contains(playerColor))
                    {
                        if (mapLayout.areColorNeighbours(player.startingPosition, p.startingPosition))
                        {
                            availableColors.Remove(playerColor);
                        }
                    }
                }
            }
            return availableColors;
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
            foreach (Player player in players)
            {
                mapLayout.visitContiniousNeighbours((pos) =>
               {
                   player.points++;
               }, player.startingPosition);
            }
        }

        public MapUpdate createMapUpdate()
        {
            return new MapUpdate(mapLayout, players);
        }
    }

}
