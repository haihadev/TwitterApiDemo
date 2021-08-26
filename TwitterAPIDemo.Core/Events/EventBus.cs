using System;
using System.Collections.Generic;
using System.Linq;
using TwitterAPIDemo.Core.Extensions;

namespace TwitterAPIDemo.Core.Events
{
    public static class EventBus<T>
    {
        static EventBus()
        {
            Listeners = new List<Action<T>>();
        }

        private static List<Action<T>> Listeners { get; }
        private static readonly object LockObject = new object();

        public static void Emit(T source)
        {
            lock (LockObject)
            {
                Listeners.OfType<Action<T>>().ForEach(x => x(source));
            }
        }

        public static void InitializeListener(Action<T> messageAction)
        {
            lock (LockObject)
            {
                Listeners.Add(messageAction);
            }
        }
    }
}
