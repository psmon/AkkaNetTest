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
    public class SimpleActor : ReceiveActor
    {
        protected ILoggingAdapter Log = Context.GetLogger();

        public SimpleActor()
        {
            Receive<string>(message => {
                Log.Info("Received String message: {0}", message);
                Sender.Tell("re:" + message);
            });

            Receive<int>(num => {
                Log.Info("Received String message: {0}", num);
                Sender.Tell( num * 2 );
            });

        }
    }

    public class SimpleClass
    {
        public string Receive(string message)
        {
            return "re:" + message;
        }

        public int Receive(int num)
        {
            return num * 2;
        }

    }


}
