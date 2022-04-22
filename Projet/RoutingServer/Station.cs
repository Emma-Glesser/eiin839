using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace RoutingServer
{
    public class Station
    {
        public int number { get; set; }
        public string contractName { get; set; }
        public Position position { get; set; }
        public TotalStands totalStands { get; set; }
    }
    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }

        public GeoCoordinate getGeoCoordinate()
        {
            return new GeoCoordinate(latitude, longitude);
        }
    }
    public class TotalStands
    {
        public Availabilities availabilities { get; set; }
        public int capacity { get; set; }
    }
    public class Availabilities
    {
        public int bikes { get; set; }

    }

}
