using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Veloose.Model
{
    
    public class Properties
    {
        public string name { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Properties2
    {
        public string nom { get; set; }
        public int num_station { get; set; }
        public int nb_bornettes { get; set; }
        public string En_service { get; set; }
        public string M_en_S_16_nov_07 { get; set; }
        public string street { get; set; }
        public string Mot_Directeur { get; set; }
        public string commune { get; set; }
        public string code_insee { get; set; }
        public double X_CC43 { get; set; }
        public double Y_CC43 { get; set; }
        public double X_WGS84 { get; set; }
        public double Y_WGS84 { get; set; }
        public string Nrivoli { get; set; }
        public string No { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties2 properties { get; set; }
    }

    public class RootObject
    {
        public string name { get; set; }
        public string type { get; set; }
        public Crs crs { get; set; }
        public ObservableCollection<Feature> features { get; set; }
    }
}
