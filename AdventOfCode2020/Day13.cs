using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Math;

namespace AdventOfCode2020
{
    [Command(nameof(Day13))]
    public class Day13 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            var (earliestDepartTime, busTimes) = ParseInput(day13Input);
            foreach (var bus in busTimes)
            {
                var sum = 0;
                do
                {
                    sum += bus.Key;
                } while (sum < earliestDepartTime);
                busTimes[bus.Key] = sum - earliestDepartTime;
            }
            var earliestBus = busTimes.Aggregate((lowest, current) => current.Value < lowest.Value ? current : lowest);
            console.Output.WriteLine($"{earliestBus.Key * earliestBus.Value}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            var (earliestDepartTime, busTimes) = ParseInput(day13Input);
            console.Output.WriteLine($"{"Chinese Remainder Theorem would solve this. Don't have time to learn number theory for now."}");
            return default;
        }

        private (int earliestDepartTime, Dictionary<int, int> busTimes) ParseInput(string input)
        {
            var lines = input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            var earliestDepartTime = int.Parse(lines[0]);
            var busTimes = lines[1].Split(',').Where(s => s != "x").ToDictionary(s => int.Parse(s), _ => 0);
            return (earliestDepartTime, busTimes);
        }

        private string day13Input = @"1006605
19,x,x,x,x,x,x,x,x,x,x,x,x,37,x,x,x,x,x,883,x,x,x,x,x,x,x,23,x,x,x,x,13,x,x,x,17,x,x,x,x,x,x,x,x,x,x,x,x,x,797,x,x,x,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,29
";

    }
}
