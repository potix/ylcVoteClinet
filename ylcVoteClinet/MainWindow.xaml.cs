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
        private bool opened;

        public MainWindow()
        {
            InitializeComponent();
            opened = false;
            VideoIdTextBox.DataContext = setting;
            ChoicesDataGrid.DataContext = setting;
            TargetComboBox.DataContext = setting;
            DurationSlider.DataContext = setting;
            DurationLabel.DataContext = setting;
            URITextBox.DataContext = setting;
            InsecureCheckBox.DataContext = setting;
        }

        private void AddChoiceButtonClick(object sender, RoutedEventArgs e)
        {
            if (ChoicesTextBox.Text == null || ChoicesTextBox.Text == "")
            {
                return;
            }
            setting.ChoiceItems.Add(new ChoiceItem() { Choice = ChoicesTextBox.Text });
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


        private async void OpenVote()
        {
            if (opened)
            {
                return;
            }
            if (setting.VideoId == null || setting.VideoId == "")
            {
                return;
            }
            GrpcChannel channel = GrpcChannel.ForAddress(setting.URI);
            ylcc.ylccClient client = new ylcc.ylccClient(channel);
            YlccProtocol protocol = new YlccProtocol();
            Collection<VoteChoice> choices = new Collection<VoteChoice>();
            foreach (var choiceItem in setting.ChoiceItems) {
                choices.Add(new VoteChoice() { Label = (choices.Count + 1).ToString(), Choice = choiceItem.Choice });
            }
            OpenVoteRequest openVoteRequest = protocol.BuildOpenVoteRequest(setting.VideoId, setting.TargetValue.Target, setting.Duration, choices);
            OpenVoteResponse openVoteResponse = await client.OpenVoteAsync(openVoteRequest);
            if (openVoteResponse.Status.Code != Code.Success)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + setting.URI + "\n");
                sb.Append("VideoId:" + setting.VideoId + "\n");
                sb.Append("Reason:" + openVoteResponse.Status.Message + "\n");
                MessageBox.Show(sb.ToString());
                this.Close();
                return;
            }
            setting.VoteId = openVoteResponse.VoteId;
            opened = true;
            ViewWindow viewWindow = new ViewWindow(setting);
            viewWindow.Closed += ViewWindowCloseEventHandler;
            viewWindow.Show();
        }

        private void OpenVoteClick(object sender, RoutedEventArgs e)
        {
            OpenVote();
        }

        private async void UpdateVoteDurationClick()
        {
            if (!opened)
            {
                return;
            }
            if (setting.VoteId == null || setting.VoteId == "")
            {
                return;
            }
            GrpcChannel channel = GrpcChannel.ForAddress(setting.URI);
            ylcc.ylccClient client = new ylcc.ylccClient(channel);
            YlccProtocol protocol = new YlccProtocol();
            UpdateVoteDurationRequest updateVoteDurationRequest = protocol.BuildUpdateVoteDurationRequest(setting.VoteId, setting.Duration);
            UpdateVoteDurationResponse updateVoteDurationResponse = await client.UpdateVoteDurationAsync(updateVoteDurationRequest);
            if (updateVoteDurationResponse.Status.Code != Code.Success)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + setting.URI + "\n");
                sb.Append("VideoId:" + setting.VideoId + "\n");
                sb.Append("Reason:" + updateVoteDurationResponse.Status.Message + "\n");
                MessageBox.Show(sb.ToString());
                this.Close();
                return;
            }
        }

        private void UpdateVoteDurationClick(object sender, RoutedEventArgs e)
        {
            UpdateVoteDurationClick();

        }

        private async void GetVoteResult()
        {
            if (!opened)
            {
                return;
            }
            if (setting.VoteId == null || setting.VoteId == "")
            {
                return;
            }
            GrpcChannel channel = GrpcChannel.ForAddress(setting.URI);
            ylcc.ylccClient client = new ylcc.ylccClient(channel);
            YlccProtocol protocol = new YlccProtocol();
            GetVoteResultRequest getVoteResultRequest = protocol.BuildGetVoteResultRequest(setting.VoteId);
            GetVoteResultResponse getVoteResultResponse = await client.GetVoteResultAsync(getVoteResultRequest);
            if (getVoteResultResponse.Status.Code != Code.Success)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + setting.URI + "\n");
                sb.Append("VideoId:" + setting.VideoId + "\n");
                sb.Append("Reason:" + getVoteResultResponse.Status.Message + "\n");
                MessageBox.Show(sb.ToString());
                this.Close();
                return;
            }
            setting.Total = getVoteResultResponse.Total;
            setting.Counts = getVoteResultResponse.Counts;

        }

        private void GetVoteResultClick(object sender, RoutedEventArgs e)
        {
            GetVoteResult();
        }

        private async void CloseVote()
        {
            if (!opened)
            {
                return;
            }
            if (setting.VoteId == null || setting.VoteId == "")
            {
                return;
            }
            if (setting.IsInsecure)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            GrpcChannel channel = GrpcChannel.ForAddress(setting.URI);
            ylcc.ylccClient client = new ylcc.ylccClient(channel);
            YlccProtocol protocol = new YlccProtocol();
            CloseVoteRequest closeVoteRequest = protocol.BuildCloseVoteRequest(setting.VoteId);
            CloseVoteResponse closeVoteResponse = await client.CloseVoteAsync(closeVoteRequest);
            if (closeVoteResponse.Status.Code != Code.Success)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("通信エラー\n");
                sb.Append("URI:" + setting.URI + "\n");
                sb.Append("VideoId:" + setting.VideoId + "\n");
                sb.Append("VoteId:" + setting.VideoId + "\n");
                sb.Append("Reason:" + closeVoteResponse.Status.Message + "\n");
                MessageBox.Show(sb.ToString());
                this.Close();
            }
            opened = false;
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
