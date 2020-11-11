using CliFx;
using CliFx.Attributes;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    [Command(nameof(Day02))]
    public class Day02 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            console.Output.WriteLine($"{nameof(Day02)}-{Part}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            console.Output.WriteLine($"{nameof(Day02)}-{Part}");
            return default;
        }
    }
}