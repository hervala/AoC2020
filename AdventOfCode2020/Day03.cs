﻿using CliFx;
using CliFx.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    [Command(nameof(Day03))]
    public class Day03 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            var yPos = 0;
            var xPos = 0;
            var parsedInput = ParseInput(day3Input);
            var height = parsedInput.Length;
            var width = parsedInput[0].Length;
            var treeCount = 0;

            do
            {
                treeCount += parsedInput[yPos][xPos % width];
                yPos++;
                xPos += 3;

            } while (yPos <= height - 1);

            console.Output.WriteLine($"{treeCount}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            int yPos;
            int xPos;
            var parsedInput = ParseInput(day3Input);
            var height = parsedInput.Length;
            var width = parsedInput[0].Length;
            int treeCount;
            var slopes = new (int right, int down)[] 
            { 
                (right: 1, down: 1),
                (right: 3, down: 1),
                (right: 5, down: 1),
                (right: 7, down: 1),
                (right: 1, down: 2),
            };

            ulong total = 1;

            foreach (var slope in slopes)
            {
                treeCount = 0;
                yPos = 0;
                xPos = 0;
                do
                {
                    treeCount += parsedInput[yPos][xPos % width];
                    yPos += slope.down;
                    xPos += slope.right;

                } while (yPos <= height - 1);
                total *= (ulong)treeCount;
            }
            
            console.Output.WriteLine($"{total}");
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

        private string day3Input = @"........#....#..##..#...#.....#
...............#....##........#
.#....##...##..#...............
.#.......#......#.##..##...#...
.....#.#....#..##...#.....#....
...#.#..##...###......#..#..#.#
.....#..##........#.##......#..
..##.....###.........##........
..............##..#.#.#.#......
.#....##..#.##.#....#..#.#..#..
.#.#....#.##.#...#....#.....#..
..#...#.#.....#....#.......##..
.#.#..##.....#...........#.....
.#.##...#.....#......#.##......
..#..#..........#.....#..###.#.
##....##....#.#...........#..#.
.....#.#.......#.#.#..#.##....#
...##.#....#..#.....#.........#
.....#........#.##...#.........
.....#................#.#...#..
...#....##.....##....#.......#.
....##.#.....#.#.......#.......
#...............#..#...........
.......###.#.......#.##....#.#.
..#........###........#......#.
.#.......#...##.....####....##.
..##.#....#.....#..#....#......
..#...#..#.#..##...#.....#.....
.#.......###.......#....#......
...#...#.......#........#...#.#
..#....#...#.......#.#..##.....
##............#.#..#..........#
.......###...##..#.....#....#..
##..######.#..#.......###....##
###..#...#.##......##....#...#.
..............##.###..........#
.....#........##.#.###....#....
..#...#.....##.#......#.#..#.#.
#....#.............#.#.........
.........##.#........#...#.....
..........#..##.#.#.....#..##..
........##......#..#..#...#.#..
.##.......#..#.#...#.####..#...
##...#........#.###...##....#..
....###.####...#..#..#......###
#....#....#.#.....##.........#.
#.......#....#....##...........
##...##.#.......#....#...#....#
....#....#........##..#.#..#.#.
..##.....##...#..........#...#.
.#.#.#...#.....##..#........#..
#....#.....#..........#....#...
...##.#.......#.#.........#....
##.##.........##.....##.....##.
##.#..##..#...##........##.....
.........##.......#....#...#...
.#.....#........####.#.#.....#.
...........##..#.###...........
..#....##....#...#.............
#.#............#.......#.......
.##........#...#..##.....#.#...
#.##..............##..##.......
##.........#......#......#..#..
##.#....#...#....##....#..#.##.
......#...#..#.#...#.#....#.##.
##.......#.....#.........#.....
...##...#................#.#...
....#.####...#.#.....##....##.#
#...#..#.#.##................##
.........##.....##...#..#......
......####....#.##.#.....#.....
...#..#.#....#.#.#..#..........
.....#........##...#.##....#.#.
..##......#...................#
.....#..#...............#..#...
....#........#..#.#...##...#.##
..#.#.......#.##.........#...#.
...##......#.#.................
.#....#...#.............##.#...
........#.##...#..#...###.....#
#....#.#........##....#......##
.###.......#..#..........#..#..
#....#..#....#........#...#...#
##.#.###.##.#...##.#......#.#.#
#..#..#..##........#..###.#...#
....#..#..#.....#...##....#...#
.......##.......#..#.##...##..#
.##....#..###................##
#...#.##.##...#.##......##.....
...##.....##..##...#..#........
...............##.....##.......
.#..#.#..#....#.....#..#...#...
.#....#..#........#.#...#.....#
##.....####..#......#..........
........#.........#.........#..
#...####....#.##...#....#...##.
.#....####..#...##..#......####
...........##.##..#.##...##....
..#..#.......#.##....#.#...#.##
#...........#..#...............
.......#.##..#.....##......#...
....##.#.##.....#...........#.#
.............#.##..#...#......#
#......#...........#........#..
#.#..#.............#...#.......
#.........##...#....#...##.....
##...#..#..#..#....#...........
.#.....#........#.....#.##..#.#
...#..............##.####.#..#.
##.....#..#.#..#..##...........
...#...#.......#...............
..#..................###..#..##
....###..........#.#..#...#.#.#
..#..#..#.#..........#.#......#
....#....#.#...#.###...##..#...
....#.......#...#....##........
.#.....#.......###....#........
....#..#..#.....#......#.......
......#...#..#....#.#.......#..
.##.#..#...#.#.#...........#...
..#....##.#....#.#....#...#.#.#
...##..#.......#....#.#.....##.
##.#......#.#.......##...#.....
......#...#.##..............#..
.##.........#......##.#..#....#
#.......#.....#...##...#..#...#
..#..##.......#......#......##.
#..##...###.#.#...........#....
##......#.....####..#..#....#.#
.......##...##.#...#...........
....#..#.##.#.....#.#....#.#...
....#.....#.....####...#..#.##.
.##..#..#..###...#....#.##.#.#.
..#.#.##..........##...........
#.##.#.#....#.##....#..#...##.#
#...#....#...###....#.......#..
.......#..#............#.......
................##.#.#.....#..#
..........................#....
.##....##...#.#....####..#....#
......#...#....#...#.##..###.#.
.........#............#.......#
.#.#..#........#..#.........#..
#..#...#......#.#....#..#.#....
...........#.................#.
.#.#..#...##..###......##....##
.#.#.##......####.........##...
..#....#.#..#................#.
##.......#....#.........##.#.#.
##..#.###...........#..#.#..#.#
...#............##.#....#......
...#................##.#..#....
....#..##.#...#.#.....#.......#
......#......#.#........#..##..
...##...#.....#.##.......#.....
##...#...#.............#..#....
..#...##.....#..........#..#.##
#.##...#..................#.###
.........#..........#.###...#.#
#..#.....#.#.#....#......#...#.
.............#.##..###.....#.##
..#..#.....#..#.............#..
.#.....##.#.#..#.........#.....
..#.......#....#.....##.#......
.#.........#..#....##...#.##...
.##..##................###....#
.#..##..............#...#......
.#..............#.##....##.....
.#......#..#..##..#...###.....#
................##...#.#..#...#
##.#.......#...................
....#.#.......#..#.##..........
....###............##...#......
.......#....#.#.....##.#.....#.
....#...............#.#........
..#.##....#.#.#......##..#.....
.##......#...#.#..#..#.......#.
....#...#........#.#..##.......
.##...###.#....#..........##..#
..#.......##..#.....###......#.
...#.#..##.#.#...........#.....
##........#.#..##.........#..#.
.....###.......#..#.#.....##.#.
..#...##.#..............#......
......#...#...............##.#.
##...#..#....#...#.####.##.....
...#............#.##...........
...#........##.#.##.......#....
...#..#..##....#...#......#..#.
#.....#..#......#.#.....##.#.#.
.....#.##......#...#..#..###..#
...........##..##.#.#..........
...#........##........##..##.#.
......###...#.....#..###.#..#..
#.....#.#....#...##....##.....#
.##....#......#.....#.#..##.##.
##....###.......#...##.......##
...##......#....###............
..#...#...#.#..#..........##.#.
#.#.###...#..#.....#....#.###..
..##.....#.#.#.......#.........
...####..#....#..#.........#...
.##...........#.##.#...#.#.##..
...#.#....#.##......#........#.
##....##....#..#...#..#.#......
#......#..#...#...#.#.#.#.####.
....##.#.#.....#.###........#..
....##..#.#.#.#....#....#.#..#.
..#.###..#............##..#.#..
...#...#..#...#.#.#.....#.....#
..........#.....#..#.......##.#
..............##...........#...
.......#.....#...#.....#.....#.
.#.###.....##......##....#...#.
.....#.........#.#....#........
..#.#....#.##...#.##....##...#.
...#......#.#.....#.......###..
#.##....##.....#.#.#...#......#
#..#...#..........#.........##.
....#.#.#.#.....#...###........
#.#..#...#......#...#.##...####
.#...#......#....##...#........
..#.........#............#...#.
##......#..#...#....#.##....#..
.#...##..#..##.#.#.#........#.#
.##.........###...#......#..###
...##.....##..#.#.........#....
...........##........#...#.....
..##..#...#..#..#.....#......#.
..#..#.#....#.....#..#.##...#..
#....#........##..........#.###
......#...#...#....#...##.#....
...#......#.#.....##......##...
#....#..##............#....#.#.
...#...##.#..........##........
......#.###......#...#.#.......
..................#.##..#..#..#
....#.....#...#.....#...#....#.
.#....##.#..#..#.....###.##...#
#.......#..#....##.##.#.....##.
..##........##...#.....#....##.
#.........#...........##.#.....
.#....#.#...##..###..........#.
....##..##....####...#......#..
##.##..#..#....#....####...#...
..#...............#.##.........
...#.#....#..#....#......#.....
.#..#...#........#...#.....##..
#.....###.......#.....#........
...#.##..#.......#....#........
....##..###.##...#.#....#.#....
#.####...#.......#.....#.#....#
#.......#......#.......#.#.#...
##....#......#..#...#..#..####.
.##.....#........#..#...#......
#.#.#....#....#...#.##..##.....
....#..#.........###.##.##.....
...##...##.###..#..##.....#.###
..###.......................#..
......##..#.#.........#......#.
.###......##....#.....#.......#
.....#..#..##........#......##.
..##.....#....#.#.............#
..##.........##.#..#.........##
......#......#.#......#........
.#...#..#......##...#..#....#..
...............###............#
#.####.#....#...#...........#.#
............................#.#
.#..#...#.#.#.###..##.....##...
....##...#.................##..
......##....#...............##.
.#......#.##.#..#.....##...##..
.............#........#......#.
#..........#.#....#####.#...#.#
.#.#...##..#.#...#.#..#.#..#...
#.##.......##......#.#.#....#..
##.....##.#.#.##..........##..#
....##..#.#.......#....#.##....
..#.#.#...#.....#.......#......
.#....#..#...........###.......
#.#...#.....#......#...#.....#.
#........#.#..........#...#.#..
...#...#....#.........#........
.....................#..##.....
...#......##........#.##.#.#.##
.............###...#.#...#..#..
.#..##........##....#...###..##
.#..#.#...............#.....##.
...........##.#....#..##.#....#
.##.#.#..#.#..#...#.#.#..#.#.##
.......#.#..#..#..#..#...#.....
.#......##............#.#..#...
..#...#..##..#..#...##......#..
...##......##....#............#
.......#.....##...##.#...#..#..
......#.......#..##.........#..
..#...#.#.....#.#.......#.#...#
.#......##.##.#.#.#.##..#....##
#.....#.........#.#....#....##.
.......#.........#....#..#.#.##
.....##....#..#.#.#...#.....##.
#####.#.......######......#....
..##.#.......#.#..............#
..#.##....#.....#...#.#...##...
.....#...#..#....#.#..#........
.#....#.#..#.#.#.##..#.......#.
....#..#..#..........##...#....
.......#.#......#........#.....
##.#.#.###....##.#..#..#....#..
#.##......#..#.......#.#...#...
..##...#.......#.......#...#...
........##.........#.#....#.#..
..#...#..##.#.#.#...#....#.....
.###......#........#....#...#..
.#.......##......###..##.......
#....#.#....#.##.........####..
......#..........#..##.....#...
.............#......#..##.#....
...................#....#...#..
.#..........#...#.#..##...#....
.....#...#..........##.##......
#...#..#.##........#...#.......
";

    }
}
