using System.Diagnostics;
using LewisTech.Utils.Command;

namespace LewisTech.Utils.Tests.QueryHandlers
{
    public class TestCommandHandler : ICommandHandler<TestCommand, TestCommandResult>
    {
        public bool HandleWasCalled { get; private set; }

        private Stopwatch _stopwatch;

        public TestCommandHandler(Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
        }

        public TestCommandResult Handle(TestCommand query)
        {
            Trace.WriteLine("Handle started " + _stopwatch.ElapsedMilliseconds);


            HandleWasCalled = true;

            Trace.WriteLine("Handle finished " + _stopwatch.ElapsedMilliseconds);

            return new TestCommandResult();
        }

        public TResult Handle<TResult>(ICommand<TResult> command)
        {
            throw new System.NotImplementedException();
        }
    }
}