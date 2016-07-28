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

        //[DebuggerStepThrough]
        //public Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        //{
        //    var handlerType =
        //        typeof(IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

        //    dynamic handler = _resolver(handlerType);

        //    return handler.Handle((dynamic)query);
        //}

    }
}
