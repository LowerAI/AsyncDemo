using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子612
    /// </summary>
    class Program39
    {
        static async Task Main39(string[] args)
        {
            
        }

        async Task Go()
        {
            await PrintAnswerToLife();
            Console.WriteLine("Done");
        }

        async Task PrintAnswerToLife()
        {
            await Task.Delay(5000);
            int answer = 21 * 2;
            Console.WriteLine(answer);
        }
    }
}