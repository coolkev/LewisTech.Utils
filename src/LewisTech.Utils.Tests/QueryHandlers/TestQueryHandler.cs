using System;
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
            
            HandleWasCalled = true;
            
            return new TestQueryResult();
        }
    }
     public class TestQueryHandler2 : IQueryHandler<TestQuery2, TestQueryResult>
    {
        public bool HandleWasCalled { get; private set; }
        
        public TestQueryResult Handle(TestQuery2 query)
        {

            HandleWasCalled = true;
            
            return new TestQueryResult();
        }
    }
    public class TestQueryHandler3 : IQueryHandler<TestQuery3, TestQueryResult3>
    {
        public bool HandleWasCalled { get; private set; }
        
        public TestQueryResult3 Handle(TestQuery3 query)
        {

            HandleWasCalled = true;
            
            return new TestQueryResult3();
        }
    }
    
    public class TestQueryHandlerException : IQueryHandler<TestQuery, TestQueryResult>
    {
        
        public TestQueryResult Handle(TestQuery query)
        {
            throw new Exception("Test Exception");
        }
    }
}