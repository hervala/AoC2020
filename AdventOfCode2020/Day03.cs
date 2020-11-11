using CliFx;
using CliFx.Attributes;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    [Command(nameof(Day03))]
    public class Day03 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            console.Output.WriteLine($"{nameof(Day03)}-{Part}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            console.Output.WriteLine($"{nameof(Day03)}-{Part}");
            return default;
        }
    }
}
