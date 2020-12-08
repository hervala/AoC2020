using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    [Command(nameof(Day09))]
    public class Day09 : DayCommand, ICommand
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

        private void ParseInput(string input)
        {
            
        }

        private string day9Input = @"";

    }
}
