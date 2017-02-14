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
        protected ILoggingAdapter Log = Context.GetLogger();

        public TestActor()
        {
            Receive<string>(message => {
                Log.Error("Received String message: {0}", message);
                
                Sender.Tell("re:" + message);
            });

            Receive<List<int>>(message => {
                Log.Error("Received ListCount: {0}", message.Count);
                Sender.Tell("re:" + message);
            });
        }
    }
}
