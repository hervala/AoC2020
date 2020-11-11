using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    public abstract class DayCommand 
    {
        [CommandParameter(0, Description = "Part of day's puzzle to run")]
        public int Part { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var result = Part switch
            {
                1 => Part01(console),
                2 => Part02(console),
                _ => default,
            };
            return result;
        }

        public abstract ValueTask Part01(IConsole console);

        public abstract ValueTask Part02(IConsole console);
    }
}
