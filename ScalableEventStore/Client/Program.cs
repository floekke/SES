using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteToNode(1);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        static void WriteToNode(int nodeNumber)
        {
            IEventStoreConnection ConnectTo() => EventStoreConnection.Create(connectionString: $"ConnectTo=tcp://admin:changeit@localhost:{nodeNumber + 1000};");

            var c = ConnectTo();
            c.ConnectAsync().Wait();

            var evt = UTF8Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { foo = "bar" }));
            var meta = UTF8Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { meta = "AKSDFKASDFLSAF" }));

            var s1 = DateTime.UtcNow;
        
            foreach (var x in Enumerable.Range(0, 100000))
            {
                c.AppendToStreamAsync("foo", ExpectedVersion.Any, new EventData(Guid.NewGuid(), "fooEvent", true, evt, meta))
                 .Wait();
            }

            var s2 = DateTime.UtcNow;

            Console.WriteLine($"Wrote events in {(s2 - s1).TotalSeconds}");
        }
    }
}
