using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using System.Threading.Tasks;

using ServiceActor.Shared;

using ServiceActor.Shared.Actors;
using Akka.Pattern;

namespace AkkaTest
{
    [TestClass]
    public class ActorTest
    {
        [TestMethod]
        [Description("액터 생명주기 체크")]
        public void ActorLifecycle()
        {
            using (var system = ActorSystem.Create("system"))
            {
                var first = system.ActorOf(Props.Create<StartStopActor1>(), "first");   //부모가 자식을 자동생성 => second
                first.Tell("stop"); //부모 액트를 정지시킬시, 하위 액트 종료처리 순서체크
                Task.Delay(3000).Wait();
            }
        }

        [TestMethod]
        [Description("액터 장애처리-Supervision")]
        public void FailureHandling()
        {
            using (var system = ActorSystem.Create("system"))
            {
                var supervisingActor = system.ActorOf(Props.Create<SupervisingActor>(), "supervising-actor");
                supervisingActor.Tell("failChild");
                Task.Delay(3000).Wait();
            }
        }

        [TestMethod]
        [Description("액터 장애처리-BackoffSupervisor")]
        public void FailureHandling2()
        {
            using (var system = ActorSystem.Create("system"))
            {
                var childProps = Props.Create<SupervisedActor>();
                var supervisor = BackoffSupervisor.Props(
                    Backoff.OnFailure(
                        childProps,
                        childName: "supervised-actor",
                        minBackoff: TimeSpan.FromSeconds(3),
                        maxBackoff: TimeSpan.FromSeconds(30),
                        randomFactor: 0.2)
                    );

                var supervisingActor = system.ActorOf( supervisor , "supervising-actor");
                var childActor = system.ActorSelection("/user/supervising-actor/supervised-actor");
                childActor.Tell("fail");                
                Task.Delay(9000).Wait();
            }
        }

    }
}
