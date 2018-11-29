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
    /// Interaction logic for InGameControlsView.xaml
    /// </summary>
    public partial class InGameControlsView : UserControl
    {
        private Button[] colorButtons;

        private InGameControlsModel Model
        {
            get
            {
                return (Application.Current.MainWindow.FindName("RightPanel") as ContentControl).DataContext as InGameControlsModel;
            }
        }

        public InGameControlsView()
        {
            InitializeComponent();
            InitColors();
            Model.NextPlayerEvent += Model_NextPlayerEvent;
        }

        private void Model_NextPlayerEvent()
        {
            MessageBox.Show("Your turn.");
            EnableleColorButtons(Model.AvailableColors);
        }

        private void InitColors()
        {
            colorButtons = (ChangeBtn.Parent as Grid).Children.OfType<Button>().Where(b => Grid.GetRow(b) < 4).ToArray();
            if(colorButtons.Count() != ColorMap.Items.Count)
            {
                throw new Exception("Model-view mismatch. There are less or more model elements than visual elements.");
            }
            DisableColorButtons();
        }

        private void DisableColorButtons()
        {
            for (int i = 0; i < colorButtons.Count(); i++)
            {
                Color origColor = ColorMap.Items.ElementAt(i).Value.Color;
                float factor = 0.4f;
                Color newColor = Color.FromScRgb(origColor.ScA * factor, origColor.ScR * factor, origColor.ScG * factor, origColor.ScB * factor);
                colorButtons[i].Background = new SolidColorBrush(newColor);
                colorButtons[i].IsEnabled = false;
            }
        }

        private void EnableleColorButtons(List<Model.Color> availableColors)
        {
            for (int i = 0; i < colorButtons.Count(); i++)
            {
                if (availableColors.Contains(ColorMap.Items.ElementAt(i).Key))
                {
                    colorButtons[i].Background = ColorMap.Items.ElementAt(i).Value;
                    colorButtons[i].IsEnabled = true;
                }
            }
        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            MessageBox.Show("Change!");
            DisableColorButtons();
        }

        private void SkipBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            EnableleColorButtons(Model.AvailableColors);
            MessageBox.Show("Skip");
        }

        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ChangeBtn.Background = btn.Background;
            ChangeBtn.IsEnabled = true;
        }
    }
}
