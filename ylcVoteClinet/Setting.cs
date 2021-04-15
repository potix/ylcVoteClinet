using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ylccProtocol;

namespace ylcVoteClinet
{

    public class TargetValue
    {
        public string Label { get; set; }
        public Target Target { get; set; }
    }

    public class ChoiceItem
    {
        public string Choice { get; set; }
    }

    public class Setting
    {
        public string VideoId { get; set; }
        public string VoteId { get; set; }

        public ObservableCollection<ChoiceItem> ChoiceItems { get; set; }
        
        public TargetValue TargetValue { get; set; }
        
        public ObservableCollection<TargetValue> TargetValues { get; set; }

        public int Duration { get; set; }

        public string URI { get; set; }

        public bool IsInsecure { get; set; }

        public int Total { get; set; }

        public Collection<VoteCount> Counts { get; set; }

        public Setting()
        {
            ChoiceItems = new ObservableCollection<ChoiceItem>();
            TargetValues = new ObservableCollection<TargetValue>();
            TargetValue defaultTargetValue = new TargetValue { Label = "all user", Target = Target.AllUser };
            TargetValues.Add(defaultTargetValue);
            TargetValues.Add(new TargetValue { Label = "owner and moderator and sponsor", Target = Target.OwnerModeratorSponsor });
            TargetValues.Add(new TargetValue { Label = "owner and moderator", Target = Target.OwnerModerator });
            TargetValue = defaultTargetValue;
            Duration = 5;
            URI = "http://127.0.0.1:12345";
            IsInsecure = true;
            Total = 0;
            Counts = null;
        }

        public void Dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            Debug.Print(sb.ToString());
        }
    }
}
