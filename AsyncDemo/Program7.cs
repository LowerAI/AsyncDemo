using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 如果多个线程都引用到同一个对象的实例，那么他们就共享了数据
    /// </summary>
    class ThreadTest
    {
        bool _done;

        static void Main7()
        {
            ThreadTest tt = new ThreadTest(); // 创建了一个共同的实例
            new Thread(tt.Go).Start();
            tt.Go();
        }

        void Go() // 这是一个实例方法
        {
            if (!_done)
            {
                _done = true;
                Console.WriteLine("Done");
            }
        }
    }
}
