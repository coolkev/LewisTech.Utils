using System.Diagnostics;
using LewisTech.Utils.Query;

namespace LewisTech.Utils.Tests.QueryHandlers
{
    public class TestQueryHandler : IQueryHandler<TestQuery, TestQueryResult>
    {
        public bool HandleWasCalled { get; private set; }

        private Stopwatch _stopwatch;

        public TestQueryHandler(Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
        }

        public TestQueryResult Handle(TestQuery query)
        {
            //Trace.WriteLine("Handle started " + _stopwatch.ElapsedMilliseconds);


            HandleWasCalled = true;

            //Trace.WriteLine("Handle finished " + _stopwatch.ElapsedMilliseconds);

            return new TestQueryResult();
        }
    }
}