using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Configuration;
using System.Configuration;

using ServiceActor.Shared.Actors;
using Akka.Routing;

namespace ServiceA
{

    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo cki;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            
            using (ActorSystem system = ActorSystem.Create("ServiceA"))
            {
                
                var props = Props.Create<TestActor>().WithRouter(FromConfig.Instance);
                var actor = system.ActorOf(props, "some-group");

                system.ActorOf<TestActor>("a1");
                system.ActorOf<TestActor>("a2");
                system.ActorOf<TestActor>("a3");
                

                while (true)
                {                    
                    // Start a console read operation. Do not display the input.
                    cki = Console.ReadKey(true);
                    // Announce the name of the key that was pressed .
                    Console.WriteLine("  Key pressed: {0}\n", cki.Key);
                    //clusteActor.Tell("Hello From Seed Node " + cki.Key);

                    //system.ActorSelection("akka.tcp://ServiceB@127.0.0.1:4052/user/b1").Tell(cki.Key.ToString());

                    //actor.Tell(cki.Key.ToString() );

                    Console.WriteLine(actor.Ask(cki.Key.ToString()).Result);

                    // Exit if the user pressed the 'X' key.
                    if (cki.Key == ConsoleKey.X) break;

                }
            }

        }

        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("\nThe read operation has been interrupted.");

            Console.WriteLine("  Key pressed: {0}", args.SpecialKey);

            Console.WriteLine("  Cancel property: {0}", args.Cancel);

            // Set the Cancel property to true to prevent the process from terminating.
            Console.WriteLine("Setting the Cancel property to true...");
            args.Cancel = true;

            // Announce the new value of the Cancel property.
            Console.WriteLine("  Cancel property: {0}", args.Cancel);
            Console.WriteLine("The read operation will resume...\n");
        }
    }
}

