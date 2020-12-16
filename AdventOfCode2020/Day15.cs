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
    [Command(nameof(Day15))]
    public class Day15 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            console.Output.WriteLine($"{PlayGame(2020)}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            console.Output.WriteLine($"{PlayGame(30000000)}");
            return default;
        }

        public int PlayGame(int numberOfTurns)
        {
            var startingNumbers = ParseInput(day15Input);
            var turn = 0;
            var spokenNumbers = new Dictionary<int, SpokenNumber>();
            var previousNumber = 0;
            foreach (var number in startingNumbers)
            {
                turn++;
                if (!spokenNumbers.TryGetValue(number, out var spokenNumber))
                {
                    spokenNumber = new SpokenNumber();
                    spokenNumbers.Add(number, spokenNumber);
                }

                spokenNumber.AddTurn(turn);
                previousNumber = number;
            }

            var numberSpoken = 0;
            do
            {
                turn++;
                if (spokenNumbers.TryGetValue(previousNumber, out var previousSpokenNumber))
                {
                    numberSpoken = previousSpokenNumber.GetTurnsApart();
                    if (!spokenNumbers.TryGetValue(numberSpoken, out var spokenNumber))
                    {
                        spokenNumber = new SpokenNumber();
                        spokenNumbers.Add(numberSpoken, spokenNumber);
                    }

                    spokenNumber.AddTurn(turn);
                    previousNumber = numberSpoken;

                }
                else
                {
                    throw new Exception("should not!");
                }
            } while (turn < numberOfTurns);

            return numberSpoken;
        }

        private IEnumerable<int> ParseInput(string input)
        {
            return input.Split(',').Select(s => int.Parse(s));
        }

        public class SpokenNumber
        {
            public int PreviousTurn { get; set; }
            public int OtherTurns { get; set; }

            public SpokenNumber()
            {
                OtherTurns = new();
            }

            public void AddTurn(int turn)
            {
                if (PreviousTurn != 0)
                {
                    OtherTurns = PreviousTurn;
                }
                PreviousTurn = turn;
            }

            public int GetTurnsApart()
            {
                if (OtherTurns == 0)
                {
                    return 0;
                }
                return PreviousTurn - OtherTurns;
            }

        }

        private string day15Input = @"0,1,4,13,15,12,16";

    }
}
