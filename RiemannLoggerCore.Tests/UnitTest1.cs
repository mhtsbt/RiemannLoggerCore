using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RiemannLoggerCore.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var client = new RiemannClient())
            {
                client.Send(service: "Demo Client", state: "warn", description: "Simple event description");
            }
        }
    }
}
