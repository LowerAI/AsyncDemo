using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 本例原本应该是WPF页面
    /// </summary>
    class Program37
    {
        static void Main37(string[] args)
        {
        }

        //void Go()
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        _result.Text += GetPrimesCountAsync(i * 1000000, 1000000) + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1) + Environment.NewLine;
        //    }
        //}

        static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() => ParallelEnumerable.Range(start, count).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}