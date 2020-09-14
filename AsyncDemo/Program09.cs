using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 静态字段(field)也会在线程间共享数据
    /// </summary>
    internal class Program09
    {
        private static bool _done = false;

        private static void Main9()
        {
            new Thread(Go).Start();
            Go();
        }

        private static void Go()
        {
            if (!_done)
            {
                _done = true;
                Console.WriteLine("Done");
            }
        }
    }
}