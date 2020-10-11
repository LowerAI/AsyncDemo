using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子6131
    /// </summary>
    class Program41
    {
        static async Task Main41(string[] args)
        {
            
        }

        async Task<int> GetAnswerToLife()
        {
            await Task.Delay(5000);
            int answer = 21 * 2;
            return answer;
        }
    }
}