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
        public Color newColor { get; set; }

        public ColorChange() { }
        public ColorChange(Color newColor)
        {
            this.newColor = newColor;
        }
    }

    public class JoinGame : GameChange
    {
        public string playerName { get; set; }

        public JoinGame() { }

        public JoinGame(string playerName)
        {
            this.playerName = playerName;
        }
    }

    public class NewGame : GameChange
    {
        public int playerNumber { get; set; }
        public int usedColors { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }


        public NewGame() { }

        public NewGame(int playerNumber, int usedColors, int rows, int columns)
        {
            this.playerNumber = playerNumber;
            this.usedColors = usedColors;
            this.rows = rows;
            this.columns = columns;
        }
    }
}
