using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Configuration;


namespace ServiceA
{
    public class MyActor : ReceiveActor
    {        
        public MyActor()
        {
            Receive<string>(message => {                
                Console.WriteLine("Received String message: {0}", message);
                Sender.Tell("re:"+message);
            });

            Receive<List<int>>(message => {
                Console.WriteLine("Received ListCount: {0}", message.Count);
                Sender.Tell("re:" + message);
            });
        }
    }

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

            using (ActorSystem system = ActorSystem.Create("MyServer", config))
            {
                system.ActorOf<MyActor>("greeter");
                Console.ReadKey();
            }

        }
    }
}
