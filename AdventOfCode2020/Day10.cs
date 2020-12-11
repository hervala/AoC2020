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
    [Command(nameof(Day10))]
    public class Day10 : DayCommand, ICommand
    {
        private ulong adapterCounter = 0;

        public override ValueTask Part01(IConsole console)
        {
            var joltAdapters = ParseInput(day10Input);
            var joltDifferenceCounter = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
            };

            var joltLevel = 0;

            foreach (var adapter in joltAdapters)
            {
                if (adapter > joltLevel && adapter <= (joltLevel + 3))
                {
                    joltDifferenceCounter[adapter - joltLevel]++;
                    joltLevel = adapter;
                }
            }

            joltDifferenceCounter[3]++;  // the devices inbuild adapter

            console.Output.WriteLine($"{joltDifferenceCounter[1] * joltDifferenceCounter[3]}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            //            day10Input = @"16
            //10
            //15
            //5
            //1
            //11
            //7
            //19
            //6
            //12
            //4";

//            day10Input = @"28
//33
//18
//42
//31
//14
//46
//20
//48
//47
//24
//23
//49
//45
//19
//38
//39
//11
//1
//32
//25
//35
//8
//17
//7
//9
//4
//2
//34
//10
//3";

            var joltAdapters = new[] { 0 }.Union(ParseInput(day10Input).ToArray()).ToArray();


            Recursive(joltAdapters, level: 0, console);  // THIS TAKES FOR EVER... RECURSION IS NOT WORKING WITH THIS. NEED TO THINK ABOUT THIS.

            console.Output.WriteLine($"{1}");
            return default;
        }

        private Dictionary<(int[], int), int> memoize = new();

        public void Recursive(int[] joltAdapters, int level, IConsole console)
        {
            var i = 1;
            foreach (var adapter in joltAdapters)
            {
                if (adapter > level && adapter <= level + 3)
                {
                    Recursive(joltAdapters[i..], adapter, console);
                }
                i++;
                if (i == 4)
                    break;
            }

            if (joltAdapters.Length == 0)
            {
                adapterCounter++;
                if (adapterCounter % 50_000_000 == 0)
                {
                    console.Output.WriteLine($"{DateTime.Now:G}: {adapterCounter}");
                }
            }
        }

        private SortedSet<int> ParseInput(string input)
        {
            var rows = input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s));
            return new SortedSet<int>(rows);
        }

        private string day10Input = @"97
62
23
32
51
19
98
26
90
134
73
151
116
76
6
94
113
127
119
44
115
50
143
150
86
91
36
104
131
101
38
66
46
96
54
70
8
30
1
108
69
139
24
29
77
124
107
14
137
16
140
80
68
25
31
59
45
126
148
67
13
125
53
57
41
47
35
145
120
12
37
5
110
138
130
2
63
83
22
79
52
7
95
58
149
123
89
109
15
144
114
9
78
";

    }
}
