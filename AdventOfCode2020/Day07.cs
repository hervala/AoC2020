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

    [Command(nameof(Day07))]
    public class Day07 : DayCommand, ICommand
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

        private IEnumerable<string> ParseInput(string input) => input.Split(Environment.NewLine + Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s));

        private string day7Input = @"";

    }
}
