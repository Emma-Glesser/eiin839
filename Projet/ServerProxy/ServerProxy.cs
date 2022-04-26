using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.Caching;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerProxy
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class ServerProxy : IServerProxy
    {
        GenericProxyCache<JCDecauxItem> cache = new GenericProxyCache<JCDecauxItem>();
        private const string APIKEY = "6cf8bcf84d387b362b61cea92ff54c74e96a41ea";
        private HttpClient client = new HttpClient();
        public string GetAvailabilities(string station, string contract)
        {
            string key = station + " " + contract;
            Console.WriteLine("Find station : " + key);
            JCDecauxItem item = cache.Get(key, 60);
            return item.content;
        }
        public string GetStations()
        {
            string key = "all";
            Console.WriteLine("Retrieve all stations");
            JCDecauxItem item = cache.Get(key);
            return item.content;
        }
    }

    public class GenericProxyCache<T>
    { 
        ObjectCache cache = MemoryCache.Default;
        public DateTimeOffset dt_default = ObjectCache.InfiniteAbsoluteExpiration;
        
        void setDT_default(DateTimeOffset newdt) { this.dt_default = newdt; }
        public T Get(string CacheItemName)
        {
            T item = (T)cache.Get(CacheItemName,null);
            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T), new object[] { CacheItemName });
                cache.Add(CacheItemName, item, dt_default);
            }
            return item;
        }

        public T Get(string CacheItemName, double dt_seconds)
        {
            T item = (T)cache.Get(CacheItemName, null);
            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T), new object[] { CacheItemName });
                DateTimeOffset time = DateTimeOffset.Now.AddSeconds(dt_seconds);
                cache.Add(CacheItemName, item, time);
            }
            return item;
        }
        public T Get(string CacheItemName, DateTimeOffset dt)
        {
            T item = (T)cache.Get(CacheItemName, null);
            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T), new object[] { CacheItemName });
                cache.Add(CacheItemName, item, dt);
            }
            return item;
        }
    }

    public class JCDecauxItem
    { 
        public string content { get; set; } 
        private HttpClient client = new HttpClient();
        private const string APIKEY = "6cf8bcf84d387b362b61cea92ff54c74e96a41ea";

        public JCDecauxItem(string key)
        {
            string request = "";
            if (key.Equals("all") || key.Equals("all "))
            {
                request = "https://api.jcdecaux.com/vls/v3/stations?apiKey=" + APIKEY;
            }
            else
            {
                string[] keys = key.Split(' ');
                string station_number = keys[0];
                string contract_name = keys[1];
                request = "https://api.jcdecaux.com/vls/v3/stations/" + station_number + "?contract=" + contract_name + "&apiKey=" + APIKEY;  
            }
            try
            {
                HttpResponseMessage response = client.GetAsync(request).Result;
                response.EnsureSuccessStatusCode();
                this.content = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}
