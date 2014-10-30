using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Veloose.Model;

namespace Veloose.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Interfaces

        private INavigationService _navigationService;
        public ICommand SearchingCommand { get; set; }
        public ICommand NavigateToStationCommand { get; set; }

        #endregion

        #region Properties

        #region HelloWorld

        private string _helloWorld;

        public string HelloWorld
        {
            get { return _helloWorld; }
            set { Set(() => HelloWorld, ref _helloWorld, value); }
        }

        #endregion

        #region SearchingText

        /// <summary>
        /// The <see cref="SearchingText" /> property's name.
        /// </summary>
        public const string SearchingTextPropertyName = "SearchingText";

        private string _searchingText = "";

        /// <summary>
        /// Sets and gets the SearchingText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SearchingText
        {
            get { return _searchingText; }

            set
            {
                if (_searchingText == value)
                {
                    return;
                }
                _searchingText = value;
                if(!string.IsNullOrEmpty(_searchingText))
                    SearchingAction(_searchingText);
                RaisePropertyChanged(SearchingTextPropertyName);
            }
        }

        #endregion

        #region Stations

        /// <summary>
        /// The <see cref="stations" /> property's name.
        /// </summary>
        public const string stationsPropertyName = "stations";

        private ObservableCollection<Feature> _stations = new ObservableCollection<Feature>();

        /// <summary>
        /// Sets and gets the stations property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Feature> stations
        {
            get { return _stations; }

            set
            {
                if (_stations == value)
                {
                    return;
                }

                _stations = value;
                RaisePropertyChanged(stationsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="StationContainer" /> property's name.
        /// </summary>
        public const string StationContainerPropertyName = "StationContainer";

        private RootObject _stationContaineRootObject = new RootObject();

        public static readonly DependencyProperty NavigateToStationProperty =
            DependencyProperty.Register("NavigateToStation", typeof (object), typeof (MainViewModel),
                new PropertyMetadata(default(object)));

        /// <summary>
        /// Sets and gets the StationContainer property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public RootObject StationContainer
        {
            get { return _stationContaineRootObject; }

            set
            {
                if (_stationContaineRootObject == value)
                {
                    return;
                }

//                RaisePropertyChanging(StationContainerPropertyName);
                _stationContaineRootObject = value;
                RaisePropertyChanged(StationContainerPropertyName);
            }
        }

        #endregion

        #region CurrentStation

        /// <summary>
        /// The <see cref="CurrentStation" /> property's name.
        /// </summary>
        public const string CurrentStationPropertyName = "CurrentStation";

        private Feature _currentStation = new Feature();

        /// <summary>
        /// Sets and gets the CurrentStation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Feature CurrentStation
        {
            get { return _currentStation; }

            set
            {
                if (_currentStation == value)
                {
                    return;
                }

                _currentStation = value;
                RaisePropertyChanged(CurrentStationPropertyName);
            }
        }

        #endregion

        #region UserPosition

        /// <summary>
        /// The <see cref="UserPosition" /> property's name.
        /// </summary>
        public const string UserPositionPropertyName = "UserPosition";

        private Geoposition _userPosition = new Geoposition();

        /// <summary>
        /// Sets and gets the UserPosition property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Geoposition UserPosition
        {
            get { return _userPosition; }

            set
            {
                if (_userPosition == value)
                {
                    return;
                }

                _userPosition = value;
                RaisePropertyChanged(UserPositionPropertyName);
            }
        }

        #endregion

        #endregion

        public async void SetMap()
        {
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            UserPosition =  await locator.GetGeopositionAsync();
            
        }

        public async void LoadJson()
        {
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("Data");
            StorageFile file = await folder.GetFileAsync("Velo_Toulouse.json");
            string rtfString = await FileIO.ReadTextAsync(file);
            StationContainer = JsonConvert.DeserializeObject<RootObject>(rtfString);
            stations  = new ObservableCollection<Feature>( StationContainer.features.OrderBy(f => f.properties.num_station).ToList());
        }

//        private StorageFolder GetFolder(string name)
//        {
//            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
//            return folder.GetFolderAsync(name).GetResults();
//        }
//
//        private StorageFile GetFile(StorageFolder folder, string fileName)
//        {
//            return folder.GetFileAsync(fileName).GetResults();
//        }

        
        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToStationCommand = new RelayCommand(() => _navigationService.NavigateTo(Constants.DetailsPageName));
            SearchingCommand = new RelayCommand<string>(SearchingAction);
//            StationContainer = new RootObject{ features = new List<Feature>{
            CurrentStation = new Feature
            {
                properties = new Properties2 { nom = "Rue Herbette kjljlj lkjlkjlkj kj", commune = "toulouse", num_station = 1, nb_bornettes = 18, street = "rue du test", No = "3" }} ;
    
//            }    }         };
            LoadJson();
//            _currentStation = IsInDesignMode ? stations.FirstOrDefault() : null;
            HelloWorld = IsInDesignMode
                ? "Runs in design mode"
                : "Runs in runtime mode";
        }

        private void SearchingAction(string obj)
        {
            stations = new ObservableCollection<Feature>(StationContainer.features.Where(n =>
                            n.properties.nom.ToLower().Contains(SearchingText.ToLower()) || n.properties.street.ToLower().Contains(SearchingText.ToLower()) ||
                            n.properties.num_station.ToString().Contains(SearchingText)));
        }
    }
}
