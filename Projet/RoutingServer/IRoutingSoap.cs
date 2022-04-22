using System.ServiceModel;
using System.Device.Location;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RoutingServer
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IRoutingSoap
    {
        [OperationContract]
        Task<bool> GetAvailabilitySoap(string station, string contract);

        [OperationContract]
        Task<List<Station>> GetStationsSoap();

        [OperationContract]
        Task<GeoCoordinate> GetCoordinateFromAddressSoap(string address);

        [OperationContract]
        Task<Station> GetClosestStationSoap(double lat, double lon);

        [OperationContract]
        Task<Path[]> GetPathSoap(string origin, string destination);
    }
}
