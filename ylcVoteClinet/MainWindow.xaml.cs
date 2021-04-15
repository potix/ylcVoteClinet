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
using ylccProtocol;
using Grpc.Net.Client;

namespace ylcVoteClinet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        private Setting setting = new Setting();

        public MainWindow()
        {
            InitializeComponent();
            VideoIdTextBox.DataContext = setting;
            ChoicesDataGrid.DataContext = setting;
            TargetComboBox.DataContext = setting;
            DurationSlider.DataContext = setting;
            DurationLabel.DataContext = setting;
        }

        private void AddChoiceButtonClick(object sender, RoutedEventArgs e)
        {
            if (ChoicesTextBox.Text == null || ChoicesTextBox.Text == "")
            {
                return;
            }
            setting.ChoiceItems.Add(new ChoiceItem() { Label = ChoicesTextBox.Text });
        }

        private void ChoiceRemove(object sender, RoutedEventArgs e)
        {
            if (ChoicesDataGrid.SelectedIndex == -1)
            {
                return;
            }
            setting.ChoiceItems.Remove(setting.ChoiceItems[ChoicesDataGrid.SelectedIndex]);
            ChoicesDataGrid.SelectedIndex = -1;
        }

        private void OpenVoteClick(object sender, RoutedEventArgs e)
        {

            ViewWindow viewWindow = new ViewWindow(setting);
            viewWindow.Closed += ViewWindowCloseEventHandler;
            viewWindow.Show();
        }

        private void UpdateVoteDurationClick(object sender, RoutedEventArgs e)
        {

        }

        private void GetVoteResultClick(object sender, RoutedEventArgs e)
        {

        }

        private async void CloseVote()
        {
            if (setting.IsInsecure)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            GrpcChannel channel = GrpcChannel.ForAddress(setting.URI);
            ylcc.ylccClient client = new ylcc.ylccClient(channel);
            YlccProtocol protocol = new YlccProtocol();
            CloseVoteRequest closeVoteRequest = protocol.BuildCloseVoteRequestRequest(setting.VoteId);
            StartCollectionWordCloudMessagesResponse startCollectionWordCloudMessagesResponse = await client.StartCollectionWordCloudMessagesAsync(startCollectionWordCloudMessagesRequest);
            if (startCollectionWordCloudMessagesResponse.Status.Code != Code.Success && startCollectionWordCloudMessagesResponse.Status.Code != Code.InProgress)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + setting.URI + "\n");
                sb.Append("VideoId:" + setting.VideoId + "\n");
                sb.Append("Reason:" + startCollectionWordCloudMessagesResponse.Status.Message + "\n");
                MessageBox.Show(sb.ToString());
                this.Close();
                return;
            }

        }

        private void CloseVoteClick(object sender, RoutedEventArgs e)
        {
            CloseVote();
        }

        private void ViewWindowCloseEventHandler(object sender, EventArgs e)
        {
            CloseVote();
        }

    }
}
