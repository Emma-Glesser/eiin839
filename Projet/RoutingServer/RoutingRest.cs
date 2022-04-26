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
        static SortedDictionary<double, Station> firstStationsFrom = new SortedDictionary<double, Station>();
        static SortedDictionary<double, Station> firstStationsTo = new SortedDictionary<double, Station>();
        private const string APIKEYOPENRS = "5b3ce3597851110001cf624822905f9a7b6846959c276a255a3f31c8";
        static ServerProxyClient cache = new ServerProxyClient();
        private static HttpClient client = new HttpClient();

        /* Get if there is an available bike into the station */
        public async Task<bool> GetAvailability(string station, string contract)
        {
            Console.WriteLine("Get availibility from proxy");
            Station s = JsonSerializer.Deserialize<Station>(await cache.GetAvailabilitiesAsync(station, contract));
            return await checkDisponibility(s);
        }

        /* Retrieve all stations and fill the stations variables */
        public async Task<List<Station>> GetStations()
        {
            Console.WriteLine("Retrieve all stations");
            string response = await cache.GetStationsAsync();
            stations = JsonSerializer.Deserialize<List<Station>>(response);
            closestStationsFrom.AddRange(stations);
            closestStationsTo.AddRange(stations);
            return stations;
        }

        /* Transform an address into a gps position */
        public async Task<GeoCoordinate> GetCoordinateFromAddress(string address)
        {
            Console.WriteLine("Find gps position from address");
            Location location = null;
            try
            {
                string request = "https://api.openrouteservice.org/geocode/search?api_key=" + APIKEYOPENRS + "&text=" + address;
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                location = JsonSerializer.Deserialize<Location>(responseBody);
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

        /* Get the closest station from the gps position */
        public async Task<Station> GetClosestStation(double lat, double lon)
        {
            Console.WriteLine("Find closest station from gps position");
            GeoCoordinate position = new GeoCoordinate(lat, lon);
            if (stations.Count == 0)
            {
                await GetStations();
            }
            closestStationsFrom.Sort(new StationComparer(position));
            return closestStationsFrom[0];
        }

        /* Get the path from the start to the destination */
        public async Task<Path[]> GetPath(string origin, string destination)
        {
            try
            {
                Console.WriteLine("Find path");
                GeoCoordinate from = await GetCoordinateFromAddress(origin);
                GeoCoordinate to = await GetCoordinateFromAddress(destination);
                await ComputeClosestStation(from, to);
                Station stationFrom = await getStationFrom();
                Station stationTo = await getStationToAsync();
                GeoCoordinate stationFromP = stationFrom.position.getGeoCoordinate();
                GeoCoordinate stationToP = stationTo.position.getGeoCoordinate();
                Path[] paths = await GetPaths(from, to, stationFromP, stationToP);
                if (MustWalk(paths, true))
                {
                    return new Path[] { paths[3] };
                }
                else
                {
                    return new Path[] { paths[0], paths[1], paths[2] };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
                return null;
            }
      
        }

        /* Sort the stations by their distance from start and finish */
        public async Task ComputeClosestStation(GeoCoordinate from, GeoCoordinate to)
        {
            Console.WriteLine("Sort station by distance from start and finish");
            if (stations.Count == 0)
            {
                await GetStations();
            }
            closestStationsFrom.Sort(new StationComparer(from));
            closestStationsTo.Sort(new StationComparer(to));
            firstStationsFrom.Clear();
            firstStationsTo.Clear();
            Path path = null;
            for (int i=0; i<3; i++)
            {
                try
                {
                    string request = this.GetORSrequest(from, closestStationsTo[i].position.getGeoCoordinate(), false);
                    HttpResponseMessage response = await client.GetAsync(request);
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    path = JsonSerializer.Deserialize<Path>(responseBody);
                    firstStationsFrom.Add(path.GetDistance(), closestStationsFrom[i]);

                    request = GetORSrequest(closestStationsTo[i].position.getGeoCoordinate(), to, true);
                    response = await client.GetAsync(request);
                    response.EnsureSuccessStatusCode();
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    path = JsonSerializer.Deserialize<Path>(responseBody);
                    firstStationsTo.Add(path.GetDistance(), closestStationsTo[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }

        /* Find the closest station from the start with an available bike */
        public async Task<Station> getStationFrom()
        {
            Console.WriteLine("Find closest station from start");
            foreach (Station station in firstStationsFrom.Values)
            {
                if (await checkDisponibility(station))
                {
                    return station;
                }
            }
            Station closest = null;
            int i = 3;
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

        /* Find the closest station from the destination with a free place for a bike */
        public async Task<Station> getStationToAsync()
        {
            Console.WriteLine("Find closest station from destination");
            foreach (Station station in firstStationsTo.Values)
            {
                if (await checkDisponibility(station))
                {
                    return station;
                }
            }
            Station closest = null;
            int i = 3;
            while (closest == null)
            {
                if (await checkPlace(closestStationsTo[i]))
                {
                    closest = closestStationsTo[i];
                }
                i++;
            }
            return closest;
        }

        /* Check if there is an available bike in the station */
        public async Task<bool> checkDisponibility(Station station)
        {
            Console.WriteLine("Check if there is a free bike to take");
            string response = await cache.GetAvailabilitiesAsync(station.number.ToString(), station.contractName);
            station = JsonSerializer.Deserialize<Station>(response);
            return station.totalStands.availabilities.bikes > 0;
        }

        /* Check if there is a free place to leave the bike in the station */
        public async Task<bool> checkPlace(Station station)
        {
            Console.WriteLine("Check if there is a free place to leave the bike");
            string response = await cache.GetAvailabilitiesAsync(station.number.ToString(), station.contractName);
            station = JsonSerializer.Deserialize<Station>(response);
            return station.totalStands.availabilities.bikes < station.totalStands.capacity;
        }

        /* Create the request to call open route service to get the path from A to B using the bike or not */
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

        /* Check if it is faster to walk than to walk, take the bike and walk again by checking the different paths.
         * It compares according to distance or to the time taken*/
        private bool MustWalk(Path[] paths, bool time)
        {
            Console.WriteLine("Check if the user must walk");
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

        /*Find paths from A to BikeStation A, from BikeStationA to BikeStationB, from BikeStationB to B and from A to B 
         * by using open route service*/
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

    /*Comparater to sort Station by distance to a gps position */
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