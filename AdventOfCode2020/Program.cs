using CliFx;
using System;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    class Program
    {
        public static async Task<int> Main() =>
        await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
    }
}
