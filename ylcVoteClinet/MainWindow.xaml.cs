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
using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ylcVoteClinet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class ChoicesItem
    {
        public string ChoicesText { get; set; }
    }

    public partial class MainWindow : Window
    {

        private ObservableCollection<ChoicesItem> ChoicesItems { get; set; } = new ObservableCollection<ChoicesItem>();
        private string latestChaoiceText;


        public MainWindow()
        {
            InitializeComponent();
            choicesDataGrid.DataContext = ChoicesItems;
            durationLabel.Content = durationSlider.Value.ToString() + "分";

        }

        private void updateChoicesView()
        {
          
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            if (choicesTextBox.Text == null || choicesTextBox.Text == "") {
                return;
            }
            ChoicesItems.Add(new ChoicesItem()
            {
                ChoicesText = choicesTextBox.Text,
            });
            choicesTextBox.Text = "";
            updateChoicesView();
        }

        private void VoteStartClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void VoteEndClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void ExtendDurationClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void GetResultClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }


        private void DurationChanged(object sender, RoutedEventArgs e)
        {
            durationLabel.Content = durationSlider.Value.ToString() + "分";
        }


        private void ChoicesPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            latestChaoiceText = ((TextBox)e.EditingElement).Text;
        }


        private void ChoicesCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string v = ((TextBox)e.EditingElement).Text;
            if (v == null || v == "")
            {
                e.Cancel = true;
                ((TextBox)e.EditingElement).Text = latestChaoiceText;
                return;
            }
            updateChoicesView();
        }

        private void ChoicesDataGridRemove(object sender, RoutedEventArgs e)
        {
            if (choicesDataGrid.SelectedIndex == -1)
            {
                return;
            } 
            ChoicesItems.Remove(ChoicesItems[choicesDataGrid.SelectedIndex]);
            choicesDataGrid.SelectedIndex = -1;
        }
    }
}
