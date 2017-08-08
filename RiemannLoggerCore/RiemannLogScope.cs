using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RiemannLoggerCore
{
    public class ConsoleLogScope
    {
        private readonly string _name;
        public readonly object _state;

        internal ConsoleLogScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public ConsoleLogScope Parent { get; private set; }

        private static AsyncLocal<ConsoleLogScope> _value = new AsyncLocal<ConsoleLogScope>();
        public static ConsoleLogScope Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }

        public static IDisposable Push(string name, object state)
        {
            var temp = Current;
            Current = new ConsoleLogScope(name, state);
            Current.Parent = temp;

            return new DisposableScope();
        }

        public override string ToString()
        {
            return _state?.ToString();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
