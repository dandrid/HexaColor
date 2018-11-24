using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public enum Color
    {
        COLOR_1,
        COLOR_2,
        COLOR_3,
        COLOR_4,
        COLOR_5,
        COLOR_6,
        COLOR_7,
        COLOR_8,
        COLOR_9,
        COLOR_10,
        COLOR_11,
        COLOR_12,
        COLOR_13,
        COLOR_14,
        COLOR_15,
        COLOR_16,
        COLOR_17,
        COLOR_18,
        COLOR_19,
        COLOR_20
    }

    public class Position
    {
        public Position(int rowCooridnate, int columnCooridnate)
        {
            this.rowCooridnate = rowCooridnate;
            this.columnCooridnate = columnCooridnate;
        }
        public readonly int rowCooridnate;
        public readonly int columnCooridnate;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if(obj is Position)
            {
                var another = (Position)obj;
                return rowCooridnate == another.rowCooridnate && columnCooridnate == another.columnCooridnate;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 484048395;
            hashCode = hashCode * -1521134295 + rowCooridnate.GetHashCode();
            hashCode = hashCode * -1521134295 + columnCooridnate.GetHashCode();
            return hashCode;
        }
    }

    public class Model
    {
    }

    public class Cell
    {
        public Cell(Color color)
        {
            this.color = color;
        }
        public Color color { get; internal set; }
        public List<Cell> neighbourCells { get; set; }

        public void changeContinousColors(Color newColor)
        {
            Color oldColor = color;
            color = newColor;

            foreach(Cell cell in neighbourCells) 
            {
                if (cell.color == oldColor)
                {
                    cell.changeContinousColors(newColor);
                }
            }
        }
    }

    public class MapLayout
    {
        private int usedColors;
        private int mapSize;
        public Dictionary<Position, Cell> cells { get; internal set; }

        /**
         * Create a map with the given parameters, and initialize with random colors 
         */
        public MapLayout(int rows, int columns, int usedColors)
        {
            
            if (rows < 3 || columns < 3)
            {
                throw new ArgumentException("Map size must be greater or equals than 3");
            }
            if (rows != columns)
            {
                throw new ArgumentException("Map must be quadratic");
            }
            if (rows % 2 == 0 || rows % 2 == 0)
            {
                throw new ArgumentException("Map size must be impair");
            }
            this.usedColors = usedColors;
            this.mapSize = rows;
            this.cells = new Dictionary<Position, Cell>();

            IEnumerable<Int32> rowNumbers = Enumerable.Range(0, mapSize);
            IEnumerable<Int32> columnNumbers = Enumerable.Range(0, mapSize);

            foreach(Int32 rowNumber in rowNumbers) 
            {
                foreach (Int32 columnNumber in columnNumbers)
                {
                    Cell newCell = new Cell(getRandomColor(usedColors));
                    cells.Add(new Position(rowNumber, columnNumber), newCell);
                }
            }

            setNeighbourCellValues();
        }

        private void setNeighbourCellValues()
        {
            foreach(var pair in cells)
            {
                Position position = pair.Key;
                Cell cell = pair.Value;

                var possibleNeighbourPositions = new List<Position>();

                possibleNeighbourPositions.Add(new Position(position.rowCooridnate - 1, position.columnCooridnate));
                possibleNeighbourPositions.Add(new Position(position.rowCooridnate, position.columnCooridnate - 1));
                possibleNeighbourPositions.Add(new Position(position.rowCooridnate, position.columnCooridnate + 1));
                possibleNeighbourPositions.Add(new Position(position.rowCooridnate + 1, position.columnCooridnate - 1));
                possibleNeighbourPositions.Add(new Position(position.rowCooridnate + 1, position.columnCooridnate));
                possibleNeighbourPositions.Add(new Position(position.rowCooridnate + 1, position.columnCooridnate + 1));

                var actualNeighbourCells = new List<Cell>();
                foreach (var possiblePosition in possibleNeighbourPositions)
                {
                    if (isValidPosition(possiblePosition))
                    {
                        Cell neighbourCell;
                        if (cells.TryGetValue(possiblePosition, out neighbourCell))
                        {
                            actualNeighbourCells.Add(neighbourCell);
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("Invalid state, cell not exists, row: {0}, column: {1}", possiblePosition.rowCooridnate, possiblePosition.columnCooridnate));
                        }
                    }
                }
                cell.neighbourCells = actualNeighbourCells;
            }
        }

        private bool isValidPosition(Position position)
        {
            if(position.rowCooridnate < 0 || position.columnCooridnate < 0
                || position.rowCooridnate >= mapSize || position.columnCooridnate >= mapSize)
            {
                return false;
            }
            return true;
        }

        private Color getRandomColor(int usedColors)
        {
            Array values = Enum.GetValues(typeof(Color));
            Random random = new Random();
            Color randomColor = (Color)values.GetValue(random.Next(usedColors));
            return randomColor;
        }

        /**
         * Changes all continious colors starting from the given position 
         */
        public void changeContinousColors(Position position, Color color)
        {
            Cell cell;
            if (cells.TryGetValue(position, out cell))
            {
                cell.changeContinousColors(color);
            }
            else
            {
                throw new ArgumentException("Position not found in the game!");
            }
        }

        public Queue<Position> getPlayerStartingPositions(int playerNumber)
        {
            Queue<Position> startingPositions = new Queue<Position>();
            if (playerNumber == 2)
            {
                startingPositions.Enqueue(new Position(0, 0));
                startingPositions.Enqueue(new Position(mapSize - 1, mapSize - 1));
            }
            else if (playerNumber == 4)
            {
                startingPositions.Enqueue(new Position(0, 0));
                startingPositions.Enqueue(new Position(0, mapSize - 1));
                startingPositions.Enqueue(new Position(mapSize - 1, 0));
                startingPositions.Enqueue(new Position(mapSize - 1, mapSize - 1));
            }
            else if (playerNumber == 8)
            {
                int middleIndex = mapSize / 2;

                // Upper 2
                startingPositions.Enqueue(new Position(0, middleIndex - 1));
                startingPositions.Enqueue(new Position(0, middleIndex + 1));

                // Down 2
                startingPositions.Enqueue(new Position(mapSize - 1, middleIndex - 1));
                startingPositions.Enqueue(new Position(mapSize - 1, middleIndex + 1));

                // Left 2
                startingPositions.Enqueue(new Position(middleIndex - 1, 0));
                startingPositions.Enqueue(new Position(middleIndex + 1, 0));

                // Right 2
                startingPositions.Enqueue(new Position(middleIndex - 1, mapSize - 1));
                startingPositions.Enqueue(new Position(middleIndex + 1, mapSize - 1));
            }
            else
            {
                throw new InvalidOperationException(string.Format("Invalid state, wrong number of players: {0}", playerNumber));
            }
            return startingPositions;
        }
    }

    public class Player
    {
        public readonly Position startingPosition;
        public string name { get; set; }
        public int points { get; set; }

        public Player(Position startingPosition, string name)
        {
            this.startingPosition = startingPosition;
            this.name = name;
            this.points = 0;
        }
    }

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

        public void handleChanges(GameChange change)
        {
            if(change is ColorChange)
            {
                ColorChange colorChange = (ColorChange) change;
                mapLayout.changeContinousColors(colorChange.player.startingPosition, colorChange.newColor);

                // Calculate next player
                int nextPlayerIndex = (players.IndexOf(colorChange.player) + 1) % players.Count;
                Player nextPlayer = players.ElementAt(nextPlayerIndex);

                throw new NotImplementedException(); // TODO Send next player to client
            }

            if (isGameWon())
            {
                calculatePoints();
                throw new NotImplementedException(); // TODO Handle the game won, send to clients
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
            foreach(Player player in players)
            {
                Cell playerCell;
                if(mapLayout.cells.TryGetValue(player.startingPosition, out playerCell))
                {
                    colorToPlayer.Add(playerCell.color, player);
                }
                else
                {
                    throw new InvalidOperationException("Invalid state, player must have a starting position!");
                }
            }
            
            foreach(var pair in mapLayout.cells)
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
    }

    public interface GameChange { }

    public class ColorChange : GameChange
    {
        public readonly Player player; // This can also be an ID if needed
        public readonly Color newColor;

        public ColorChange(Player player, Color newColor)
        {
            this.player = player;
            this.newColor = newColor;
        }
    }
}
