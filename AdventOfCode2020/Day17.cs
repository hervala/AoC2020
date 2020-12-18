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
    [Command(nameof(Day17))]
    public class Day17 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            var (cubes, dimX, dimY, dimZ) = ParseInput(day17Input);
            HashSet<Point> cubesBuffer;
            var cycle = 0;

            do
            {
                cycle++;
                console.Output.WriteLine($"cycle: {cycle}");
                cubesBuffer = new();
                for (int iz = dimZ.min - 1; iz <= dimZ.max + 1; iz++)
                {
                    for (int ix = dimX.min - 1; ix <= dimX.max + 1; ix++)
                    {
                        for (int iy = dimY.min - 1; iy <= dimY.max + 1; iy++)
                        {
                            var point = new Point(ix, iy, iz);
                            var active = cubes.Contains(point);
                            var neigborCnt = cubes.Where(p => p.In(point.Neighbours())).Count();
                            if (active && (neigborCnt < 2 || neigborCnt > 3))
                            {
                                //becomes inactive
                                cubesBuffer.Remove(point);
                            }
                            else if (active)
                            {
                                cubesBuffer.Add(point);
                            }
                            else if (!active && neigborCnt == 3)
                            {
                                // becomes active
                                cubesBuffer.Add(point);
                            }
                        }
                    }
                }

                dimX.min--;
                dimX.max++;
                dimY.min--;
                dimY.max++;
                dimZ.min--;
                dimZ.max++;

                cubes = new HashSet<Point>(cubesBuffer);
            } while (cycle < 6);


            console.Output.WriteLine($"{cubes.Count()}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            var (cubes, dimX, dimY, dimZ, dimW) = ParseInput4D(day17Input);
            HashSet<Point4D> cubesBuffer;
            var cycle = 0;

            do
            {
                cycle++;
                console.Output.WriteLine($"cycle: {cycle}");
                cubesBuffer = new();

                for (int iw = dimW.min - 1; iw <= dimW.max + 1; iw++)
                {
                    for (int iz = dimZ.min - 1; iz <= dimZ.max + 1; iz++)
                    {
                        for (int ix = dimX.min - 1; ix <= dimX.max + 1; ix++)
                        {
                            for (int iy = dimY.min - 1; iy <= dimY.max + 1; iy++)
                            {
                                var point = new Point4D(ix, iy, iz, iw);
                                var active = cubes.Contains(point);
                                var neigborCnt = cubes.Where(p => p.In(point.Neighbours())).Count();
                                if (active && (neigborCnt < 2 || neigborCnt > 3))
                                {
                                    //becomes inactive
                                    cubesBuffer.Remove(point);
                                }
                                else if (active)
                                {
                                    cubesBuffer.Add(point);
                                }
                                else if (!active && neigborCnt == 3)
                                {
                                    // becomes active
                                    cubesBuffer.Add(point);
                                }
                            }
                        }
                    }
                }

                dimX.min--;
                dimX.max++;
                dimY.min--;
                dimY.max++;
                dimZ.min--;
                dimZ.max++;
                dimW.min--;
                dimW.max++;

                cubes = new HashSet<Point4D>(cubesBuffer);
            } while (cycle < 6);


            console.Output.WriteLine($"{cubes.Count()}");
            return default;
        }

        private (HashSet<Point> cubes, (int min, int max) dimX, (int min, int max) dimY, (int min, int max) dimZ) ParseInput(string input)
        {
            var x = 0;
            var y = 0;
            var z = 0;
            var points = new HashSet<Point>();
            var rows = input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var row in rows)
            {
                foreach (var ix in Enumerable.Range(0, row.Length))
                {
                    if(row[ix] == '#')
                    {
                        points.Add(new Point(ix, y, z));
                    }
                }
                y++;
            }
            return (points, (0, rows.First().Length), (0, rows.Count()), (0, 0));
        }

        private (HashSet<Point4D> cubes, (int min, int max) dimX, (int min, int max) dimY, (int min, int max) dimZ, (int min, int max) dimW) ParseInput4D(string input)
        {
            var x = 0;
            var y = 0;
            var z = 0;
            var w = 0;
            var points = new HashSet<Point4D>();
            var rows = input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var row in rows)
            {
                foreach (var ix in Enumerable.Range(0, row.Length))
                {
                    if (row[ix] == '#')
                    {
                        points.Add(new Point4D(ix, y, z, w));
                    }
                }
                y++;
            }
            return (points, (0, rows.First().Length), (0, rows.Count()), (0, 0), (0, 0));
        }

        public record Point(int x, int y, int z)
        {
            public bool In(IEnumerable<Point> points) => points.Any(p => p.Equals(this));

            public IEnumerable<Point> Neighbours()
            {
                List<Point> points = new();
                foreach (var iz in Enumerable.Range(z - 1, 3))
                {
                    foreach (var ix in Enumerable.Range(x - 1, 3))
                    {
                        foreach (var iy in Enumerable.Range(y - 1, 3))
                        {
                            var point = new Point(ix, iy, iz);
                            if (point != this)
                            {
                                points.Add(point);
                            }
                        }
                    }
                }
                return points;
            }
        }

        public record Point4D(int x, int y, int z, int w)
        {
            public bool In(IEnumerable<Point4D> points) => points.Any(p => p.Equals(this));

            public IEnumerable<Point4D> Neighbours()
            {
                List<Point4D> points = new();

                foreach (var iw in Enumerable.Range(w - 1, 3))
                {
                    foreach (var iz in Enumerable.Range(z - 1, 3))
                    {
                        foreach (var ix in Enumerable.Range(x - 1, 3))
                        {
                            foreach (var iy in Enumerable.Range(y - 1, 3))
                            {
                                var point = new Point4D(ix, iy, iz, iw);
                                if (point != this)
                                {
                                    points.Add(point);
                                }
                            }
                        }
                    }
                }

                return points;
            }
        }


        private string day17Input = @"
#.#..###
.#....##
.###...#
..####..
....###.
##.#.#.#
..#..##.
#.....##
";

    }
}
