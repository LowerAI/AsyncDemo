using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子longRunning
    /// </summary>
    internal class Program20
    {
        private static void Main20(string[] args)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("Foo");
            }, TaskCreationOptions.LongRunning);
        }
    }
}