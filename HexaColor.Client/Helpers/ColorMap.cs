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
            Items.Add(Model.Color.COLOR_1, Brushes.Red);
            Items.Add(Model.Color.COLOR_2, Brushes.LimeGreen);
            Items.Add(Model.Color.COLOR_3, Brushes.MediumBlue);
            Items.Add(Model.Color.COLOR_4, Brushes.BlueViolet);
            Items.Add(Model.Color.COLOR_5, Brushes.LightSkyBlue);
            Items.Add(Model.Color.COLOR_6, Brushes.LightSlateGray);
            Items.Add(Model.Color.COLOR_7, Brushes.Olive);
            Items.Add(Model.Color.COLOR_8, Brushes.Crimson);
            Items.Add(Model.Color.COLOR_9, Brushes.Tomato);
            Items.Add(Model.Color.COLOR_10, Brushes.Gainsboro);
            Items.Add(Model.Color.COLOR_11, Brushes.Gold);
            Items.Add(Model.Color.COLOR_12, Brushes.Yellow);
            Items.Add(Model.Color.COLOR_13, Brushes.Brown);
            Items.Add(Model.Color.COLOR_14, Brushes.DarkTurquoise);
            Items.Add(Model.Color.COLOR_15, Brushes.GreenYellow);
            Items.Add(Model.Color.COLOR_16, Brushes.Magenta);
            Items.Add(Model.Color.COLOR_17, Brushes.Khaki);
            Items.Add(Model.Color.COLOR_18, Brushes.Orange);
            Items.Add(Model.Color.COLOR_19, Brushes.Violet);
            Items.Add(Model.Color.COLOR_20, Brushes.RosyBrown);
        }

    }
}
