using System;
using System.Globalization;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RoutingServer.ProxyCache;

namespace RoutingServer
{
    public class RoutingRest : IRoutingRest
    {
        static List<Station> stations = new List<Station>();
        static List<Station> closestStationsFrom = new List<Station>();
        static List<Station> closestStationsTo = new List<Station>();
        private const string APIKEYOPENRS = "5b3ce3597851110001cf624822905f9a7b6846959c276a255a3f31c8";
        static ServerProxyClient cache = new ServerProxyClient();
        private static HttpClient client = new HttpClient();
        public async Task<bool> GetAvailability(string station, string contract)
        {
            Station s = JsonSerializer.Deserialize<Station>(await cache.GetAvailabilitiesAsync(station, contract));
            return await checkDisponibility(s);
        }

        public async Task<List<Station>> GetStations()
        {
            string response = await cache.GetStationsAsync();
            Console.WriteLine(response);
            stations = JsonSerializer.Deserialize<List<Station>>(response);
            closestStationsFrom.AddRange(stations);
            closestStationsTo.AddRange(stations);
            return stations;
        }
        public async Task<GeoCoordinate> GetCoordinateFromAddress(string address)
        {
            Location location = null;
            try
            {
                string request = "https://api.openrouteservice.org/geocode/search?api_key=" + APIKEYOPENRS + "&text=" + address;
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                location = JsonSerializer.Deserialize<Location>(responseBody);
                Console.WriteLine(location);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            if (location != null)
            {
                return location.GetCoordinate();
            }
            return new GeoCoordinate();
        }

        public async Task<Station> GetClosestStation(double lat, double lon)
        {
            GeoCoordinate position = new GeoCoordinate(lat, lon);
            if (stations.Count == 0)
            {
                await GetStations();
            }
            closestStationsFrom.Sort(new StationComparer(position));
            return closestStationsFrom[0];
        }

        public async Task<Path[]> GetPath(string origin, string destination)
        {
            GeoCoordinate from = await GetCoordinateFromAddress(origin);
            GeoCoordinate to = await GetCoordinateFromAddress(destination);
            await ComputeClosestStation(from, to);
            Station stationFrom = await getStationFrom();
            Station stationTo = await getStationToAsync();
            GeoCoordinate stationFromP = stationFrom.position.getGeoCoordinate();
            GeoCoordinate stationToP = stationTo.position.getGeoCoordinate();
            Path[] paths = await GetPaths(from, to, stationFromP, stationToP);
            if (MustWalk(paths,true))
            {
                return new Path[] { paths[3] };
            }
            else
            {
                return new Path[] { paths[0] , paths[1] , paths[2] };
            }        
        }

        public async Task ComputeClosestStation(GeoCoordinate from, GeoCoordinate to)
        {
            if (stations.Count == 0)
            {
                await GetStations();
            }
            closestStationsFrom.Sort(new StationComparer(from));
            closestStationsTo.Sort(new StationComparer(to));
        }

        public async Task<Station> getStationFrom()
        {
            Station closest = null;
            int i = 0;
            while (closest == null)
            {
                if (await checkDisponibility(closestStationsFrom[i]))
                {
                    closest = closestStationsFrom[i];
                }
                i++;
            }
            return closest;
        }

        public async Task<Station> getStationToAsync()
        {
            Station closest = null;
            int i = 0;
            while (closest == null)
            {
                if (await checkPlace(closestStationsFrom[i]))
                {
                    closest = closestStationsTo[i];
                }
                i++;
            }
            return closest;
        }

        public async Task<bool> checkDisponibility(Station station)
        {
            string response = await cache.GetAvailabilitiesAsync(station.number.ToString(), station.contractName);
            station = JsonSerializer.Deserialize<Station>(response);
            return station.totalStands.availabilities.bikes > 0;
        }

        public async Task<bool> checkPlace(Station station)
        {
            string response = await cache.GetAvailabilitiesAsync(station.number.ToString(), station.contractName);
            station = JsonSerializer.Deserialize<Station>(response);
            return station.totalStands.availabilities.bikes < station.totalStands.capacity;
        }

        private string GetORSrequest(GeoCoordinate from, GeoCoordinate to,bool bike )
        {
            string start = from.Longitude.ToString(CultureInfo.InvariantCulture) + "," + from.Latitude.ToString(CultureInfo.InvariantCulture);
            string end = to.Longitude.ToString(CultureInfo.InvariantCulture) + "," + to.Latitude.ToString(CultureInfo.InvariantCulture);

            string request = "https://api.openrouteservice.org/v2/directions/";
            if (bike)
            {
                request += "cycling-regular";
            }
            else
            {
                request += "foot-walking";
            }  
                request += "?api_key=" + APIKEYOPENRS + "&start=" + start + "&end=" + end;
            return request;
        }

        private bool MustWalk(Path[] paths, bool time)
        {
            if (time)
            {
                double walkDuration = paths[3].GetDuration();
                double walkBikeDuration = paths[0].GetDuration() + paths[1].GetDuration() + paths[2].GetDuration();

                if (walkDuration <= walkBikeDuration)
                {
                    return true;
                }

                return false;
            }
            else
            {
                double walkDistance = paths[3].GetDistance();
                double walkBikeDistance = paths[0].GetDistance() + paths[1].GetDistance() + paths[2].GetDistance();

                if (walkDistance <= walkBikeDistance)
                {
                    return true;
                }

                return false;
            }

        }

        private async Task<Path[]> GetPaths(GeoCoordinate src, GeoCoordinate dest, GeoCoordinate from, GeoCoordinate to)
        {
            Path path1 = null;
            Path path2 = null;
            Path path3 = null;
            Path path4 = null;
            try
            {
                string request = this.GetORSrequest(src,from,false);
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                path1 = JsonSerializer.Deserialize<Path>(responseBody);

                request = GetORSrequest(from,to,true);
                response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
                path2 = JsonSerializer.Deserialize<Path>(responseBody);

                request = GetORSrequest(to,dest,false);
                response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
                path3 = JsonSerializer.Deserialize<Path>(responseBody);

                request = GetORSrequest(src, dest, false);
                response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
                path4 = JsonSerializer.Deserialize<Path>(responseBody);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return new Path[] { path1, path2, path3, path4 };
        }
    }


    class Location
    {
        public Features[] features { get; set; }

        public double[] GetPosition() {
            return new double[] { this.features[0].geometry.coordinates[1], this.features[0].geometry.coordinates[0] };
        }

        public GeoCoordinate GetCoordinate()
        {
            return new GeoCoordinate(this.features[0].geometry.coordinates[1], this.features[0].geometry.coordinates[0]);
        }
    }

    class Features
    {
        public Geometry geometry { get; set; }
    }

    class Geometry
    {
        //C'est longitude latitude 
        public double[] coordinates { get; set; }
    }

    public class Path
    {
        public PathFeatures[] features { get; set;}

        public double GetDistance() { return this.features[0].properties.segments[0].distance; }

        public double GetDuration() { return this.features[0].properties.segments[0].duration; }
    }

    public class PathFeatures
    {
        public Properties properties { get; set; }

        public PathGeometry geometry { get; set; }
    }

    public class Properties
    {
        public Segment[] segments { get; set; }
    }

    public class Segment
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public Step[] steps { get; set; }
    }

    public class Step
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public string instruction { get; set; }
    }

    public class PathGeometry
    {
        //longitude puis latitude
        public List<double[]> coordinates { get; set; }
    }

    public class StationComparer : Comparer<Station>
    {
        private GeoCoordinate reference;
        public StationComparer(GeoCoordinate position)
        {
            this.reference = position;
        }
        public override int Compare(Station a, Station b)
        {
            double distance1 = reference.GetDistanceTo(a.position.getGeoCoordinate());
            double distance2 = reference.GetDistanceTo(b.position.getGeoCoordinate());
            if (distance1<distance2)
            {
                return -1;
            }
            else if (distance2<distance1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

}