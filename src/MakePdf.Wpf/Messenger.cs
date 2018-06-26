using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Events;

namespace MakePdf.Wpf
{
    public enum MessengerType
    {
        NewVersionFound,
        Processing,
        UpdateRecentFiles,
        ReadSettingFile,
    }

    class Messenger
    {
        public static Messenger Instance { get; } = new Messenger();
        readonly List<EventAggregator> messengers;

        Messenger()
        {
            messengers = new List<EventAggregator>();
            for (int i = 0; i < Enum.GetNames(typeof(MessengerType)).Length; i++)
            {
                messengers.Add(new EventAggregator());
            }
        }

        public  EventAggregator this[MessengerType type]
        {
            get => messengers[(int)type];
        }
    }
}
