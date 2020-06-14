using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 如果多个线程都引用到同一个对象的实例，那么他们就共享了数据
    /// </summary>
    class Program8
    {
        static void Main8()
        {
            bool done = false;

            ThreadStart action = () =>
            {
                if (!done)
                {
                    done = true;
                    Console.WriteLine("Done");
                }
            };

            new Thread(action).Start();
            action();
        }
    }
}
