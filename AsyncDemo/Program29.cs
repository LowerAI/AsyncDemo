using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子delay
    /// </summary>
    internal class Program29
    {
        private static void Main29(string[] args)
        {
            Delay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));
            // 5秒钟之后，Continuation开始的时候，才占用线程

            Console.ReadKey();
        }

        // 注意：没有非泛型版本的TaskCompletionSource
        static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }
    }
}