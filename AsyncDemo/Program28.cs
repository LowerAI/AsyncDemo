using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子timer
    /// </summary>
    internal class Program28
    {
        private static void Main28(string[] args)
        {
            var awaiter = GetAnswerToLife().GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Console.WriteLine(awaiter.GetResult());
            });

            Console.ReadKey();
        }

        // 调用此方法相当于调用Task.Factory.StartNew,
        // 并使用TaskCreationOptions.LongRunning选项来创建非线程池的线程
        static Task<int> GetAnswerToLife()
        {
            var tcs = new TaskCompletionSource<int>();
            var timer = new System.Timers.Timer(5000) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(42); };
            timer.Start();
            return tcs.Task;
        }
    }
}