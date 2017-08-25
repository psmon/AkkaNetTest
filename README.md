# AkkaNetTest

Akka를 이용하여 리모트,클러스터,샤딩,퍼시던트등 다양한 

메시지모델및 분산처리를 직접 구현해보고 테스트해보는 코드입니다. (Akka/Java/Scala가 원조 )

RestAPI를 대체하는 메시지모델이아닌,연동전략을 위한 고급네트워크 라이브러리 학습을 통하여 조금더 풍성하고 고성능 API를 설계/구현을 할수 있는 능력을 가지는것으로 목표로 하고있습니다.

![remote](http://www.aaronstannard.com/images/2015/markedup/markedup-in-app-marketing-network-topology.png)

link:http://www.aaronstannard.com/markedup-akkadotnet/


Akka를 이용한 개인진행 저장소 : https://github.com/psmon/psmonSearch/blob/master/README.md

# Actor란 무엇인가?

## Thread VS Actor

![remote](http://getakka.net/images/exception_prop.png)

![remote](http://getakka.net/images/serialized_timeline_invariants.png)

# 장애처리 모델(Supervision)

## One-For-One Strategy vs. All-For-One Strategy

![image](http://getakka.net/images/OneForOne.png)

![image](http://getakka.net/images/AllForOne.png)


link : http://getakka.net/articles/concepts/supervision.html

## OOP VS Actor

# Remote Test

![remote](http://getakka.net/images/remoting-initial-state.png)

![remote](http://getakka.net/images/remote-address-annotation.png)


Server Config

	akka {	            
		remote {
			log-remote-lifecycle-events = DEBUG
			log-received-messages = on
              
			helios.tcp {
				port = 9000 #bound to a specific port
				hostname = 127.0.0.1
			}
	}


Server Code Sample

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
    
    using (ActorSystem system = ActorSystem.Create("MyServer", config))
	{
		system.ActorOf<MyActor>("greeter");
		Console.ReadKey();
	}
            
      
Client Code Sample

    var greeter = system.ActorSelection("akka.tcp://MyServer@localhost:9000/user/greeter");
    var result = greeter.Ask("Hi2", null).Result; //Wair For Result
    Console.WriteLine(result);

    List<int> tmpList = new List<int>();
    greeter.Tell(tmpList);    //Just Tell...

# BroadCast Test

![remote](http://getakka.net/images/BroadcastRouter.png)

Server Code Sample

	var system = ActorSystem.Create("ServiceB")
	system.ActorOf<TestActor>("b1");
	system.ActorOf<TestActor>("b2");
	system.ActorOf<TestActor>("b3");

Client Code Sample

	provider = "Akka.Remote.RemoteActorRefProvider, Akka.Remote"
		deployment {
		/some-group {
			router = broadcast-group
			routees.paths = [
			"akka.tcp://ServiceB@127.0.0.1:8002/user/b1",
			"akka.tcp://ServiceB@127.0.0.1:8002/user/b2", 
			"akka.tcp://ServiceB@127.0.0.1:8002/user/b3"]
		}
                    
	}		
	var props = Props.Create<TestActor>().WithRouter(FromConfig.Instance);
    var broadCaseActor = system.ActorOf(props, "some-group");
	broadCaseActor.Tell("HI");


# RoundRobin Test

![remote](http://getakka.net/images/RoundRobinRouter.png)

Config Sample

	akka.actor.deployment {
	  /some-pool {
		router = round-robin-pool
		nr-of-instances = 5
	  }
	}



# ConsistentHashRouter Test

![remote](http://getakka.net/images/ConsistentHashRouter.png)

Message Sample

	public class SomeMessage : IConsistentHashable
	{
	   public Guid GroupID { get; private set; }
	   public object ConsistentHashKey {  get { return GroupID; } }
	}

Config Sample

	akka.actor.deployment {
	  /some-pool {
		router = consistent-hashing-pool
		nr-of-instances = 5
		virtual-nodes-factor = 10
	  }
	}


# ScatterGatherFirstCompleted Test

![remote](http://getakka.net/images/ScatterGatherFirstCompletedRouter.png)

Config Sample

	akka.actor.deployment {
	  /some-pool {
		router = scatter-gather-pool
		nr-of-instances = 5
		within = 10s
	  }
	}

# SmallestMailBox Test

![remote](http://getakka.net/images/SmallestMailbox.png)

Config Sample

	akka.actor.deployment {
	  /some-pool {
		router = smallest-mailbox-pool
		nr-of-instances = 5
	  }
	}

# Persistence Test ( Memory DB,Local DB )

Persistence모듈은 메모리DB인 redis기능 자체를 모두포함하는 기능은아니며

Actor의 상태를 좀더 추상적(FSM디자인패턴)으로 관리하고 유지함으로, 서비스특화된 고성능 메모리DB를 구현하는데 의의가 있습니다.

link : http://getakka.net/articles/persistence/persistent-fsm.html

## In-memory journal plugin.

	akka.persistence.journal.inmem {


## In-memory snapshot store plugin.

	akka.persistence.snapshot-store.inmem {
		# Class name of the plugin.
		class = "Akka.Persistence.Snapshot.MemorySnapshotStore, Akka.Persistence"
		# Dispatcher for the plugin actor.
		plugin-dispatcher = "akka.actor.default-dispatcher"
	}

## Local file system snapshot store plugin.

	akka.persistence.snapshot-store.local {    

# cluster-sharding

![remote](https://petabridge.com/images/2017/cluster-sharding-intro/sharding-hierarchy.png)

link : https://petabridge.com/blog/introduction-to-cluster-sharding-akkadotnet/


# Java와 이기종통신

AKKA의 태생은 JAVA로부터 왔기때문에, .net에서 Actor기반 메시지 설계가 가능하다면

JAVA에서도 동일한 컨셉으로 구현이 가능합니다. 이기종간 Actor메시지 전송을 기대해보지만 아직 미지원으로 확인되었으며

이경우 웹소켓중간 인터페이스를 경유해 이기종 고성능 통신이 가능합니다. 

플래폼 파편화에 따른 저수준의 순수한 고성능 이기종 통신이 필요하다면 JnBridge 도입 검토도 해볼 필요가 있습니다.

link : http://git.interparktour.com/N17042/AKKATEST/snippets/7

link2 : https://www.playframework.com/documentation/2.6.x/ScalaWebSockets

link3 : https://jnbridge.com/

# Akka를 이용한 머신러닝으로 확장(텐서플로우 대체 플래폼은 아님)

AKKA는 텐서플로우 대체 플랫폼도 아니며 AI전용 플랫폼도아닙니다.

네트워크 라이브러리가, 어떻게 머신러닝으로 확장가능한지? 심화학습을 하는데 목적이 있습니다.

![remote](http://www.cakesolutions.net/hs-fs/hub/323094/file-2535964003-png/Carl/Classification_Workflow_-_Detail.png?t=1502444754289)

link : http://getakka.net/articles/streams/workingwithgraphs.html (고성능 네트워크 데이터 전송및 연산 실현을 위해,AkkaStreams/Graphs로 확장이됩니다.)

link : http://www.cakesolutions.net/teamblogs/lifting-machine-learning-into-akka-streams
 
    
