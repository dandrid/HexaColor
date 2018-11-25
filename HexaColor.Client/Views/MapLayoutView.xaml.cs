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
using Color = HexaColor.Model.Color;

namespace HexaColor.Client.Views
{
    /// <summary>
    /// Interaction logic for MapLayoutView.xaml
    /// </summary>
    public partial class MapLayoutView : UserControl
    {
        private Dictionary<Model.Color, Style> coloredStyles;
        private Point Q;
        private MapLayoutModel Model
        {
            get
            {
                return (Application.Current.MainWindow.FindName("LeftPanel") as ContentControl).DataContext as MapLayoutModel;
            }
        }

        private object LeftPanelDataContext
        {
            set
            {
                (Application.Current.MainWindow.FindName("LeftPanel") as ContentControl).DataContext = value;
            }
        }

        public MapLayoutView()
        {
            InitializeComponent();
            Model.MapLayoutInitialized += Model_MapLayoutInitialized;   
        }

        private void Model_MapLayoutInitialized()
        {
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
            double canvasSize = 300.0;

            // trigonometry magic
            int half = Model.MapSize / 2;
            double c = canvasSize / Model.MapSize;
            double d = (c / 2) / Math.Sin((60.0 / 180.0) * Math.PI);
            double b = (canvasSize - (half + 1) * d) / Model.MapSize;
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
            foreach (var item in Model.GameModel.mapLayout.cells)
            {
                int col = item.Key.columnCooridnate;
                int row = item.Key.rowCooridnate;
                Button hexagon = new Button();
                hexagon.Style = coloredStyles[item.Value.color];
                Canvas.SetLeft(hexagon, col * Q.X);
                if (col % 2 == 1 && row == Model.MapSize - 1)
                {
                    // skip this item 
                    continue;
                }
                else if (col % 2 == 1 && row != Model.MapSize - 1)
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
