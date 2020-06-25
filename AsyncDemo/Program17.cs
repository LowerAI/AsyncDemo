using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 例子signaling:使用信号来实现线程的阻塞和重启
    /// </summary>
    class Program17
    {
        static void Main17(string[] args)
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
