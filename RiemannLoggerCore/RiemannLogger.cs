using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Generic;

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

            var formattedLogValues = state as IReadOnlyList<KeyValuePair<string, object>>;
        
            var args = new List<KeyValuePair<string, string>>();

            Console.WriteLine("=============================================");

            if (formattedLogValues != null)
            {
                foreach (var logValue in formattedLogValues)
                {
                    if (logValue.Value != null && logValue.Key != "{OriginalFormat}")
                    {
                        args.Add(new KeyValuePair<string, string>(logValue.Key, logValue.Value.ToString()));
                        Console.WriteLine(logValue.Key + "=" + logValue.Value.ToString());
                    }
                }
            }

            if (current != null)
            {
                foreach (var logValue in current._state as IReadOnlyList<KeyValuePair<string, object>>)
                {
                    if (logValue.Value != null && logValue.Key != "{OriginalFormat}")
                    {
                        args.Add(new KeyValuePair<string, string>(logValue.Key, logValue.Value.ToString()));
                        Console.WriteLine(logValue.Key + "=" + logValue.Value.ToString());
                    }
                }
            }


            client.Send(service: "Demo Client", state: logLevel.ToString(), description: message, args: args);
        }
    }
}
