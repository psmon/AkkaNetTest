using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace ServiceActor.Shared.Actors
{    
    public class TestActor : ReceiveActor
    {
        public TestActor()
        {
            Receive<string>(message => {
                Console.WriteLine("Received String message: {0}", message);
                Sender.Tell("re:" + message);
            });

            Receive<List<int>>(message => {
                Console.WriteLine("Received ListCount: {0}", message.Count);
                Sender.Tell("re:" + message);
            });
        }
    }
}
