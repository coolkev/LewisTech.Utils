using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LewisTech.Utils.Command;
using LewisTech.Utils.Command.Decorators;
using LewisTech.Utils.Tests.QueryHandlers;
using NUnit.Framework;

namespace LewisTech.Utils.Tests.CommandHandlers
{
    [TestFixture]
    public class CommandHandlerTestFixture
    {

        [Test]
        public void RunTestCommandSync()
        {
            var stopwatch = Stopwatch.StartNew();

            var handler = new TestCommandHandler(stopwatch);

            var result = handler.Handle(new TestCommand());

            Assert.IsTrue(handler.HandleWasCalled);

        }

        [Test]
        public void TestCommandProcessor()
        {

            var stopwatch = Stopwatch.StartNew();

            var handler = new TestCommandHandler(stopwatch);

            var processor = new CommandProcessor(t => handler);

            var result = processor.Process(new TestCommand());

            Assert.IsTrue(handler.HandleWasCalled);

        }


        [Test]
        public void RunTestCommandWithValidationDecorator()
        {

            var stopwatch = Stopwatch.StartNew();

            var handler = new TestCommandHandler(stopwatch);
            var decorator = CommandValidationDecorator.Decorate(handler);

            var result = decorator.Handle(new TestCommand());
            
            Assert.IsTrue(handler.HandleWasCalled);
            var helper = decorator.Helper();

            Assert.AreEqual(1, helper.DecoratorChain().Count());

            Assert.AreSame(handler, decorator.Decorated);
            Assert.AreSame(handler, helper.RootHandler());
        }

        [Test]
        public async Task RunTestCommandWithValidationAndMakeAsyncDecorator()
        {

            var stopwatch = Stopwatch.StartNew();

            var handler = new TestCommandHandler(stopwatch);
            var asyncDecorator = MakeCommandAsyncDecorator.Decorate(handler);
            var decorator = CommandValidationDecorator.Decorate(asyncDecorator);

            var result = await decorator.HandleAsync(new TestCommand());

            Assert.IsTrue(handler.HandleWasCalled);

            Assert.AreEqual(2, decorator.Helper().DecoratorChain().Count());

            Assert.AreSame(handler, asyncDecorator.Decorated);
            Assert.AreSame(asyncDecorator, decorator.Decorated);

            Assert.AreSame(handler, decorator.Helper().RootHandler());
        }
    }
}
