using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 
    /// </summary>
    class Program14
    {
        static void Main14()
        {
            for (int i = 0; i < 10; i++)
            {
                int temp = i;
                new Thread(() => Console.Write(temp)).Start();
            }
        }

        // 1 在循环的整个生命周期内指向的是同一个内存地址
        // 每个线程对 Console.WriteLine() 的调用都会在它运行的时候进行修改。
        // 解决方案 (captured-solution)
    }
}
