using System;

namespace MTD.CouchBot.Core
{
    class Program
    {
        private static void Main(string[] args) => Startup.RunAsync(args).GetAwaiter().GetResult();
    }
}
