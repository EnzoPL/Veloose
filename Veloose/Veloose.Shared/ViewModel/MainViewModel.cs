using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GalaSoft.MvvmLight;

namespace Veloose.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _helloWorld;

        public string HelloWorld
        {
            get { return _helloWorld; }
            set { Set(() => HelloWorld, ref _helloWorld, value); }
        }
        


        public MainViewModel()
        {
            HelloWorld = IsInDesignMode
                ? "Runs in design mode"
                : "Runs in runtime mode";
        }
    }
}
