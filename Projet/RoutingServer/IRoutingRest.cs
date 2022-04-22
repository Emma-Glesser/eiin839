using System.ServiceModel;
using System.ServiceModel.Web;
using System.Device.Location;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RoutingServer
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IRoutingRest
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Station?station={station}&contract={contract}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Task<bool> GetAvailability(string station, string contract);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Stations", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Task<List<Station>> GetStations();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Coord?add={address}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        Task<GeoCoordinate> GetCoordinateFromAddress(string address);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Closest?lat={lat}&lon={lon}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        Task<Station> GetClosestStation(double lat, double lon);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Path?origin={origin}&dest={destination}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        Task<Path[]> GetPath(string origin, string destination);
    }
}
