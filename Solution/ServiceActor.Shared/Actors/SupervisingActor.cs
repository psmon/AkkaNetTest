using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;

using Akka.Pattern;

namespace ServiceActor.Shared.Actors
{
    public class SupervisingActor : UntypedActor
    {
        private IActorRef child = Context.ActorOf(Props.Create<SupervisedActor>(), "supervised-actor");

        protected override void OnReceive(object message)
        {            
            switch (message as string)
            {
                case "failChild":
                    child.Tell("fail");
                    break;
            }            
        }
    }

    public class SupervisedActor : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("supervised actor started");
        protected override void PostStop() => Console.WriteLine("supervised actor stopped");

        protected override void OnReceive(object message)
        {
            switch (message as string)
            {
                case "fail":
                    Console.WriteLine("supervised actor fails now");
                    throw new Exception("I failed!");
            }
        }
    }
}
