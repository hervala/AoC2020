using CliFx;
using CliFx.Attributes;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace AdventOfCode2020
{
    [Command(nameof(Day01))]
    public class Day01 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            var input = day1Input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s));

            int result = 0;
            bool found = false;

            foreach (var num1 in input)
            {
                foreach (var num2 in input)
                {
                    if (num1 + num2 == 2020)
                    {
                        result = num1 * num2;
                        found = true;
                        break;
                    }
                        
                }
                if (found)
                {
                    break;
                }
            }

            console.Output.WriteLine($"{result}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            // example input
            // var input = new int[] { 1721, 979, 366, 299, 675, 1456 };
            var input = day1Input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s));

            int result = 0;
            bool found = false;

            foreach (var num1 in input)
            {
                foreach (var num2 in input)
                {
                    foreach (var num3 in input)
                    {
                        if (num1 + num2 + num3 == 2020)
                        {
                            result = num1 * num2 * num3;
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }

                }
                if (found)
                {
                    break;
                }
            }

            console.Output.WriteLine($"{result}");
            return default;
        }

        private string day1Input = @"2004
1867
1923
1819
1940
1675
1992
1728
2006
1578
1630
1893
1910
1509
1569
1967
1917
1922
1919
1813
1870
370
1617
1600
1729
503
1856
1842
1990
1605
1931
1827
1618
1727
1920
1802
1523
1797
1816
1962
1748
1946
1714
1863
1559
1866
1894
1646
1720
1787
1519
1765
562
1823
1639
1697
544
1938
1681
1477
1778
1718
1853
1632
1651
1694
1683
1911
1692
1997
1745
1873
1750
1795
1924
1724
1596
1726
1979
1869
1740
1847
1951
1541
1755
1991
1680
1612
1903
1691
422
1508
1665
1948
1707
1773
1861
1954
2005
1808
1904
543
1678
2001
1688
1855
1258
1695
1877
1554
1568
1771
1857
1597
1738
577
2010
604
1655
1644
1671
1281
1777
1690
1702
1949
1679
1862
1525
1789
1959
1595
1641
1829
1941
1854
1619
1706
1530
1828
1926
1577
1614
1963
1935
1627
1607
1769
111
1647
1716
1696
1868
1021
1906
1575
1905
1668
1758
1915
1892
1663
2003
1943
1742
1883
1576
1510
1546
1734
814
1367
1902
1698
1912
1818
1615
1851
1564
1719
1952
1616
1988
1768
1957
1744
1858
1705
1794
1944
1973
1960
1887
1804
1913
1770
1825
1737
1799
1532
";
    }
}

//---Day 1: Report Repair ---

//After saving Christmas five years in a row, you've decided to take a vacation at a nice resort on a tropical island. Surely, Christmas will go on without you.

//The tropical island has its own currency and is entirely cash-only. The gold coins used there have a little picture of a starfish; the locals just call them stars. None of the currency exchanges seem to have heard of them, but somehow, you'll need to find fifty of these coins by the time you arrive so you can pay the deposit on your room.

//To save your vacation, you need to get all fifty stars by December 25th.

//Collect stars by solving puzzles. Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first. Each puzzle grants one star. Good luck!

//Before you leave, the Elves in accounting just need you to fix your expense report (your puzzle input); apparently, something isn't quite adding up.

//Specifically, they need you to find the two entries that sum to 2020 and then multiply those two numbers together.

//For example, suppose your expense report contained the following:

//1721
//979
//366
//299
//675
//1456

//In this list, the two entries that sum to 2020 are 1721 and 299. Multiplying them together produces 1721 * 299 = 514579, so the correct answer is 514579.

//Of course, your expense report is much larger. Find the two entries that sum to 2020; what do you get if you multiply them together?

//Your puzzle answer was 802011.
//--- Part Two ---

//The Elves in accounting are thankful for your help; one of them even offers you a starfish coin they had left over from a past vacation.They offer you a second one if you can find three numbers in your expense report that meet the same criteria.

//Using the above example again, the three entries that sum to 2020 are 979, 366, and 675. Multiplying them together produces the answer, 241861950.

//In your expense report, what is the product of the three entries that sum to 2020?

//Your puzzle answer was 248607374.

//Both parts of this puzzle are complete! They provide two gold stars: **