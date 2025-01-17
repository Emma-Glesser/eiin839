﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientSoap.RoutingServer;

namespace ClientSoap
{
    internal class Program
    {
        private static RoutingSoapClient client = new RoutingSoapClient();
        public static void Main(string[] args)
        {
            Console.WriteLine("Heavy Client ready ! Type help to see commands");
            while(true)
            {
                Console.Write(">");
                string command = Console.ReadLine();
                if (command.Equals("quit"))
                {
                    break;
                }
                if (command.Equals("cache"))
                {
                    TimeTakeToGetPath();
                }
                if (command.Equals("cities"))
                {
                    CompareTwoCities();
                }
                if (command.Equals("path"))
                {
                    Console.Write("start : ");
                    string start = Console.ReadLine();
                    Console.Write("finish : ");
                    string dest = Console.ReadLine();
                    Path[] paths = client.GetPathSoap(start, dest);
                    Console.WriteLine(paths);
                }
                if (command.Equals("closest"))
                {
                    Console.Write("latitude : ");
                    double lat = Double.Parse(Console.ReadLine());
                    Console.Write("longitude : ");
                    double lon = Double.Parse(Console.ReadLine());
                    Station station = client.GetClosestStationSoap(lat,lon);
                    Console.WriteLine(station);
                }
                if (command.Equals("gps"))
                {
                    Console.Write("address : ");
                    string address = Console.ReadLine();
                    GeoCoordinate gps = client.GetCoordinateFromAddressSoap(address);
                    string result = gps.Latitude + " , " + gps.Longitude;
                    Console.WriteLine(result);
                }
                if (command.Equals("available"))
                {
                    Console.Write("station : ");
                    string station = Console.ReadLine();
                    Console.Write("contract : ");
                    string contract = Console.ReadLine();
                    bool result = client.GetAvailabilitySoap(station,contract);
                    Console.WriteLine(result);
                }
                if (command.Equals("help"))
                {
                    Console.WriteLine("-----------Commands-----------");
                    Console.WriteLine("quit");
                    Console.WriteLine("cache");
                    Console.WriteLine("cities");
                    Console.WriteLine("path");
                    Console.WriteLine("closest");
                    Console.WriteLine("gps");
                    Console.WriteLine("available");
                }
            }
        }

        private static void TimeTakeToGetPath()
        {
            string start = "Oslo";
            string finish = "Seville";
            Console.WriteLine("Time take to get path from " + start + " to " + finish + " :");
            DateTime init1 = DateTime.Now;
            client.GetPathSoap(start, finish);
            DateTime end1 = DateTime.Now;
            Console.WriteLine("First call to rounting server : " + (end1 - init1));
            DateTime init2 = DateTime.Now;
            client.GetPathSoap(start, finish);
            DateTime end2 = DateTime.Now; 
            Console.WriteLine("Second call to rounting server : " + (end2 - init2));
        }

        private static void CompareTwoCities()
        {
            string start1 = "Oslo";
            string finish1 = "Seville";
            Console.Write("Time take to get path from " + start1 + " to " + finish1 + " :");
            DateTime init1 = DateTime.Now;
            client.GetPathSoap(start1, finish1);
            DateTime end1 = DateTime.Now;
            Console.WriteLine(end1 - init1);

            string start2 = "31 Av. Minerve, 13015 Marseille";
            string finish2 = "Rue de la Largade, 13015 Marseille";
            Console.Write("Time take to get path from " + start2 + " to " + finish2 + " :");
            DateTime init2 = DateTime.Now;
            client.GetPathSoap(start2, finish2);
            DateTime end2 = DateTime.Now;
            Console.WriteLine(end2 - init2);
        }
    }

}
