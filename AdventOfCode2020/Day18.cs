using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Math;

namespace AdventOfCode2020
{
    [Command(nameof(Day18))]
    public class Day18 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            // 1 + (1 + 2 * (2 + 2))

            // 1 + 2 + 3 + 4

            // 2 * 1 + 2 * 4

            var calc = new Add(
                    new Literal(1),
                        new Nested(
                            new Multiply(
                                new Add(
                                    new Literal(1),
                                    new Literal(2)),
                                new Nested(
                                    new Add(
                                        new Literal(2),
                                        new Literal(2)
                                    )
                                )
                            )
                        )
                    );

            var calc2 = new Add(new Literal(1), new Add(new Literal(2), new Add(new Literal(3), new Literal(4))));

            var calc3 = new Add(new Add(new Add(new Literal(1), new Literal(2)), new Literal(3)), new Literal(4));

            console.Output.WriteLine(Eval(calc));

            console.Output.WriteLine(Eval(calc2));

            console.Output.WriteLine(Eval(calc3));

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

        private int Eval(Expr expr)
        {
            return expr switch
            {
                Literal(int i) => i,
                Add(Expr left, Expr right) => Eval(left) + Eval(right),
                Multiply(Expr left, Expr right) => Eval(left) * Eval(right),
                Nested(Expr nested) => Eval(nested),
                _ => throw new Exception("huuu"),
            };
        }

        public abstract record Expr;
        public record Literal(int value) : Expr;
        public record Add(Expr left, Expr right) : Expr;
        public record Multiply(Expr left, Expr right) : Expr;
        public record Nested(Expr nested) : Expr;

        private string day19Input = @"";

    }
}
