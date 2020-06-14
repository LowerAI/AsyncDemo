using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 使用lock语句来加锁
    /// </summary>
    class Program11
    {
        static bool _done = false;
        static readonly object _locker = new object();

        static void Main11()
        {
            new Thread(Go).Start();
            Go();
        }

        static void Go()
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
