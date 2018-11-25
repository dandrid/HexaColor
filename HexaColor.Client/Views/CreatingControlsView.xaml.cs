﻿using HexaColor.Client.ViewModels;
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
            set {
                (Window.GetWindow(this).FindName("RightPanel") as ContentControl).DataContext = value;
            }
        }
        private object LeftPanelDataContext
        {
            set
            {
                (Window.GetWindow(this).FindName("LeftPanel") as ContentControl).DataContext = value;
            }
        }

        public CreatingControlsView()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanelDataContext = new StartingControlsModel();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            LeftPanelDataContext = new MapLayoutModel();
        }
    }
}
