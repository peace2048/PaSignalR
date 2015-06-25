using System;
using Microsoft.AspNet.SignalR.Client;

namespace PsSignalR.CommandLine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var conn = new Microsoft.AspNet.SignalR.Client.HubConnection("http://localhost:55740/");
            var hub = conn.CreateHubProxy("planHub");
            hub.On<int, int>("Updated", (plan, result) =>
            {
                Console.WriteLine("PLAN  :{0}", plan);
                Console.WriteLine("RESULT:{0}", result);
                Console.WriteLine("DIFF  :{0}", result - plan);
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