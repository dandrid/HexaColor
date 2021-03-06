﻿using System;
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
    /// Interaction logic for EmptyMapLayoutView.xaml
    /// </summary>
    public partial class EmptyMapLayoutView : UserControl
    {
        private object LeftPanelDataContext
        {
            set
            {
                (Application.Current.MainWindow.FindName("LeftPanel") as ContentControl).DataContext = value;
            }
        }

        public EmptyMapLayoutView()
        {
            InitializeComponent();
        }
    }
}
