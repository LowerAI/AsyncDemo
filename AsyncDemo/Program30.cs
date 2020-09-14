using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子master
    /// </summary>
    internal class Program30
    {
        private static void Main30(string[] args)
        {
            Delay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));
            // 5秒钟之后，Continuation开始的时候，才占用线程

            Task.Delay(5000).GetAwaiter().OnCompleted(() => Console.WriteLine(42));
            Task.Delay(5000).ContinueWith(ant => Console.WriteLine(42));
            // Task.Delay相当于异步版本的Thread.Sleep
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