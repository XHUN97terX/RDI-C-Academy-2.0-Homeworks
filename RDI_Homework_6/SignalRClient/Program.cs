using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        /*
         * consoleLock célja: ne legyen a kiírás közepébe beletömve egy nem odatartozó sor,
         * csak mert async-ot kellett használni, hogy ne legyen deadlock...
         * azt is mutatja, mikor futott le minden request már
         */ 
        static object consoleLock = new object();
        static async void NewEmpire(IHubProxy signalr, dynamic empire)
        {
            await WriteEmpires(signalr, string.Format("A new empire has arisen!\n{0}\n", empire));
        }
        static async void EmpireModified(IHubProxy signalr, int id)
        {
            var k = await signalr.Invoke<Empire>("GetEmpire", id);
            await WriteEmpires(signalr, string.Format("An empire has changed. Now they are known as {0} and they are {1}.\n", k.EName, k.EGov));
        }
        static async void EmpireCrushed(IHubProxy signalr, dynamic empire)
        {
            await WriteEmpires(signalr, string.Format("The {0} empire has been crushed. Another falls to the Prethoryn...\n", empire.EName));
        }
        static async Task WriteEmpires(IHubProxy signalr, string header = null)
        {
            var results = await signalr.Invoke<IEnumerable<Empire>>("GetEmpires");
            lock (consoleLock)
            {
                if (header != null)
                    Console.WriteLine(header);
                Console.WriteLine("The current empires are:");
                foreach (var item in results)
                    Console.WriteLine(item);
                Console.WriteLine("\n");
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter when the server is ready...");
            Console.ReadLine();
            Console.Clear();

            string url = "http://localhost:8080";
            var hubConnection = new HubConnection(url);
            var hubProxy = hubConnection.CreateHubProxy("EmpireHub");

            hubProxy.On("NewEmpire", x => NewEmpire(hubProxy, new Empire() { EGov = x.EGov, Empno = x.Empno, EName = x.EName }));
            hubProxy.On("EmpireModified", x => EmpireModified(hubProxy, (int)x));
            hubProxy.On("EmpireCrushed", x => EmpireCrushed(hubProxy, x));

            hubConnection.Start().ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    Console.WriteLine("ERROR" + x.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("CONNECTED");
                }
            }).Wait();

            var task = hubProxy.Invoke<IEnumerable<string>>("GetEmpireNames");
            task.Wait();
                Console.WriteLine("The names of the current empires:");
                foreach (var k in task.Result)
                    Console.WriteLine(k);
                Console.WriteLine("\n");

            task = hubProxy.Invoke<IEnumerable<string>>("GetGovernmentNames");
            task.Wait();
                Console.WriteLine("Government types:");
                foreach (var k in task.Result)
                    Console.WriteLine(k);
                Console.WriteLine("\n");
            /*
             * nem akarom (ennél is jobban) restruktúrálni a kódot, hogy valamennyi sorrendiség még legyen azért...
             * egy kis delay pont elégnek tűnik, a lockkal végképp...
             * de lehet egy lassabb gépen (vagy éppen gyorsabb gépen?) nem elég...
             */
            var j = new Empire() { EName = "Federation Late to the Game", EGov = "Prethoryn food", Empno = 21 };
            hubProxy.Invoke("AddEmpire", j).Wait();
            Task.Delay(20).Wait();
            hubProxy.Invoke("ModifyEmpireGovernment", 21, "Human Federation").Wait();
            Task.Delay(20).Wait();
            hubProxy.Invoke("RemoveEmpire", 21).Wait();
            Task.Delay(20).Wait();
            lock (consoleLock)
                Console.WriteLine("Press Enter to stop...");
            Console.ReadLine();
            hubConnection.Stop();
        }
    }
}
