using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
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

            foreach (Int32 rowNumber in rowNumbers)
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
            foreach (var pair in cells)
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
            if (position.rowCooridnate < 0 || position.columnCooridnate < 0
                || position.rowCooridnate >= mapSize || position.columnCooridnate >= mapSize)
            {
                return false;
            }
            return true;
        }

        private Random random = new Random();
        private Color getRandomColor(int usedColors)
        {
            Array values = Enum.GetValues(typeof(Color));
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
}
