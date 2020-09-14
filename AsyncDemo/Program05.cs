using System;
using System.Threading;

namespace AsyncDemo
{
    internal class Program05
    {
        private static void Main5()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Sleep for 2 seconds.");
                Thread.Sleep(2000);
            }

            Console.WriteLine("Main thread exits.");
        }
    }
}