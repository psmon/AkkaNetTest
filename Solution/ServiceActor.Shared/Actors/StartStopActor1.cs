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
    public class StartStopActor1 : UntypedActor
    {
        protected override void PreStart()
        {
            Console.WriteLine("first started");
            Context.ActorOf(Props.Create<StartStopActor2>(), "second");
        }

        protected override void PostStop() => Console.WriteLine("first stopped");

        protected override void OnReceive(object message)
        {
            if(message is string)
            {
                switch (message as string)
                {
                    case "stop":
                        Context.Stop(Self);
                        break;
                }
            }            
        }
    }

    public class StartStopActor2 : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("second started");
        protected override void PostStop() => Console.WriteLine("second stopped");

        protected override void OnReceive(object message)
        {
        }
    }
}
