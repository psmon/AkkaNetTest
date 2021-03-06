﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Routing;
using Akka.Event;
using Akka.Configuration;

using Akka.Cluster;

using ServiceActor.Shared.Actors;

namespace ServiceB
{
    class Program
    {
        static void Main(string[] args)
        {

            ConsoleKeyInfo cki;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

            using (var system = ActorSystem.Create("ServiceB"))
            {
                var props = Props.Create<TestActor>().WithRouter(FromConfig.Instance);
                var roundRobbinActor = system.ActorOf(props, "workers");

                var props2 = Props.Create<TestActor>().WithRouter(FromConfig.Instance);
                var randomActor = system.ActorOf(props2, "some-group2");

                system.ActorOf<TestActor>("b1");
                system.ActorOf<TestActor>("b2");
                system.ActorOf<TestActor>("b3");

                while (true)
                {
                    try
                    {                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Console.Write("Press any key, or 'X' to quit, or ");
                    Console.WriteLine("CTRL+C to interrupt the read operation:");

                    // Start a console read operation. Do not display the input.
                    cki = Console.ReadKey(true);                    

                    // Announce the name of the key that was pressed .
                    Console.WriteLine("  Key pressed: {0}\n", cki.Key);


                    // 심플 액터 테스트....
                    var simpleActor = system.ActorSelection("akka.tcp://ServiceA@127.0.0.1:8001/user/simple");
                    Console.WriteLine("simple Result:" + simpleActor.Ask( 100,TimeSpan.FromSeconds(2) ).Result ) ;


                    // 클러스터 테스트 ( 랜덤 )
                    //randomActor.Tell(cki.Key.ToString());

                    // 클러스터 테스트 ( 라운드 로빈 )
                    //Console.WriteLine( roundRobbinActor.Ask(cki.Key.ToString()).Result );

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
