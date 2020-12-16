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
        private Day14 Day14;
        private Day15 Day15;
        private Day16 Day16;
        private IConsole console;

        public Benchmarks()
        {
            Day14 = new Day14();
            Day15 = new Day15();
            Day16 = new Day16();
            (console, _, _) = VirtualConsole.CreateBuffered();
        }

        [Benchmark]
        public ValueTask Day14_Part1() => Day14.Part01(console);

        [Benchmark]
        public ValueTask Day14_Part2() => Day14.Part02(console);

        [Benchmark]
        public ValueTask Day15_Part1() => Day15.Part01(console);

        [Benchmark]
        public ValueTask Day15_Part2() => Day15.Part02(console);

        [Benchmark]
        public ValueTask Day16_Part1() => Day16.Part01(console);

        [Benchmark]
        public ValueTask Day16_Part2() => Day16.Part02(console);

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }

}
