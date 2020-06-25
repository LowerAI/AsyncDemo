using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子task
    /// </summary>
    class Program18
    {
        static void Main18(string[] args)
        {
            Task.Run(() => Console.WriteLine("Foo"));
            Console.ReadLine();
        }
    }
}
