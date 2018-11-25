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
    /// Interaction logic for CreatingControlsView.xaml
    /// </summary>
    public partial class CreatingControlsView : UserControl
    {
        private object RightPanelDataContext
        {
            set
            {
                (Application.Current.MainWindow.FindName("RightPanel") as ContentControl).DataContext = value;
            }
        }
        private object LeftPanelDataContext
        {
            set
            {
                (Application.Current.MainWindow.FindName("LeftPanel") as ContentControl).DataContext = value;
            }
        }

        public CreatingControlsView()
        {
            InitializeComponent();

            //chosenColors = new List<Model.Color>();
            //InitColors();
        }

        //private void InitColors()
        //{
        //    foreach(var item in ColorMap.Items)
        //    {
        //        Button btn = new Button();
        //        btn.Background = item.Value;
        //        btn.Style = FindResource("NotPressedColorBtnStyle") as Style;
        //        btn.Click += ColorBtn_Click;
        //        ColorsPanel.Children.Add(btn);
        //    }
        //}

        //private void ColorBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btn = sender as Button;
        //    Model.Color btnColor = ColorMap.Items.Where(c => c.Value.Equals(btn.Background)).First().Key;
        //    if (chosenColors.Contains(btnColor))
        //    {
        //        chosenColors.Remove(btnColor);
        //        btn.Style = FindResource("NotPressedColorBtnStyle") as Style;
        //    } else
        //    {
        //        chosenColors.Add(btnColor);
        //        btn.Style = FindResource("PressedColorBtnStyle") as Style;
        //    }
        //}

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanelDataContext = new StartingControlsModel();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text;
            int.TryParse((PlayerBox.SelectedItem as ComboBoxItem).Content.ToString(), out int playerCnt);
            int.TryParse((ColorBox.SelectedItem as ComboBoxItem).Content.ToString(), out int colorCnt);
            int.TryParse((SizeBox.SelectedItem as ComboBoxItem).Content.ToString().Substring(0,1), out int size);

            MapLayoutModel mapLayoutModel = new MapLayoutModel(name, 2, colorCnt, size); // TODO player number
            mapLayoutModel.InitMapLayout();
            LeftPanelDataContext = mapLayoutModel;
            //MessageBox.Show(String.Format("Initialized Name: {0}, Color count: {1}, Map size: {2}", name, colorCnt, size));
        }
    }
}
