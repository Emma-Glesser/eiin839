using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var svc = new ServiceHost(typeof(ServerProxy));
            svc.Open();
            Console.ReadLine();
        }
    }
}
