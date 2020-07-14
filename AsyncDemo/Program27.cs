using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子run
    /// </summary>
    internal class Program27
    {
        private static void Main27(string[] args)
        {
            Task<int> task = Run(() =>
            {
                Thread.Sleep(5000);
                return 42;
            });
        }

        // 调用此方法相当于调用Task.Factory.StartNew,
        // 并使用TaskCreationOptions.LongRunning选项来创建非线程池的线程
        static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(function());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }).Start();
            return tcs.Task;
        }
    }
}