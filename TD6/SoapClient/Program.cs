using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoapClient.MathsOperations;

namespace SoapClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MathsOperationsClient();
            int result = await client.AddAsync(15, 42);
            Console.WriteLine("Addition : 15 + 42 = " + result);
            result = await client.SubstractAsync(15, 42);
            Console.WriteLine("Soustraction : 15 - 42 = " + result);
            result = await client.MultiplyAsync(11, 11);
            Console.WriteLine("Multiplication : 11*11 = " + result);
            double resultd = await client.DivideAsync(25, 5);
            Console.WriteLine("Division : 25/5 = " + resultd);
            Console.ReadLine();
        }
    }
}
