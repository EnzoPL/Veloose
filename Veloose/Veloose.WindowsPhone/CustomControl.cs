using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Pour en savoir plus sur le modèle d'élément Contrôle basé sur un modèle, consultez la page http://go.microsoft.com/fwlink/?LinkId=234235

namespace Veloose
{
    public sealed class CustomControl : Control
    {
        public CustomControl()
        {
            this.DefaultStyleKey = typeof(CustomControl);
        }
    }
}
