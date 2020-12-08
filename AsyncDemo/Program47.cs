using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子
    /// </summary>
    class Program47
    {
        static async Task Main47(string[] args)
        {
            Console.WriteLine(await GetWebPageAsync("http://oreilly.com"));
        }

        static Dictionary<string, string> _cache = new Dictionary<string, string>();

        static async Task<string> GetWebPageAsync(string uri)
        {
            if (_cache.TryGetValue(uri, out string html))
                return html;
            return _cache[uri] = await new WebClient().DownloadStringTaskAsync(uri);
        }
    }
}