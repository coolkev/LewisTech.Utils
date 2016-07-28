using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LewisTech.Utils.Query;

namespace LewisTech.Utils.Tests.QueryHandlers
{
    public class TestAsyncQueryHandler : IAsyncQueryHandler<TestQuery, TestQueryResult>
    {
        public bool HandleWasStarted { get; private set; }
        public bool HandleWasFinished { get; private set; }

        private readonly Stopwatch _stopwatch;

        public TestAsyncQueryHandler(Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
        }

        public async Task<TestQueryResult> HandleAsync(TestQuery query)
        {
            HandleWasStarted = true;
            Trace.WriteLine("Handle started " + _stopwatch.ElapsedMilliseconds);


            await Task.Run(() => Thread.Sleep(1000));

            HandleWasFinished = true;

            Trace.WriteLine("Handle finished " + _stopwatch.ElapsedMilliseconds);

            return new TestQueryResult();
        }

    }
}