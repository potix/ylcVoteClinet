using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ylccProtocol;
using System.Windows;

namespace ylcVoteClinet
{

    public class TargetValue
    {
        public string Label { get; set; }
        public Target Target { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }

    public class Result
    {
        public int Count { get; set; }

        public double Rate { get; set; }
    }

    public class Setting
    {
        private readonly YlccProtocol protocol = new YlccProtocol();

        public string VideoId { get; set; }
        public string VoteId { get; set; }

        public ObservableCollection<Choice> Choices { get; set; }
        
        public TargetValue TargetValue { get; set; }
        
        public ObservableCollection<TargetValue> TargetValues { get; set; }

        public int Duration { get; set; }

        public string Uri { get; set; }

        public bool IsInsecure { get; set; }


        public string WindowBackgroundColor { get; set; }
        
        public string BoxForegroundColor { get; set; }
        
        public string BoxBackgroundColor { get; set; }
        
        public string BoxBorderColor { get; set; }

        public int FontSize { get; set; }

        public int Padding { get; set; }

        public int Total { get; set; }

        public ObservableCollection<Result> Results { get; set; }

        public Setting()
        {
            Choices = new ObservableCollection<Choice>();
            TargetValues = new ObservableCollection<TargetValue>();
            TargetValue defaultTargetValue = new TargetValue { Label = "all user", Target = Target.AllUser };
            TargetValues.Add(defaultTargetValue);
            TargetValues.Add(new TargetValue { Label = "owner and moderator and sponsor", Target = Target.OwnerModeratorSponsor });
            TargetValues.Add(new TargetValue { Label = "owner and moderator", Target = Target.OwnerModerator });
            TargetValue = defaultTargetValue;
            Duration = 5;
            Uri = "http://127.0.0.1:12345";
            IsInsecure = true;
            Total = 0;
            WindowBackgroundColor = "#000000";
            BoxForegroundColor = "#FFFFFF";
            BoxBackgroundColor = "#4169E1";
            BoxBorderColor = "#000080";
            Results = null;
            FontSize = 20;
            Padding = 32;
        }

        public bool UpdateResults(int total, ICollection<VoteCount> counts)
        {
            if (total == 0)
            {
                return false;
            }
            Total = total;
            Results = new ObservableCollection<Result>();
            foreach (VoteCount count in counts)
            {
                Results.Add(new Result() { Count = count.Count, Rate = Math.Ceiling((double)count.Count * 100.0 / (double)total * 10) / 10 });
            }
            return true;
        }

        public void Dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            Debug.Print(sb.ToString());
        }
    }
}
