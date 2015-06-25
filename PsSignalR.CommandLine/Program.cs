using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace PsSignalR.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = new Microsoft.AspNet.SignalR.Client.HubConnection("http://localhost:55740/");
            var hub = conn.CreateHubProxy("planHub");
            hub.On<int, int>("Updated", (newPlan, newResult) =>
            {
                Console.WriteLine("PLAN  :{0}", newPlan);
                Console.WriteLine("RESULT:{0}", newResult);
                Console.WriteLine("DIFF  :{0}", newResult - newPlan);
            });
            conn.Start().ContinueWith(t =>
            {
                if (args.Length == 2)
                {
                    hub.Invoke("Update", int.Parse(args[0]), int.Parse(args[1]));
                }
                else
                {
                    hub.Invoke("GetPlan");
                }
            });
            Console.ReadLine();
        }
    }
}
