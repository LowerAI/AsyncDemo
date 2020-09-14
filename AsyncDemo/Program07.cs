using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 如果多个线程都引用到同一个对象的实例，那么他们就共享了数据
    /// </summary>
    internal class ThreadTest
    {
        private bool _done;

        private static void Main7()
        {
            ThreadTest tt = new ThreadTest(); // 创建了一个共同的实例
            new Thread(tt.Go).Start();
            tt.Go();
        }

        private void Go() // 这是一个实例方法
        {
            if (!_done)
            {
                _done = true;
                Console.WriteLine("Done");
            }
        }
    }
}