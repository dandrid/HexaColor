using HexaColor.Client.Helpers;
using HexaColor.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexaColor.Client.Views
{
    /// <summary>
    /// Interaction logic for MapLayoutView.xaml
    /// </summary>
    public partial class MapLayoutView : UserControl
    {
        private Dictionary<Model.Color, Style> coloredStyles;
        private Point Q;
        private readonly int mapSize = 7;   // example value, must be odd

        public MapLayoutView()
        {
            InitializeComponent();
            InitStyles();
            InitCanvas();
            InitHexagons();
        }

        private void InitCanvas()
        {
            MapLayoutCanvas.Width = 300;
            MapLayoutCanvas.Height = 300;
        }

        private void InitStyles()
        {
            // example
            double canvasSize = 300.0;

            // trigonometry magic
            int half = mapSize / 2;
            double c = canvasSize / mapSize;
            double d = (c / 2) / Math.Sin((60.0 / 180.0) * Math.PI);
            double b = (canvasSize - (half + 1) * d) / mapSize;
            double a = d + b;
            double e = d / 2.0;
            Point A = new Point(0.0, 0.0);
            Point F = new Point(e, c / 2.0);
            Point E = new Point(e + b, c / 2.0);
            Point D = new Point(a, 0.0);
            Point B = new Point(e, -c / 2.0);
            Point C = new Point(e + b, -c / 2.0);

            Q = new Point(e + b, c / 2.0);

            coloredStyles = new Dictionary<Model.Color, Style>();
            foreach (var item in ColorMap.Items)
            {
                FrameworkElementFactory elementFactory = new FrameworkElementFactory(typeof(Path));
                elementFactory.SetValue(Path.FillProperty, item.Value);
                elementFactory.SetValue(Path.StrokeProperty, Brushes.Black);
                elementFactory.SetValue(Path.StrokeThicknessProperty, 0.1);
                elementFactory.SetValue(Path.DataProperty, new PathGeometry
                {
                    Figures =
                {
                    new PathFigure
                    {
                        StartPoint = new Point(0.0, 0.0),
                        IsClosed = true,
                        IsFilled = true,
                        Segments =
                        {
                            new LineSegment(B, true),
                            new LineSegment(C, true),
                            new LineSegment(D, true),
                            new LineSegment(E, true),
                            new LineSegment(F, true)
                        }
                    }
                }
                });

                ControlTemplate controlTemplate = new ControlTemplate();
                controlTemplate.VisualTree = elementFactory;

                Style hexagonStyle = new Style
                {
                    TargetType = typeof(Button),
                    Setters =
                {
                    new Setter
                    {
                        Property = Control.TemplateProperty,
                        Value = controlTemplate
                    }
                }
                };
                coloredStyles.Add(item.Key, hexagonStyle);
            }
        }

        private void InitHexagons()
        {
            for(int row = 0; row < mapSize; row++)
            {
                for(int col = 0; col < mapSize; col++)
                {
                    Button hexagon = new Button();
                    hexagon.Style = coloredStyles[Model.Color.COLOR_1];
                    Canvas.SetLeft(hexagon, col * Q.X);
                    if(col % 2 == 1 && row == mapSize - 1)
                    {
                        // skip this item
                        continue;
                    }
                    else if (col % 2 == 1 && row != mapSize - 1)
                    {
                        Canvas.SetTop(hexagon, row * 2 * Q.Y + Q.Y * 2.0);
                    }
                    else
                    {
                        Canvas.SetTop(hexagon, row * 2 * Q.Y + Q.Y);
                    }
                    MapLayoutCanvas.Children.Add(hexagon);
                }
            }

        }
    }
}
