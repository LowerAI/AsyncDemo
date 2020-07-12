using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 如果交换Go方法里语句的顺序，那么“Done”被打印两次的几率会大大增加
    /// </summary>
    internal class Program10
    {
        private static bool _done = false;

        private static void Main10()
        {
            new Thread(Go).Start();
            Go();
        }

        private static void Go()
        {
            if (!_done)
            {
                Console.WriteLine("Done");
                Thread.Sleep(100);
                _done = true;
            }
        }
    }
}