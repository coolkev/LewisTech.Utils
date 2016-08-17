using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LewisTech.Utils.Query;
using NUnit.Framework;

namespace LewisTech.Utils.Tests.QueryHandlers
{
    [TestFixture]
    public class QueryHandlerTestFixture
    {

        [Test]
        public void RunTestQuerySync()
        {
            var stopwatch = Stopwatch.StartNew();

            var handler = new TestQueryHandler(stopwatch);

            TestQueryResult result = handler.Handle(new TestQuery());

            Assert.IsTrue(handler.HandleWasCalled);

        }

        [Test]
        public async Task RunTestQueryAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var handler = new TestAsyncQueryHandler(stopwatch);

            var task = handler.HandleAsync(new TestQuery());

            Assert.IsTrue(handler.HandleWasStarted);
            var result = await task;

            Assert.IsTrue(handler.HandleWasFinished);
        }

        [Test]
        public async Task RunMultipleTestQueryAsyncSequentially()
        {
            var stopwatch = Stopwatch.StartNew();

            var handler1 = new TestAsyncQueryHandler(stopwatch);
            var handler2 = new TestAsyncQueryHandler(stopwatch);

            var result1 = await handler1.HandleAsync(new TestQuery());
            var result2 = await handler2.HandleAsync(new TestQuery());

            Assert.IsTrue(handler1.HandleWasStarted);
            Assert.IsTrue(handler2.HandleWasStarted);

            Assert.IsTrue(handler1.HandleWasFinished);
            Assert.IsTrue(handler2.HandleWasFinished);
        }

        [Test]
        public async Task RunMultipleTestQueryAsyncConcurrently()
        {
            var stopwatch = Stopwatch.StartNew();

            var handler1 = new TestAsyncQueryHandler(stopwatch);
            var handler2 = new TestAsyncQueryHandler(stopwatch);

            var task1 = handler1.HandleAsync(new TestQuery());
            var task2 = handler2.HandleAsync(new TestQuery());

            Assert.IsTrue(handler1.HandleWasStarted);
            Assert.IsTrue(handler2.HandleWasStarted);

            Assert.IsFalse(handler1.HandleWasFinished);
            Assert.IsFalse(handler2.HandleWasFinished);

            var result1 = await task1;
            var result2 = await task2;

            Assert.IsTrue(handler1.HandleWasFinished);
            Assert.IsTrue(handler2.HandleWasFinished);
        }

        [Test]
        public void TestQueryProcessor()
        {


            var processor = new QueryProcessor(t => new TestQueryHandler2());


            for (var x = 0; x < 1000000; x++)
            {

                TestQueryResult result = processor.Process(new TestQuery2());

                //Assert.IsTrue(handler.HandleWasCalled);
            }
        }

        [Test]
        public void TestQueryProcessorResolveException()
        {

            var processor = new QueryProcessor(
                t =>
                    {
                        throw new Exception("Cannot resolve type");
                    });

            Assert.Throws<QueryProcessorResolveHandlerException>(
                () =>
                    {
                        processor.Process(new TestQuery());
                    });
        }

        [Test]
        public void TestQueryProcessorHandleException()
        {

            var processor = new QueryProcessor(t => new TestQueryHandlerException());

            Assert.Throws<QueryProcessorHandleException>(
                () =>
                    {
                        processor.Process(new TestQuery());
                    });
        }

    }
}
