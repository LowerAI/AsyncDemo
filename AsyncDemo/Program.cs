using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子longRunning
    /// </summary>
    class Program20
    {
        static void Main(string[] args)
        {
            Task task = Task.Factory.StartNew(() => {
                Thread.Sleep(3000);
                Console.WriteLine("Foo");
            }, TaskCreationOptions.LongRunning);
        }
    }
}
