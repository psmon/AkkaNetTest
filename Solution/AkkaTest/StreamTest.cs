using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using System.Threading.Tasks;

namespace AkkaTest
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        public void StreamBasic()
        {
            using (var system = ActorSystem.Create("system"))
            {
                var materializer = system.Materializer();
                var source = Source.From(Enumerable.Range(1, 10));
                var sink = Sink.Aggregate<int, int>(0, (agg, i) => agg + i);
                // connect the Source to the Sink, obtaining a RunnableGraph
                var runnable = source.ToMaterialized(sink, Keep.Right);

                // materialize the flow and get the value of the AggregateSink
                Task<int> sum = runnable.Run(materializer);
                int result = sum.Result;

                // sum1 and sum2 are different Tasks!
                var sum1 = runnable.Run(materializer);
                var sum2 = runnable.Run(materializer);

                Console.WriteLine(result);
                Console.WriteLine(sum1.Result);
                Console.WriteLine(sum2.Result);
            }
        }

        [TestMethod]
        public void StrameSinkAndFlows()
        {
            using (var system = ActorSystem.Create("system"))
            {
                var materializer = system.Materializer();

                // Explicitly creating and wiring up a Source, Sink and Flow
                Source.From(Enumerable.Range(1, 6))
                    .Via(Flow.Create<int>().Select(x => x * 2))
                    .To(Sink.ForEach<int>(x => Console.WriteLine(x.ToString())));

                // Starting from a Source
                var source = Source.From(Enumerable.Range(1, 6)).Select(x => x * 2);
                source.To(Sink.ForEach<int>(x => Console.WriteLine(x.ToString())));

                // Starting from a Sink
                var sink = Flow.Create<int>()
                    .Select(x => x * 2)
                    .To(Sink.ForEach<int>(x => Console.WriteLine(x.ToString())));
                Source.From(Enumerable.Range(1, 6)).To(sink);

                // Broadcast to a sink inline
                var sink2 = Sink.ForEach<int>(x => Console.WriteLine(x.ToString()))
                    .MapMaterializedValue(_ => Akka.NotUsed.Instance);

                var otherSink = Flow.Create<int>().AlsoTo(sink2).To(Sink.Ignore<int>());

                var runnable = Source.From(Enumerable.Range(1, 6)).To(sink);
                runnable.Run(materializer);


                Task.Delay(3000).Wait(); //Wait for Result

            }
        }

        [TestMethod]
        public void StrameMaterialization()
        {
            using (var system = ActorSystem.Create("system"))
            {
                var materializer = system.Materializer();

                var flow = Flow.Create<int>().Select(x => x * 2).Where(x => x > 500);
                var fused = Fusing.Aggressive(flow);

                var runnable = Source.From(Enumerable.Range(0, int.MaxValue))
                    .Via(fused)
                    .Take(1000)
                    .To(Sink.ForEach<int>(x => Console.WriteLine(x.ToString())));
                
                var runnable2 = Source.From( Enumerable.Range(0, 1000 ) )
                    .Select(x => x + 1)
                    .Async()
                    .Select(x => x * 2)
                    .To(Sink.ForEach<int>(x => Console.WriteLine(x.ToString())));

                runnable2.Run(materializer);

                runnable.Run(materializer);

                Task.Delay(3000).Wait(); //Wait for Result


            }
        }
    }    
}
