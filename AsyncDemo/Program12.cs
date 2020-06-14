using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 如果你想往线程的启动方法里传递参数，最简单的方式是使用lambda表达式，在里面使用参数调用方法
    /// </summary>
    class Program12
    {
        static void Main12()
        {
            Thread t = new Thread(() => Print("Hello from t!"));
            t.Start();
        }

        static void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
