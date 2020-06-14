using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// ### Local 本地的独立
    /// </summary>
    class Program6
    {
        static void Main6()
        {
            new Thread(Go).Start(); // 在新线程上调用Go()
            Go(); // 在main线程上调用Go()
        }

        static void Go()
        {
            for (int cycles = 0; cycles < 5; cycles++)
            {
                Console.WriteLine('?');
            }
        }
    }
}
