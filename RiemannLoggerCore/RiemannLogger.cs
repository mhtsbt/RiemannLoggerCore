using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;

namespace RiemannLoggerCore
{
    public class RiemannLogger : ILogger
    {
        private RiemannClient client;

        public RiemannLogger(RiemannClient client)
        {
            this.client = client;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            //throw new NotImplementedException();
            return ConsoleLogScope.Push("test", state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var current = ConsoleLogScope.Current;

            message = $"{ eventId }: {message} scope: {current}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            client.Send(service: "Demo Client", state: logLevel.ToString(), description: message);
        }
    }
}
