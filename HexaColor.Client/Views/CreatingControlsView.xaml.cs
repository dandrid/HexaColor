using HexaColor.Client.Helpers;
using HexaColor.Client.ViewModels;
using HexaColor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanelDataContext = new StartingControlsModel();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            int mapSize = int.Parse((MapSizeBox.SelectedItem as ComboBoxItem).Tag.ToString());
            int mapColorCount = int.Parse((MapColorBox.SelectedItem as ComboBoxItem).Tag.ToString());
            string playerName = PlayerNameBox.Text;
            int playerCount = int.Parse((PlayerCountBox.SelectedItem as ComboBoxItem).Tag.ToString());
            int aiCount = int.Parse((AICountBox.SelectedItem as ComboBoxItem).Tag.ToString());
            Dictionary<string, AiDifficulty> aiDifficulties = null;
            if(aiCount > 0)
            {
                aiDifficulties = new Dictionary<string, AiDifficulty>();
                foreach (var panel in AIPlayersPanel.Children.OfType<DockPanel>())
                {
                    string aiId = panel.Children.OfType<TextBlock>().First().Text;
                    // 1 - easy, 2 - medium, 3 - hard
                    string difficultyNum = panel.Children.OfType<StackPanel>().First().Children.OfType<ToggleButton>().Where(tb => tb.IsChecked.Value).First().Tag.ToString();
                    AiDifficulty aiDifficulty = (AiDifficulty)Enum.Parse(typeof(AiDifficulty), difficultyNum);
                    aiDifficulties.Add(aiId, aiDifficulty);
                }
            }
            // TODO pass each difficulty
            AiDifficulty difficulty = aiDifficulties == null ? AiDifficulty.EASY : aiDifficulties.ElementAt(0).Value;             

            MapLayoutModel mapLayoutModel = new MapLayoutModel(playerName, playerCount, aiCount, difficulty, mapColorCount, mapSize);
            mapLayoutModel.InitMapLayout();
            LeftPanelDataContext = mapLayoutModel;
        }

        private void PlayerCountBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Remove AI players
            ComboBoxItem selected = (sender as ComboBox).SelectedItem as ComboBoxItem;
            int.TryParse(selected.Tag.ToString(), out int playerCount);
            InitAIComboBox(playerCount);
        }

        private void AICountBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selected = (sender as ComboBox).SelectedItem as ComboBoxItem;
            if(selected != null)
            {
                int.TryParse(selected.Tag.ToString(), out int aiPlayerCount);
                InitAIPlayersPanel(aiPlayerCount);
            }
        }

        private void InitAIComboBox(int playerCount)
        {
            if (AICountBox != null)
            {
                AICountBox.Items.Clear();
                for (int i = 0; i < playerCount; i++)
                {
                    ComboBoxItem cbi = new ComboBoxItem
                    {
                        Tag = i,
                        Content = i
                    };
                    AICountBox.Items.Add(cbi);
                }
                AICountBox.SelectedIndex = 0;
            }
        }

        private void InitAIPlayersPanel(int aiPlayerCount)
        {
            if (AIPlayersPanel != null)
            {
                AIPlayersPanel.Children.Clear();
                for (int id = 1; id <= aiPlayerCount; id++)
                {
                    DockPanel dp = new DockPanel();
                    dp.Style = FindResource("PanelStyle") as Style;
                    TextBlock nameTextBlock = new TextBlock();
                    nameTextBlock.Text = "AI_" + id.ToString();
                    StackPanel difficultyPanel = new StackPanel();
                    difficultyPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    difficultyPanel.Orientation = Orientation.Horizontal;
                    ToggleButton easyBtn = new ToggleButton();
                    easyBtn.Tag = "1";
                    easyBtn.Content = "Easy";
                    easyBtn.Width = 40;
                    easyBtn.IsChecked = true;
                    easyBtn.Click += AIDifficultyBtn_Click;
                    ToggleButton mediumBtn = new ToggleButton();
                    mediumBtn.Tag = "2";
                    mediumBtn.Content = "Medium";
                    mediumBtn.Width = 40;
                    mediumBtn.Click += AIDifficultyBtn_Click;
                    ToggleButton hardBtn = new ToggleButton();
                    hardBtn.Tag = "3";
                    hardBtn.Content = "Hard";
                    hardBtn.Width = 40;
                    hardBtn.Click += AIDifficultyBtn_Click;

                    dp.Children.Add(nameTextBlock);
                    dp.Children.Add(difficultyPanel);
                    difficultyPanel.Children.Add(easyBtn);
                    difficultyPanel.Children.Add(mediumBtn);
                    difficultyPanel.Children.Add(hardBtn);

                    AIPlayersPanel.Children.Add(dp);
                }
            }
        }

        private void AIDifficultyBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if(btn.IsChecked.Value)
            {
                var siblings = (btn.Parent as Panel).Children.OfType<ToggleButton>().Where(b => !b.Equals(btn));
                foreach (var sibling in siblings)
                {
                    sibling.IsChecked = false;
                }
            } else
            {
                btn.IsChecked = true;
            }
        }
    }
}
