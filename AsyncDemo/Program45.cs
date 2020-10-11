using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子616:匿名异步函数
    /// </summary>
    class Program45
    {
        static async Task Main45(string[] args)
        {
            Func<Task> unnamed = async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("Foo1");
            };

            await NamedMethod();
            await unnamed();
        }

        static async Task NamedMethod()
        {
            await Task.Delay(1000);
            Console.WriteLine("Foo2");
        }
    }
}