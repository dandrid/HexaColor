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
        public readonly Player player; // This can also be an ID if needed
        public readonly Color newColor;

        public ColorChange(Player player, Color newColor)
        {
            this.player = player;
            this.newColor = newColor;
        }
    }
}
