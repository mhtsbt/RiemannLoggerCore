using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiemannLoggerCore
{
    public class RiemannLoggerProvider : ILoggerProvider
    {


        public ILogger CreateLogger(string categoryName)
        {
            var client = new RiemannClient();
            return new RiemannLogger(client);
        }

        public void Dispose()
        {
            
        }
    }
}
