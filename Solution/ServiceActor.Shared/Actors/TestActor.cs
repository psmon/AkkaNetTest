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
            Become(Normal);
        }

        void ChkState(string msg)
        {
            if (msg == "H")
            {
                Log.Info("be happy");
                Become(Happy);
            }
                

            if (msg == "B")
            {
                Log.Info("be normal");
                Become(Normal);
            }
                

            if (msg == "F")
            {
                Log.Info("be unhappy");
                Become(UnHappy);
            }
                
        }

        void Normal()
        {
            Receive<string>(message => {
                Log.Info("Received String message: {0}", message);
                Sender.Tell("I'am Normal re:" + message);
                ChkState(message);
            });
            
        }


        void Happy()
        {
            Receive<string>(message => {
                Log.Info("Received String message: {0}", message);
                Sender.Tell("I'am Happy re:" + message);
                ChkState(message);
            });

            Receive<List<int>>(message => {
                Log.Info("Received ListCount: {0}", message.Count);
                Sender.Tell("I'am Noral re:" + message);
            });

        }

        void UnHappy()
        {
            Receive<string>(message => {
                Log.Info("Received String message: {0}", message);
                Sender.Tell("I'am UnHappy re:" + message);
                ChkState(message);
            });
        }

        
    }
}
