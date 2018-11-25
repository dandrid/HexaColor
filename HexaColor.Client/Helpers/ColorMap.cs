using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HexaColor.Client.Helpers
{
    public static class ColorMap
    {
        public static Dictionary<Model.Color, SolidColorBrush> Items { get; private set; }

        static ColorMap()
        {
            Items = new Dictionary<Model.Color, SolidColorBrush>();
            Items.Add(Model.Color.COLOR_1, Brushes.Aquamarine);
            Items.Add(Model.Color.COLOR_2, Brushes.Azure);
            Items.Add(Model.Color.COLOR_3, Brushes.Bisque);
            Items.Add(Model.Color.COLOR_4, Brushes.Coral);
            Items.Add(Model.Color.COLOR_5, Brushes.Cyan);
            Items.Add(Model.Color.COLOR_6, Brushes.DarkGoldenrod);
            Items.Add(Model.Color.COLOR_7, Brushes.DarkOrange);
            Items.Add(Model.Color.COLOR_8, Brushes.Firebrick);
            Items.Add(Model.Color.COLOR_9, Brushes.GhostWhite);
            Items.Add(Model.Color.COLOR_10, Brushes.Gold);
            Items.Add(Model.Color.COLOR_11, Brushes.HotPink);
            Items.Add(Model.Color.COLOR_12, Brushes.IndianRed);
            Items.Add(Model.Color.COLOR_13, Brushes.Khaki);
            Items.Add(Model.Color.COLOR_14, Brushes.Lavender);
            Items.Add(Model.Color.COLOR_15, Brushes.Magenta);
            Items.Add(Model.Color.COLOR_16, Brushes.Navy);
            Items.Add(Model.Color.COLOR_17, Brushes.OliveDrab);
            Items.Add(Model.Color.COLOR_18, Brushes.PaleVioletRed);
            Items.Add(Model.Color.COLOR_19, Brushes.PeachPuff);
            Items.Add(Model.Color.COLOR_20, Brushes.Salmon);
        }

    }
}
