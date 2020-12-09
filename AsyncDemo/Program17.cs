using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 例子signaling:使用信号来实现线程的阻塞和重启
    /// </summary>
    internal class Program17
    {
        private static void Main(string[] args)
        {
            var signal = new ManualResetEvent(false);

            new Thread(() =>
            {
                Console.WriteLine("Waiting for signal ... ");
                signal.WaitOne();
                signal.Dispose();
                Console.WriteLine("Got signal!");
            }).Start();

            Thread.Sleep(3000);
            signal.Set();
        }
    }
}