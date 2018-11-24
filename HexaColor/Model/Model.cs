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
            hashCode = hashCode * -1521134295 + XCooridnate.GetHashCode();
            hashCode = hashCode * -1521134295 + YCooridnate.GetHashCode();
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
                    if(isValidPosition(possiblePosition))
                    {
                        Cell neighbourCell;
                        cells.TryGetValue(possiblePosition, out neighbourCell);
                        actualNeighbourCells.Add(neighbourCell);
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
        }

    }
}
