using System;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子task
    /// </summary>
    internal class Program18
    {
        private static void Main18(string[] args)
        {
            Task.Run(() => Console.WriteLine("Foo"));
            Console.ReadLine();
        }
    }
}