using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class MapLayout
    {
        public int usedColors { get; set; }
        public int mapSize { get; set; }
        public Dictionary<Position, Cell> cells { get; set; }

        public MapLayout() // For serialization
        {

        }

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
        }

        /**
         * Changes all continious colors starting from the given position 
         */
        public void changeContinousColors(Position position, Color newColor)
        {
            Cell cell = cells[position];
            Color oldColor = cell.color;
            cell.color = newColor;

            List<Position> neighbours = getNeighbourCellPositions(position);
            foreach (Position neighbourPosition in neighbours)
            {
                Cell neighbourCell = cells[neighbourPosition];
                if (neighbourCell.color == oldColor)
                {
                    changeContinousColors(neighbourPosition, newColor);
                }
            }
        }

        public HashSet<Color> getContinousNeighbourColors(Position position)
        {
            Color startingColor = cells[position].color;
            HashSet<Color> differentColors = new HashSet<Color>();
            Action<Position> getNeighbourColors = (pos) =>
            {
                foreach(Position neighbourPos in getNeighbourCellPositions(pos))
                {
                    if(cells[neighbourPos].color != startingColor)
                    {
                        differentColors.Add(cells[pos].color);
                    }
                }
            };

            visitContiniousNeighbours(getNeighbourColors, position);
            return differentColors;
        }

        public bool areColorNeighbours(Position one, Position other)
        {
            HashSet<Position> onePositionsAndNeighbours = new HashSet<Position>(); //
            HashSet<Position> otherPositions = new HashSet<Position>();
            visitContiniousNeighbours((pos) => 
            {
                onePositionsAndNeighbours.Add(pos);
                foreach(Position neighbourPos in getNeighbourCellPositions(pos))
                {
                    onePositionsAndNeighbours.Add(neighbourPos);
                }
            }, one);
            visitContiniousNeighbours((pos) => otherPositions.Add(pos), other);

            //check if they have an intersection
            foreach(Position otherPos in otherPositions)
            {
                if (onePositionsAndNeighbours.Add(otherPos) == false) // Position is already a neighbour of one, so they are color neighbours
                {
                    return true;
                }
            }
            return false;
        }

        public List<Position> getNeighbourCellPositions(Position position)
        {
            var possibleNeighbourPositions = new List<Position>();

            possibleNeighbourPositions.Add(new Position(position.rowCooridnate - 1, position.columnCooridnate));
            possibleNeighbourPositions.Add(new Position(position.rowCooridnate, position.columnCooridnate - 1));
            possibleNeighbourPositions.Add(new Position(position.rowCooridnate, position.columnCooridnate + 1));
            possibleNeighbourPositions.Add(new Position(position.rowCooridnate + 1, position.columnCooridnate - 1));
            possibleNeighbourPositions.Add(new Position(position.rowCooridnate + 1, position.columnCooridnate));
            possibleNeighbourPositions.Add(new Position(position.rowCooridnate + 1, position.columnCooridnate + 1));

            var actualNeighbourPositions = new List<Position>();
            foreach (var possiblePosition in possibleNeighbourPositions)
            {
                if (isValidPosition(possiblePosition))
                {
                    actualNeighbourPositions.Add(possiblePosition);
                }
            }
            return actualNeighbourPositions;
            
        }
        public void visitContiniousNeighbours(Action<Position> action, Position position)
        {
            visitContiniousNeighbours(action, position, cells[position].color, new HashSet<Cell>());
        }

        private void visitContiniousNeighbours(Action<Position> action, Position position, Color startingColor, HashSet<Cell> visiteCells)
        {
            Cell cell = cells[position];
            action(position);
            visiteCells.Add(cell);

            List<Position> neighbourCellPositions = getNeighbourCellPositions(position);
            foreach (Position neighbourCellPosition in neighbourCellPositions)
            {
                Cell neighbourCell = cells[neighbourCellPosition];
                if (neighbourCell.color == startingColor && !visiteCells.Contains(neighbourCell))
                {
                    visitContiniousNeighbours(action, position, startingColor, visiteCells);
                }
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
