using System;
using System.Threading.Tasks;
using AdventOfCode2020;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CliFx;

namespace ConsoleApp1
{
    public class Benchmarks
    {
        private Day14 day14;
        private Day15 day15;
        private IConsole console;

        public Benchmarks()
        {
            day14 = new Day14();
            day15 = new Day15();
            (console, _, _) = VirtualConsole.CreateBuffered();
        }

        [Benchmark]
        public ValueTask Day14_Part1() => day14.Part01(console);

        [Benchmark]
        public ValueTask Day14_Part2() => day14.Part02(console);

        [Benchmark]
        public ValueTask Day15_Part1() => day15.Part01(console);

        [Benchmark]
        public ValueTask Day15_Part2() => day15.Part02(console);

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }

}
