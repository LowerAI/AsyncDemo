using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子wait
    /// </summary>
    internal class Program19
    {
        private static void Main19(string[] args)
        {
            Task task = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("Foo");
            });

            Console.WriteLine(task.IsCompleted); // false

            task.Wait(); // 阻塞直至task完成操作

            Console.WriteLine(task.IsCompleted); // true
        }
    }
}