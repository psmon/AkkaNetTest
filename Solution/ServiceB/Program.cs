using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Configuration;

namespace ServiceB
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }

                remote {
                    helios.tcp {
                        port = 9000
                        hostname = localhost
                    }
                }
            }
            ");

            using (var system = ActorSystem.Create("MyClient", config))
            {
                //get a reference to the remote actor
                var greeter = system
                    .ActorSelection("akka.tcp://MyServer@localhost:9000/user/greeter");
                //send a message to the remote actor
                
                
                var result = greeter.Ask("Hi2", null).Result;
                Console.WriteLine(result);

                for (int i = 0; i < 100; i++)
                {
                    greeter.Tell("Hi..." + i);

                }

                List<int> tmpList = new List<int>();
                tmpList.Add(1);
                tmpList.Add(2);
                tmpList.Add(3);

                greeter.Tell(tmpList);
                
                Console.ReadLine();
            }
        }
    }
}
