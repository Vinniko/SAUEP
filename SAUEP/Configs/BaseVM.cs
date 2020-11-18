using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Visual_Novel_Engine.Config
{
    [Serializable]
    public class BaseVM : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
