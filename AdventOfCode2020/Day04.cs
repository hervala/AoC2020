using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    [Command(nameof(Day04))]
    public class Day04 : DayCommand, ICommand
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

        private int[][] ParseInput(string input) => input.Split(Environment.NewLine)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(row =>
                {
                    var rowArray = new int[row.Length];
                    for (int i = 0; i < row.Length; i++)
                    {
                        rowArray[i] = row[i] == '.' ? 0 : 1;
                    }
                    return rowArray;
                }).ToArray();

        private string day4Input = @"";

    }
}
