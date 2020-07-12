using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 使用lock语句来加锁
    /// </summary>
    internal class Program11
    {
        private static bool _done = false;
        private static readonly object _locker = new object();

        private static void Main11()
        {
            new Thread(Go).Start();
            Go();
        }

        private static void Go()
        {
            lock (_locker)
            {
                if (!_done)
                {
                    Console.WriteLine("Done");
                    _done = true;
                }
            }
        }
    }
}