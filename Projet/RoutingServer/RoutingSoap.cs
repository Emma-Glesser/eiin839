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
            return rest.GetStations();
        }

        public Task<Station> GetClosestStationSoap(double lat, double lon)
        {
            return rest.GetClosestStation(lat, lon);
        }

        public Task<GeoCoordinate> GetCoordinateFromAddressSoap(string address)
        {
            return rest.GetCoordinateFromAddress(address);
        }

        public Task<Path[]> GetPathSoap(string origin, string destination)
        {
            return rest.GetPath(origin, destination);
        }
    }
}
