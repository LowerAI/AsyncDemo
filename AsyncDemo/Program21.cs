using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子tresult
    /// </summary>
    class Program21
    {
        static void Main21(string[] args)
        {
            Task<int> task = Task.Run(() =>
            {
                Console.WriteLine("Foo");
                return 3;
            });

            int result = task.Result;  // 如果task没完成， 那么就阻塞i
            Console.WriteLine(result); // 3
        }
    }
}
