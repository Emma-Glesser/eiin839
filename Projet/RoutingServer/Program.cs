using System;
using System.ServiceModel;

namespace RoutingServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var svc = new ServiceHost(typeof(RoutingServer.RoutingSoap));
            svc.Open();
            var svc2 = new ServiceHost(typeof(RoutingServer.RoutingRest));
            svc2.Open();
            Console.ReadLine();
        }
    }
}
