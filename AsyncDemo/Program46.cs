using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子6162
    /// </summary>
    class Program46
    {
        static async Task Main46(string[] args)
        {
            Func<Task<int>> unnamed = async () =>
            {
                await Task.Delay(1000);
                return 123;
            };

            int answer = await unnamed();
        }
    }
}