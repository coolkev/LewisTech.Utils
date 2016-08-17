using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LewisTech.Utils.Query;
using NUnit.Framework;

namespace LewisTech.Utils.Tests.QueryHandlers
{
    [TestFixture]
    public class QueryProcessorTestFixture
    {

        [Test]
        public void RunTestQuerySync()
        {
            var stopwatch = Stopwatch.StartNew();

            var handler = new TestQueryHandler(stopwatch);

            var processor = new QueryProcessor(t => handler);

            TestQueryResult result = processor.Process(new TestQuery());

            Assert.IsTrue(handler.HandleWasCalled);

        }

        [Test]
        public async Task RunTestQueryAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var handler = new TestAsyncQueryHandler(stopwatch);



            var processor = new QueryProcessor(t => handler);

            var task = processor.ProcessAsync(new TestQuery());


            Assert.IsTrue(handler.HandleWasStarted);
            Assert.IsFalse(handler.HandleWasFinished);

            Trace.WriteLine("Before await " + stopwatch.ElapsedMilliseconds);

            var result = await task;
            Trace.WriteLine("After await " + stopwatch.ElapsedMilliseconds);

            Assert.IsTrue(handler.HandleWasFinished);

            Trace.WriteLine("Test Finished " + stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void RunTestQueryReturnsCorrectHandler()
        {
            var stopwatch = Stopwatch.StartNew();

            TestQueryHandler handler1 = null;
            TestQueryHandler2 handler2 = null;
            TestQueryHandler3 handler3 = null;
            var processor = new QueryProcessor(
                t =>
                    {
                        Console.WriteLine("Resolving Handler: " + t.DisplayName());
                        if (t.IsAssignableFrom(typeof(TestQueryHandler)))
                        {
                            return handler1 = new TestQueryHandler(stopwatch);
                        }
                        if (t.IsAssignableFrom(typeof(TestQueryHandler2)))
                        {
                            return handler2 = new TestQueryHandler2();
                        }
                        if (t.IsAssignableFrom(typeof(TestQueryHandler3)))
                        {
                            return handler3 = new TestQueryHandler3();
                        }
                        return null;
                    });


            var result = processor.Process(new TestQuery());

            Assert.IsNotNull(handler1);
            Assert.IsTrue(handler1.HandleWasCalled);

            var result2 = processor.Process(new TestQuery2());
            Assert.IsNotNull(handler2);
            Assert.IsTrue(handler2.HandleWasCalled);

            var result3 = processor.Process(new TestQuery3());
            Assert.IsNotNull(handler3);
            Assert.IsTrue(handler3.HandleWasCalled);
            
            processor = new QueryProcessor(
                t =>
                    {
                        Console.WriteLine("Resolving Handler: " + t.DisplayName());
                        if (t.IsAssignableFrom(typeof(TestQueryHandler)))
                        {
                            return handler1 = new TestQueryHandler(stopwatch);
                        }
                        if (t.IsAssignableFrom(typeof(TestQueryHandler2)))
                        {
                            return handler2 = new TestQueryHandler2();
                        }
                        if (t.IsAssignableFrom(typeof(TestQueryHandler3)))
                        {
                            return handler3 = new TestQueryHandler3();
                        }
                        return null;
                    });

            result = processor.Process(new TestQuery());

            Assert.IsNotNull(handler1);
            Assert.IsTrue(handler1.HandleWasCalled);

        }
        
    }
}
