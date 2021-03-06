﻿using CliFx;
using CliFx.Attributes;
using Spectre.Console;
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
    [Command(nameof(Day11))]
    public class Day11 : DayCommand, ICommand
    {
        [CommandOption("visual", 'v', Description = "Render the seating evolutions.")]
        public bool RenderSeating { get; set; } = false;

        [CommandOption("delay", 'd', Description = "Frame delay (ms).")]
        public int RenderDelay { get; set; } = 500;

        private int frameCnt = 1;

        public override ValueTask Part01(IConsole console)
        {
            var seatMap = ParseInput(day11Input);
            var seatMapBuffer = seatMap.Select(s => s.ToArray()).ToArray();

            bool changeDetected;
            do
            {
                changeDetected = false;
                foreach (var y in Enumerable.Range(0, seatMap.Length))
                {
                    foreach (var x in Enumerable.Range(0, seatMap[y].Length))
                    {
                        if (seatMap[y][x] > -1) 
                        {
                            seatMapBuffer[y][x] = CalculateNeigborSeats(seatMap, y, x) switch
                            {
                                0 => 1,
                                >= 4 => 0,
                                _ => seatMap[y][x],
                            };
                            changeDetected = changeDetected || seatMap[y][x] != seatMapBuffer[y][x];
                        }
                    }
                }
                seatMap = seatMapBuffer.Select(s => s.ToArray()).ToArray();
                DrawSeatMap(seatMap, console);
            } while (changeDetected);

            var totalSeatsOccupied = seatMap.SelectMany(a => a).Where(i => i > 0).Count();

            console.Output.WriteLine($"{totalSeatsOccupied}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            var seatMap = ParseInput(day11Input);
            var seatMapBuffer = seatMap.Select(s => s.ToArray()).ToArray();

            bool changeDetected;
            do
            {
                changeDetected = false;
                foreach (var y in Enumerable.Range(0, seatMap.Length))
                {
                    foreach (var x in Enumerable.Range(0, seatMap[y].Length))
                    {
                        if (seatMap[y][x] > -1)
                        {
                            seatMapBuffer[y][x] = CalculateVisibleSeats(seatMap, y, x) switch
                            {
                                0 => 1,
                                >= 5 => 0,
                                _ => seatMap[y][x],
                            };
                            changeDetected = changeDetected || seatMap[y][x] != seatMapBuffer[y][x];
                        }
                    }
                }
                seatMap = seatMapBuffer.Select(s => s.ToArray()).ToArray();
                DrawSeatMap(seatMap, console);
            } while (changeDetected);

            var totalSeatsOccupied = seatMap.SelectMany(a => a).Where(i => i > 0).Count();

            console.Output.WriteLine($"{totalSeatsOccupied}");

            return default;
        }

        private void DrawSeatMap(int[][] seatMap, IConsole console)
        {
            if (!RenderSeating)
                return;

            frameCnt++;
            if (frameCnt % 2 == 0)
                return;

            var ansiConsole = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.Detect,
                ColorSystem = ColorSystemSupport.Detect,
                Out = console.Output
            });

            var canvas = new Canvas(seatMap[0].Length, seatMap.Length);

            foreach (var y in Enumerable.Range(0, seatMap.Length))
            {
                foreach (var x in Enumerable.Range(0, seatMap[y].Length))
                {
                    var color = seatMap[y][x] switch
                    {
                        1 => Color.Maroon,
                        0 => Color.DarkOliveGreen3_2,
                        _ => Color.Black,
                    };
                    canvas.SetPixel(x, y, color);
                }
            }

            ansiConsole.Clear(home: true);
            ansiConsole.Render(new Rule($"[yellow]{"Seating"}[/]").LeftAligned().RuleStyle("grey"));
            ansiConsole.WriteLine();
            ansiConsole.Render(canvas);
            Task.Delay(RenderDelay).Wait();
        }

        private int CalculateNeigborSeats(int[][] seatMap, int y, int x)
        {
            var sum = 0;
            for (int checkY = Max(0, y - 1); checkY <= Min(seatMap.Length - 1, y + 1); checkY++)
            {
                for (int checkX = Max(0, x - 1); checkX <= Min(seatMap[checkY].Length - 1, x + 1); checkX++)
                {
                    if (checkY != y || checkX != x)
                    {
                        sum += Max(0, seatMap[checkY][checkX]);
                    }
                }
            }
            return sum;
        }

        private int CalculateVisibleSeats(int[][] seatMap, int y, int x)
        {
            var sum = 0;
            int lookY;
            int lookX;

            // north
            lookY = y;
            lookX = x;
            lookY--;
            while (lookY >= 0)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookY--;
            }

            //north-east
            lookY = y;
            lookX = x;
            lookY--;
            lookX++;
            while (lookY >= 0 && lookX < seatMap[0].Length)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookY--;
                lookX++;
            }

            //east
            lookY = y;
            lookX = x;
            lookX++;
            while (lookX < seatMap[0].Length)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookX++;
            }

            //south-east
            lookY = y;
            lookX = x;
            lookY++;
            lookX++;
            while (lookY < seatMap.Length && lookX < seatMap[0].Length)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookY++;
                lookX++;
            }

            // south
            lookY = y;
            lookX = x;
            lookY++;
            while (lookY < seatMap.Length)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookY++;
            }

            //south-west
            lookY = y;
            lookX = x;
            lookY++;
            lookX--;
            while (lookY < seatMap.Length && lookX >= 0)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookY++;
                lookX--;
            }

            //west
            lookY = y;
            lookX = x;
            lookX--;
            while (lookX >= 0)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookX--;
            }

            //north-west
            lookY = y;
            lookX = x;
            lookY--;
            lookX--;
            while (lookY >= 0 && lookX >= 0)
            {
                if (seatMap[lookY][lookX] >= 0)
                {
                    sum += seatMap[lookY][lookX];
                    break;
                }
                lookY--;
                lookX--;
            }

            return sum;
        }

        private int[][] ParseInput(string input)
        {
            return input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Select(c =>
            {
                return c switch
                {
                    'L' => 0,
                    '#' => 1,
                    '.' => -1,
                    _ => throw new InvalidOperationException("illegal character: " + c)
                };
            }).ToArray()).ToArray();
        }

        private string day11Input = @"
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLL..LLLLLLLL.LLLLLLLL.LLLLLLLL..LLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLL..LLLLLL.LLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLL.LLLLLLLLLLLL.LLLLLLLLL.LLLL.LLL.LLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLL.LLLLLLLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.L.LLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
LLLLL.LLLL.LLLLLLLLL.LLLLL.LLLLLLLLLLLLLLL.LLL.LLLLLLLLLLLLL.LLL.LLLL.LLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LL.LLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLL.LLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL...LLLLLLLLLLL.
LL..LL..L..L.L...L.L..L.L....L.LL..L.L......LL.LL....LL..LL..LLLL.L.LLL.L.....LLL.LL........L.....
LLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLL.LLL..LL.LLLLLLL.LL.LLL.LLLL.LLLLLLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLL.LLLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLL.LLLLL.LL.LLLLLLL..LLLLLLLL.LL.LLLLLLLLLLLLLLLLLLLLL
LLLLLLLLL..LLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLLLL.LLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLL.L.LLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL
...........L.....L............LL..L...LL...L.LLL.L....L.L.L......L..L...LL......L.L.L.........LL..
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LL.LLLLLLLLLLLLLLLLL.LLLL.LLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLL.L.LLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL..LLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL
............L......L......L...L....L.....LLL....L.......L.....L.L..L....L..L.L....LL.....L....L.L.
LLLLLLLLLL..LLLLLLLLLLLLLL.LLLL.LLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLL.LL.LLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLL.LLLLL.LLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLL
LLLL.LLLLL.LLLLLL.LLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLLLLLL.L.LLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
.LL........L.......L....LL.L..L.L...L.LLLLLL.L..............L.L.........LL........L....L.LL.....L.
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLL.L.LLLLLLL.LLLLL.LLLLLLLL.LLLLL.LL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LL.LLLLLL.LLL.LLLLLLLLLLLLLLLLLLLLL.LLLL.LLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLL.LLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLL.
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLL.L.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
...L..L..L.LL.L.L.L......L.LLLL.L..L.L..L..L.L...L.LL.L...L....LL..L.L..L.L...........L.L..L..LL..
LLLLLL.LLL.LLLLLLLLL.LLLLLLLLL.L.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL
LLLLL.LLLL.LLL.LLLLL.LLLLL.LLLLLLLLLLLL.LLLLLL.L.LLLLLLLLLLLLLLL..LLLLLLL.LLLLLLLLL.LLLLLLLLLLLL.L
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLL.LL.LLLLLLLLLLLL.LLLLL.L.LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LL.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLL.LLLLL.LLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.L.LLL..LLL.LLL.LLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
...LL.L..L..........L......L.L..L.LL.......LL...L.LL..L.L..L.L......L..L......L..L..L..L.L........
LLLLLLL.LL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLL.LLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLL.L.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLL.LLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLL.LLL.LLL.L.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLL.LLLLLLLLLL
LLLLLLL.LLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLLLLLL.L.LLLLLLLL.LLLLLLL.LLLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
L.....L..LL.L.......L.L..L....L..L.L...L.LLLL.......L.L...L....LL.L...L.L.L.LL.L......L.L.L.L.L...
LLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLL..L.LLLLLLLLLLLLLLLLLLLLL
L.LLLLLLLL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLL..LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLLLLL..LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLL..LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL
.......L..L..L.L....LLLLLL..L..LL.LLL..LL..L..L...L...LL.....L..LL.LL..L.L..L.L..L.L.L....LL....L.
LLLLLLLLLLLLLLLL.LLL.LLLLL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLL.LLLL.LLLLLLLLL.L.LLLLLLLLLLLL
LLLLLLL.LL.LLLLLLLLLLLL.LL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLL.LLLLL.LL.LLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLLLLL.LL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLL..LLLLLLLLL.LLLLLLLL.LLLLL
.....L.L..L.......L.LL..........L...LLLL.LL..L...L.L.L.LLLL...L.....L...L.LLLLLL..L..........LL...
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL
LLLLLLLLLL..LLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLL.L.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLL
L.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLLL
..L.........L..L.L...L................LLL..L..LLL.L......L....L.L.....L.L.L.L.L..L.L.L.L....L....L
LLLLLLLLLL..LLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LL.LLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLL.
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLL..LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.L.LLLLLL.LLLLL
LLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLL.LLLLLLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLL.LLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLL..LLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLL
...LLL.....L..L.LL.L.L.LL.....L..LL.LLLL...L.L....LL.L...L..LL...L..LLL......LL..L..L.L..L.L...LL.
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLL.LLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LL.LLLLLLL.LLLLLLLLL.LLLL.LLLLLLLL.LLLLLLLL.LL.LLLLL.LLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLL.LLLLLLLL.LLLLLLLLLL.LLLLLLLLLLLLLLL.LL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLL.LLL.LLLL.LLLLLLLLL.LLLLLL.L.LLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLL..LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.L.LLLLLL.LLLLL
LL.LLLLLL..LLLLLLLLL.LLLLL.LLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLL..LLL.LLLLL.LLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL..LLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL
...L....L..L.L..L..L.LLLL....L..L.....L.....L.L..L.L.L...L.L.........L....L..L..L.LL.L...L.L...L..
LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLL..LLLLL.LL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLL.LLLL..LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL..LL.LLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLL.LL.LL
LLLLLLLLLL.LLLLLLLLL.LLLLL.LLLLL.L.LLLLL.LLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLL.LLL.LLLLLLLLLL.LLLLLLL.LLLLL
LLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLL..LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL..LLLLLLLL.LLLLLLLL.LLLLL
LLLLLL.L.L.LLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLL.LLLLL
L.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLL.LL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLL..LLLL
";

    }
}
