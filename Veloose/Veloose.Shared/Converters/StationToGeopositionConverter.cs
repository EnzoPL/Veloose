using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;
using Veloose.Model;

namespace Veloose.Converters
{
    class StationToGeopositionConverter : IValueConverter
    {
//        public Feature Station { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var station = value as Feature;
            return new Geopoint(new BasicGeoposition { Longitude = station.properties.X_WGS84, Latitude = station.properties.Y_WGS84 });
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
