using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 静态字段(field)也会在线程间共享数据
    /// </summary>
    class Program9
    {
        static bool _done = false;

        static void Main9()
        {
            new Thread(Go).Start();
            Go();
        }

        static void Go()
        {
            if (!_done)
            {
                _done = true;
                Console.WriteLine("Done");
            }
        }
    }
}
