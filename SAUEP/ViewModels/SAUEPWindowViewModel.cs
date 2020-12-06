using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;

namespace SAUEP.WPF.ViewModels
{
    public sealed class SAUEPWindowViewModel : BindableBase
    {
        public SAUEPWindowViewModel(IEventAggregator eventAggregator)
        {
            
        }


        public string Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }


        private string _test = "test";
        
    }
}
