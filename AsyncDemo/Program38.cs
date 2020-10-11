using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子611
    /// </summary>
    class Program38
    {
        static async Task Main38(string[] args)
        {
            await PrintAnswerToLife();
        }

        static async Task PrintAnswerToLife()
        {
            await Task.Delay(5000);
            int answer = 21 * 2;
            Console.WriteLine(answer);
        }
    }
}