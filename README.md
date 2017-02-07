# AkkaNetTest

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
    
    
    
