using HexaColor.Client.Connections;
using HexaColor.Client.ViewModels;
using HexaColor.Networking;
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
    /// Interaction logic for StartingControlsView.xaml
    /// </summary>
    public partial class StartingControlsView : UserControl
    {
        private object RightPanelDataContext
        {
            set
            {
                (Application.Current.MainWindow.FindName("RightPanel") as ContentControl).DataContext = value;
            }
        }

        public StartingControlsView()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanelDataContext = new CreatingControlsModel();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Connect");
            RightPanelDataContext = new InGameControlsModel();
        }
    }
}
