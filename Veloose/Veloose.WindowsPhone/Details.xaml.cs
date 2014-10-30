using Veloose.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrat de partage cible, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace Veloose
{
    /// <summary>
    /// Cette page permet aux autres applications de partager leur contenu via cette application.
    /// </summary>
    public sealed partial class Details : Page
    {
        /// <summary>
        /// Fournit un canal permettant de communiquer avec Windows au sujet de l'opération de partage.
        /// </summary>
        private ShareOperation shareOperation;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Details()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Obtient le modèle d'affichage pour ce <see cref="Page"/>.
        /// Cela peut être remplacé par un modèle d'affichage fortement typé.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Invoqué lorsqu'une autre application souhaite partager du contenu via cette application.
        /// </summary>
        /// <param name="e">Données d'activation utilisées pour coordonner le processus avec Windows.</param>
        public async void Activate(ShareTargetActivatedEventArgs e)
        {
            this.shareOperation = e.ShareOperation;

            // Communiquer des métadonnées concernant le contenu partagé via le modèle d'affichage
            var shareProperties = this.shareOperation.Data.Properties;
            var thumbnailImage = new BitmapImage();
            this.DefaultViewModel["Title"] = shareProperties.Title;
            this.DefaultViewModel["Description"] = shareProperties.Description;
            this.DefaultViewModel["Image"] = thumbnailImage;
            this.DefaultViewModel["Sharing"] = false;
            this.DefaultViewModel["ShowImage"] = false;
            this.DefaultViewModel["Comment"] = string.Empty;
            this.DefaultViewModel["Placeholder"] = "Add a comment";
            this.DefaultViewModel["SupportsComment"] = true;
            Window.Current.Content = this;
            Window.Current.Activate();

            // Mettez à jour l'image miniature du contenu partagé dans l'arrière-plan
            if (shareProperties.Thumbnail != null)
            {
                var stream = await shareProperties.Thumbnail.OpenReadAsync();
                thumbnailImage.SetSource(stream);
                this.DefaultViewModel["ShowImage"] = true;
            }
        }

        /// <summary>
        /// Invoqué lorsque l'utilisateur clique sur le bouton Partager.
        /// </summary>
        /// <param name="sender">Instance Button utilisée pour démarrer le partage.</param>
        /// <param name="e">Données d'événement décrivant la façon dont l'utilisateur a cliqué sur le bouton.</param>
        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["Sharing"] = true;

            // TODO: effectuez un travail convenant au scénario de partage à l'aide de
            //       this._shareOperation.Data, généralement en ajoutant des informations capturées
            //       à l'aide d'éléments d'interface utilisateur ajoutés à cette page, par exemple 
            //       this.DefaultViewModel["Comment"]

            this.shareOperation.ReportCompleted();
        }
    }
}
