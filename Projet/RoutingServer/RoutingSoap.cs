using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;

namespace RoutingServer
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class RoutingSoap : IRoutingSoap
    {
        RoutingRest rest = new RoutingRest();
        public Task<bool> GetAvailabilitySoap(string station, string contract)
        {
            return rest.GetAvailability(station, contract);
        }

        public Task<List<Station>> GetStationsSoap()
        {
            try
            {
                return rest.GetStations();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            } 
        }

        public Task<Station> GetClosestStationSoap(double lat, double lon)
        {
            try
            {
                return rest.GetClosestStation(lat, lon);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public Task<GeoCoordinate> GetCoordinateFromAddressSoap(string address)
        {
            try
            {
                return rest.GetCoordinateFromAddress(address);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public Task<Path[]> GetPathSoap(string origin, string destination)
        {
            try
            {
                return rest.GetPath(origin, destination);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
