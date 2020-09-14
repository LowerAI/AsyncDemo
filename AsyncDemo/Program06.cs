using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// ### Local 本地的独立
    /// </summary>
    internal class Program06
    {
        private static void Main6()
        {
            new Thread(Go).Start(); // 在新线程上调用Go()
            Go(); // 在main线程上调用Go()
        }

        private static void Go()
        {
            for (int cycles = 0; cycles < 5; cycles++)
            {
                Console.WriteLine('?');
            }
        }
    }
}