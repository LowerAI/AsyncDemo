using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子prime
    /// </summary>
    internal class Program24
    {
        private static void Main24(string[] args)
        {
            Task<int> primeNumberTask = Task.Run(() => Enumerable.Range(2, 3000000).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % 1 > 0)));

            var awaiter = primeNumberTask.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                int result = awaiter.GetResult();
                Console.WriteLine(result); // Writes result
            });

            Console.ReadLine();
        }
    }
}