using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Routing;

namespace ServiceActor.Shared.Actors
{
    public class BroadCastMessage
    {
        public BroadCastMessage(int pid,string message)
        {
            Pid = pid;
            Message = message;
        }
        public int Pid;
        public string Message;
    }

    public class SimpleClusterListener : UntypedActor
    {
                
        protected IActorRef ApiBroadcaster;

        protected ILoggingAdapter Log = Context.GetLogger();
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);

        /// <summary>
        /// Need to subscribe to cluster changes
        /// </summary>
        protected override void PreStart()
        {
            // subscribe to IMemberEvent and UnreachableMember events
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents,
                new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });

            // subscribe to all future IMemberEvents and get current state as snapshot
            Cluster.Subscribe(Self, ClusterEvent.SubscriptionInitialStateMode.InitialStateAsSnapshot,
                new[] { typeof(ClusterEvent.IMemberEvent) });

            // subscribe to all future IMemberEvents and get current state as event stream
            Cluster.Subscribe(Self, ClusterEvent.SubscriptionInitialStateMode.InitialStateAsEvents,
                new[] { typeof(ClusterEvent.IMemberEvent) });
            
        }

        /// <summary>
        /// Re-subscribe on restart
        /// </summary>
        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        protected override void OnReceive(object message)
        {
            var up = message as ClusterEvent.MemberUp;
            if (up != null)
            {
                var mem = up;
                Log.Info("Member is Up: {0}", mem.Member);
            }
            else if (message is ClusterEvent.UnreachableMember)
            {
                var unreachable = (ClusterEvent.UnreachableMember)message;
                Log.Warning("Member detected as unreachable: {0}", unreachable.Member.Roles );                

                if (unreachable.Member.HasRole("seedcrawler") == false)
                {                    
                    //Cluster.Down(unreachable.Member.Address);
                }                

            }
            else if (message is ClusterEvent.MemberRemoved)
            {
                var removed = (ClusterEvent.MemberRemoved)message;
                Log.Warning("Member is Removed: {0}", removed.Member);
            }
            else if (message is ClusterEvent.IMemberEvent)
            {
                Log.Info("Evens {0}", message.ToString());
                //IGNORE
            }
            else if (message is string)
            {
                Log.Info("Received String message: {0}", message);
                Cluster.SendCurrentClusterState(Self);
            }
            else if(message is BroadCastMessage)
            {
                Log.Info("Received BroadCastMessage message: {0}", (message as BroadCastMessage).Message );

            }
            else
            {
                Unhandled(message);
            }
        }
    }
}
