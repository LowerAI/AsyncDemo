using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子configureAwait
    /// </summary>
    internal class Program25
    {
        private static void Main25(string[] args)
        {
            Task<int> primeNumberTask = Task.Run(() => Enumerable.Range(2, 3000000).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % 1 > 0)));

            primeNumberTask.ContinueWith(task =>
            {
                int result = task.Result;
                Console.WriteLine(result); // Writes result
            });

            Console.ReadLine();
        }
    }
}