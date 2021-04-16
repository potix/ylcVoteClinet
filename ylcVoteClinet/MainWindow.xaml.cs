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
        private readonly Setting _setting = new Setting();
        private ViewWindow _viewWindow;
        private readonly int _minutes = 60;

        public MainWindow()
        {
            InitializeComponent();
            _viewWindow = null;
            VideoIdTextBox.DataContext = _setting;
            ChoicesDataGrid.DataContext = _setting;
            TargetComboBox.DataContext = _setting;
            DurationSlider.DataContext = _setting;
            DurationLabel.DataContext = _setting;
            URITextBox.DataContext = _setting;
            InsecureCheckBox.DataContext = _setting;
            WindowBackgroundColorTextBox.DataContext = _setting;
            WindowBackgroundColorBorder.DataContext = _setting;
            BoxForegroundColorTextBox.DataContext = _setting;
            BoxForegroundColorBorder.DataContext = _setting;
            BoxBackgroundColorTextBox.DataContext = _setting;
            BoxBackgroundColorBorder.DataContext = _setting;
            BoxBorderColorTextBox.DataContext = _setting;
            BoxBorderColorBorder.DataContext = _setting;
            FontSizeTextBox.DataContext = _setting;
            PaddingTextBox.DataContext = _setting;
        }

        private void AddChoiceButtonClick(object sender, RoutedEventArgs e)
        {
            if (ChoicesTextBox.Text == null || ChoicesTextBox.Text == "")
            {
                return;
            }
            _setting.Choices.Add(new Choice() { Text = ChoicesTextBox.Text });
        }

        private void ChoiceRemove(object sender, RoutedEventArgs e)
        {
            if (ChoicesDataGrid.SelectedIndex == -1)
            {
                return;
            }
            _setting.Choices.Remove(_setting.Choices[ChoicesDataGrid.SelectedIndex]);
            ChoicesDataGrid.SelectedIndex = -1;
        }


        private async void OpenVote()
        {
            if (_viewWindow != null)
            {
                return;
            }
            if (_setting.VideoId == null || _setting.VideoId == "")
            {
                return;
            }
            if (_setting.Choices.Count == 0)
            {
                return;
            }

            try
            {
                if (_setting.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_setting.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                YlccProtocol protocol = new YlccProtocol();
                Collection<VoteChoice> choices = new Collection<VoteChoice>();
                foreach (var choiceItem in _setting.Choices) {
                    int idx = _setting.Choices.IndexOf(choiceItem);
                    choices.Add(new VoteChoice() { Label = (idx + 1).ToString(), Choice = choiceItem.Text });
                }
                OpenVoteRequest openVoteRequest = protocol.BuildOpenVoteRequest(_setting.VideoId, _setting.TargetValue.Target, _setting.Duration * _minutes, choices);
                OpenVoteResponse openVoteResponse = await client.OpenVoteAsync(openVoteRequest);
                if (openVoteResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _setting.Uri + "\n");
                    sb.Append("VideoId:" + _setting.VideoId + "\n");
                    sb.Append("Reason:" + openVoteResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    return;
                }
                _setting.VoteId = openVoteResponse.VoteId;
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("エラー\n");
                sb.Append("URI:" + _setting.Uri + "\n");
                sb.Append("VideoId:" + _setting.VideoId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
                return;
            }
            ViewWindow viewWindow = new ViewWindow(_setting);
            viewWindow.Closed += ViewWindowCloseEventHandler;
            viewWindow.Show();
            viewWindow.Render(_setting);
            _viewWindow = viewWindow;
        }

        private void OpenVoteClick(object sender, RoutedEventArgs e)
        {
            OpenVote();

        }

        private async void UpdateVoteDurationClick()
        {
            if (_viewWindow == null)
            {
                return;
            }
            if (_setting.VoteId == null || _setting.VoteId == "")
            {
                return;
            }

            try
            {
                if (_setting.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_setting.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                YlccProtocol protocol = new YlccProtocol();
                UpdateVoteDurationRequest updateVoteDurationRequest = protocol.BuildUpdateVoteDurationRequest(_setting.VoteId, _setting.Duration * _minutes);
                UpdateVoteDurationResponse updateVoteDurationResponse = await client.UpdateVoteDurationAsync(updateVoteDurationRequest);
                if (updateVoteDurationResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _setting.Uri + "\n");
                    sb.Append("VideoId:" + _setting.VideoId + "\n");
                    sb.Append("Reason:" + updateVoteDurationResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    return;
                }
                MessageBox.Show("投票時間を延長しました");
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("エラー\n");
                sb.Append("URI:" + _setting.Uri + "\n");
                sb.Append("VideoId:" + _setting.VideoId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
            }
        }

        private void UpdateVoteDurationClick(object sender, RoutedEventArgs e)
        {
            UpdateVoteDurationClick();
 
        }

        private async void GetVoteResult()
        {
            if (_viewWindow == null)
            {
                return;
            }
            if (_setting.VoteId == null || _setting.VoteId == "")
            {
                return;
            }
            try
            {
                if (_setting.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_setting.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                YlccProtocol protocol = new YlccProtocol();
                GetVoteResultRequest getVoteResultRequest = protocol.BuildGetVoteResultRequest(_setting.VoteId);
                GetVoteResultResponse getVoteResultResponse = await client.GetVoteResultAsync(getVoteResultRequest);
                if (getVoteResultResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _setting.Uri + "\n");
                    sb.Append("VideoId:" + _setting.VideoId + "\n");
                    sb.Append("Reason:" + getVoteResultResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                    return;
                }
                bool ok = _setting.UpdateResults(getVoteResultResponse.Total, getVoteResultResponse.Counts);
                if (ok)
                {
                    _viewWindow.Render(_setting);
                }
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("エラー\n");
                sb.Append("URI:" + _setting.Uri + "\n");
                sb.Append("VideoId:" + _setting.VideoId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
            }
        }

        private void GetVoteResultClick(object sender, RoutedEventArgs e)
        {
            GetVoteResult();
        }

        private async void CloseVote()
        {
            if (_viewWindow == null)
            {
                return;
            }
            if (_setting.VoteId == null || _setting.VoteId == "")
            {
                return;
            }
            try
            {
                if (_setting.IsInsecure)
                {
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                }
                GrpcChannel channel = GrpcChannel.ForAddress(_setting.Uri);
                ylcc.ylccClient client = new ylcc.ylccClient(channel);
                YlccProtocol protocol = new YlccProtocol();
                CloseVoteRequest closeVoteRequest = protocol.BuildCloseVoteRequest(_setting.VoteId);
                CloseVoteResponse closeVoteResponse = await client.CloseVoteAsync(closeVoteRequest);
                if (closeVoteResponse.Status.Code != Code.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("通信エラー\n");
                    sb.Append("URI:" + _setting.Uri + "\n");
                    sb.Append("VideoId:" + _setting.VideoId + "\n");
                    sb.Append("VoteId:" + _setting.VideoId + "\n");
                    sb.Append("Reason:" + closeVoteResponse.Status.Message + "\n");
                    MessageBox.Show(sb.ToString());
                }
            }
            catch (Exception err)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("エラー\n");
                sb.Append("URI:" + _setting.Uri + "\n");
                sb.Append("VideoId:" + _setting.VideoId + "\n");
                sb.Append("Reason:" + err.Message + "\n");
                MessageBox.Show(sb.ToString());
            }
            MessageBox.Show("投票を終了しました");
            _viewWindow = null;
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
