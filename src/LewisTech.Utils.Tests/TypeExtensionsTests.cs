using System;
using LewisTech.Utils.Command;
using LewisTech.Utils.Query;
using LewisTech.Utils.Tests.QueryHandlers;
using NUnit.Framework;

namespace LewisTech.Utils.Tests
{
    [TestFixture]
    public class TypeExtensionsTests
    {


        [TestCase(typeof(TypeExtensionsTests), "TypeExtensionsTests")]
        [TestCase(typeof(IQueryHandler), "IQueryHandler")]
        [TestCase(typeof(IQueryHandler<TestQuery, TestQueryResult>), "IQueryHandler<TestQuery, TestQueryResult>")]
        [TestCase(typeof(ICommand<TestCommandResult>), "ICommand<TestCommandResult>")]
        public void GetTypeDisplayName(Type type, string expectedName)
        {

            Assert.AreEqual(type.DisplayName(), expectedName);
        }
    }
}
