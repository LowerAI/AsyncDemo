using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 当前线程自身超时
    /// </summary>
    internal class Program04
    {
        private static TimeSpan waitTime = new TimeSpan(0, 0, 1);

        public static void Main4()
        {
            Thread newThread = new Thread(Work);
            newThread.Start();

            if (newThread.Join(waitTime + waitTime))
            {
                Console.WriteLine("New thread terminated.");
            }
            else
            {
                Console.WriteLine("Join time out.");
            }
        }

        private static void Work()
        {
            Thread.Sleep(waitTime);// 暂停当前线程病等待waitTime这么多时间
        }
    }
}