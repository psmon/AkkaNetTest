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
            Receive<string>(message =>
            {
                Log.Info("Received String message: {0}", message);
                Sender.Tell("re:" + message);
            });

            Receive<int>(num =>
            {
                Log.Info("Received String message: {0}", num);
                Sender.Tell(num * 2);
            });

        }

        protected override void PreStart()
        {
            Context.ActorOf<TestActor>("mychild");
        }

        protected override void PostStop()
        {
        }
    }

    public class SimpleClass
    {
        public string Receive(string message)
        {
            return "re:" + message;
        }

        async public Task<int> Receive(int num)
        {
            // Rest , 네트워크로 확장시
            // ASP.net MVC 채택
            // Route 정의 ( /simple )
            // Controller 정의 ( simple/Post )
            // Controller 내에서 SimpleClass 생성후 반환함수 연결...  (10라인 코드 추가)

            return num * 2;
        }

    }


}
