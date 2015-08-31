using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ICResponse
{
    public class DailyAgent : INotifyPropertyChanged
    {
        private string _agent = string.Empty;
        private int _count = new Int32();

        public string Agent
        {
            get
            {
                return _agent;
            }
            set
            {
                _agent = value;
                NotifyPropertyChanged("Agent");
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                NotifyPropertyChanged("Count");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
