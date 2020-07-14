using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子tcs
    /// </summary>
    internal class Program26
    {
        private static void Main26(string[] args)
        {
            var tcs = new TaskCompletionSource<int>();

            new Thread(() =>
            {
                Thread.Sleep(5000);
                tcs.SetResult(42);
            })
            {
                IsBackground = true
            }.Start();

            Task<int> task = tcs.Task;
            Console.WriteLine(task.Result);
        }
    }
}