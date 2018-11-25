using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public interface GameChange { }

    public class ColorChange : GameChange
    {
        public readonly Color newColor;

        public ColorChange(Color newColor)
        {
            this.newColor = newColor;
        }
    }

    public class JoinGame : GameChange
    {
        public readonly string playerName;
    }

    public class NewGame : GameChange
    {
        public readonly int playerNumber;
        public readonly int usedColors;
        public readonly int rows;
        public readonly int columns;

        public NewGame(int playerNumber, int usedColors, int rows, int columns)
        {
            this.playerNumber = playerNumber;
            this.usedColors = usedColors;
            this.rows = rows;
            this.columns = columns;
        }
    }
}
