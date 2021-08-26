using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TwitterAPIDemo.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToDelimitedString<T>(this IEnumerable<T> source, Func<T, string> function, string delimiter = ",") =>
            string.Join(delimiter, source.Select(function));

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            foreach (T item in values)
            {
                eachAction(item);
            }

            return values;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> values, Action<T, int> eachAction)
        {
            int index = 0;

            foreach (T item in values)
            {
                eachAction(item, index++);
            }

            return values;
        }

        public static async Task ParallelForEachAsync<T>(this IAsyncEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            if (scheduler != null)
                options.TaskScheduler = scheduler;

            var block = new ActionBlock<T>(body, options);

            await foreach (var item in source)
                block.Post(item);

            block.Complete();

            await block.Completion;
        }
    }
}
