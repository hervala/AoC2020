using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020
{

    [Command(nameof(Day05))]
    public class Day05 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            console.Output.WriteLine($"{1}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            console.Output.WriteLine($"{1}");
            return default;
        }

        private IEnumerable<string> ParseInput(string input)
        {
            return new List<string>();
        }

        private string day5Input = @"";

    }
}
