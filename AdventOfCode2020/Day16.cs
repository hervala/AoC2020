﻿using CliFx;
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
    [Command(nameof(Day16))]
    public class Day16 : DayCommand, ICommand
    {
        public override ValueTask Part01(IConsole console)
        {
            var (fields, _, nearbyTickets) = ParseInput(day16Input);
            List<int> invalidValues = new();

            foreach (var ticket in nearbyTickets)
            {
                invalidValues.AddRange(GetInvalidFieldValues(ticket, fields));
            }

            console.Output.WriteLine($"{invalidValues.Sum()}");
            return default;
        }

        public override ValueTask Part02(IConsole console)
        {
            var (fields, myTicket, nearbyTickets) = ParseInput(day16Input);
            List<Ticket> validTicket = GetValidTickets(fields, myTicket, nearbyTickets);

            UpdateFieldsPossiblePositions(fields, myTicket, validTicket);
            ReduceFieldPositionsToSingle(fields);
            ulong departureProduct = CalculateDepartureFieldsProduct(fields, myTicket);

            console.Output.WriteLine($"{departureProduct}");
            return default;
        }

        private static ulong CalculateDepartureFieldsProduct(Dictionary<string, TicketField> fields, Ticket myTicket)
        {
            var departureFields = fields.Values.Where(t => t.Name.StartsWith("departure")).Select(t => t.Positions.First());

            var departureProduct = 1UL;
            foreach (var field in departureFields)
            {
                departureProduct *= (ulong)myTicket.FieldValues[field];
            }

            return departureProduct;
        }

        private static void ReduceFieldPositionsToSingle(Dictionary<string, TicketField> fields)
        {
            while (fields.Values.Any(t => t.Positions.Count > 1))
            {
                foreach (var singlePosition in fields.Values.Where(t => t.Positions.Count == 1))
                {
                    var position = singlePosition.Positions.First();
                    foreach (var field in fields.Values.Where(t => t.Positions.Count > 1 && t.Positions.Contains(position)))
                    {
                        field.Positions.Remove(position);
                    }
                }
            }
        }

        private static void UpdateFieldsPossiblePositions(Dictionary<string, TicketField> fields, Ticket myTicket, List<Ticket> validTicket)
        {
            foreach (var field in fields.Values)
            {
                foreach (var fieldPosition in Enumerable.Range(0, myTicket.FieldValues.Count()))
                {
                    var valid = true;
                    foreach (var ticket in validTicket)
                    {
                        var val = ticket.FieldValues[fieldPosition];
                        var vald = field.Validate(ticket.FieldValues[fieldPosition]);
                        valid = valid && field.Validate(ticket.FieldValues[fieldPosition]);
                    }

                    if (valid)
                    {
                        field.Positions.Add(fieldPosition);
                        continue;
                    }
                }
            }
        }

        private List<Ticket> GetValidTickets(Dictionary<string, TicketField> fields, Ticket myTicket, IEnumerable<Ticket> nearbyTickets)
        {
            var validTicket = new List<Ticket>
            {
                myTicket
            };
            validTicket.AddRange(nearbyTickets.Where(ticket => GetInvalidFieldValues(ticket, fields).Count() == 0));
            return validTicket;
        }

        private IEnumerable<int> GetInvalidFieldValues(Ticket ticket, Dictionary<string, TicketField> fields)
        {
            var invalidValues = new List<int>();
            foreach (var fieldValue in ticket.FieldValues)
            {
                var valid = false;
                foreach (var field in fields.Values)
                {
                    valid = valid || field.Validate(fieldValue);
                }
                if (!valid)
                {
                    invalidValues.Add(fieldValue);
                }
            }

            return invalidValues;
        }

        private (Dictionary<string, TicketField> fields, Ticket myTicket, IEnumerable<Ticket> nearbyTickets) ParseInput(string input)
        {
            var fieldSection = true;
            var myTickerSection = false;
            var nearbyTicketSection = false;
            var fields = new Dictionary<string, TicketField>();
            Ticket myTicket = null;
            var nearbyTickets = new List<Ticket>();
            var fieldMatcher = new Regex(@"(?<field>[\w\s]+): (?<val1Min>\d+)-(?<val1Max>\d+) or (?<val2Min>\d+)-(?<val2Max>\d+)");

            var rows = input.Split(Environment.NewLine).Where(s => !string.IsNullOrWhiteSpace(s));

            foreach (var row in rows)
            {
                if (fieldSection)
                {
                    var fieldMatch = fieldMatcher.Match(row);
                    if (fieldMatch.Success)
                    {
                        var ticketField = new TicketField
                        {
                            Name = fieldMatch.Groups["field"].Value,
                        };

                        ticketField.AddValidator(new TicketField.MinMaxValidator
                        {
                            MinValue = int.Parse(fieldMatch.Groups["val1Min"].Value),
                            MaxValue = int.Parse(fieldMatch.Groups["val1Max"].Value),
                        });

                        ticketField.AddValidator(new TicketField.MinMaxValidator
                        {
                            MinValue = int.Parse(fieldMatch.Groups["val2Min"].Value),
                            MaxValue = int.Parse(fieldMatch.Groups["val2Max"].Value),
                        });

                        fields.Add(ticketField.Name, ticketField);
                    }
                    else if (row == "your ticket:")
                    {
                        fieldSection = false;
                        myTickerSection = true;
                        continue;
                    }
                }

                if (myTickerSection)
                {
                    if (row == "nearby tickets:")
                    {
                        myTickerSection = false;
                        nearbyTicketSection = true;
                        continue;
                    }
                    else
                    {
                        myTicket = new Ticket
                        {
                            FieldValues = row.Split(',').Select(s => int.Parse(s)).ToList(),
                        };
                    }
                }

                if (nearbyTicketSection)
                {
                    nearbyTickets.Add(new Ticket
                    {
                        FieldValues = row.Split(',').Select(s => int.Parse(s)).ToList(),
                    });
                }
            }

            return (fields, myTicket, nearbyTickets);
        }

        private class TicketField
        {
            public string Name { get; set; }

            public List<int> Positions { get; set; } = new();

            private readonly List<MinMaxValidator> _validators = new();

            public IEnumerable<MinMaxValidator> Validators => _validators.AsEnumerable();

            public void AddValidator(MinMaxValidator validator) => _validators.Add(validator);

            public bool Validate(int value)
            {
                var valid = false;
                foreach (var validator in _validators)
                {
                    valid = valid || (value >= validator.MinValue && value <= validator.MaxValue);
                }
                return valid;
            }

            public class MinMaxValidator
            {
                public int MinValue { get; set; }
                public int MaxValue { get; set; }
            }

        }

        private class Ticket
        {
            public List<int> FieldValues { get; set; }
        }

        private string day16Input = @"departure location: 44-401 or 415-965
departure station: 44-221 or 243-953
departure platform: 29-477 or 484-963
departure track: 43-110 or 126-951
departure date: 48-572 or 588-965
departure time: 48-702 or 719-955
arrival location: 35-336 or 358-960
arrival station: 47-442 or 449-955
arrival platform: 25-632 or 639-970
arrival track: 34-461 or 472-967
class: 41-211 or 217-959
duration: 29-500 or 519-969
price: 39-423 or 440-969
route: 50-264 or 282-958
row: 50-907 or 920-972
seat: 27-294 or 315-954
train: 29-813 or 827-962
type: 45-531 or 546-956
wagon: 29-283 or 292-957
zone: 45-518 or 525-974

your ticket:
89,139,79,151,97,67,71,53,59,149,127,131,103,109,137,73,101,83,61,107

nearby tickets:
749,494,864,530,921,599,370,550,323,202,821,99,783,496,90,828,65,605,725,745
729,731,400,774,600,84,645,661,730,486,582,870,207,640,844,567,326,592,390,664
658,564,639,901,754,589,352,373,102,677,54,949,596,316,93,648,109,676,499,416
986,839,859,58,564,246,790,723,499,264,590,494,646,676,686,725,77,454,657,142
203,527,234,99,933,695,771,142,864,702,875,758,876,359,741,605,668,52,376,886
872,457,109,217,162,786,805,788,168,684,937,672,828,632,943,108,336,769,804,911
93,741,890,99,797,63,379,479,530,366,694,564,901,83,107,935,451,609,892,676
678,669,797,595,133,189,745,647,876,198,106,540,139,137,632,64,893,882,161,595
862,452,827,261,765,759,779,854,905,675,923,792,402,78,567,547,836,659,383,209
393,388,779,772,773,632,930,117,569,315,557,547,156,358,486,488,256,610,200,557
361,554,903,675,738,327,940,157,78,169,865,134,903,601,396,449,636,171,75,830
71,730,592,803,783,795,750,82,184,76,371,449,577,646,154,572,88,796,170,569
903,657,828,324,696,101,170,134,740,62,142,572,670,904,613,597,529,577,617,210
808,930,554,163,892,151,536,692,833,245,84,243,51,80,854,725,81,104,189,728
747,746,386,701,484,365,147,321,188,135,51,378,620,562,381,610,999,368,129,673
838,678,649,257,882,135,456,422,729,51,443,328,147,840,101,374,727,264,751,315
842,829,864,744,528,688,58,812,500,773,392,768,157,868,76,625,474,977,318,755
859,107,878,611,900,663,89,651,754,874,886,901,526,171,530,595,832,316,5,657
390,857,368,372,916,897,781,133,442,473,256,72,903,107,890,689,459,487,499,887
927,129,880,72,258,134,139,592,385,678,56,731,843,271,656,700,244,318,777,423
615,795,667,134,851,652,264,138,828,620,593,759,841,668,810,875,722,425,921,76
933,207,813,172,259,771,88,919,292,937,697,655,553,843,149,731,845,883,812,127
905,845,205,244,571,643,859,140,877,783,152,903,321,198,208,640,342,948,381,415
802,682,630,932,255,99,293,561,672,554,786,181,617,362,773,403,845,69,850,361
904,616,264,626,768,246,792,569,78,389,272,401,164,325,329,590,572,660,365,260
134,192,686,170,70,864,926,466,625,831,250,692,837,867,896,648,380,719,140,748
940,83,755,325,768,728,853,874,71,544,759,850,599,879,612,666,872,191,449,245
749,92,837,618,640,588,700,94,945,687,545,551,797,69,531,94,853,65,625,256
841,862,744,416,752,373,334,74,555,203,806,292,325,910,495,920,165,451,732,455
753,150,618,832,837,316,330,877,811,88,151,363,649,921,869,734,725,982,830,938
777,327,67,78,741,755,894,491,812,597,554,208,625,602,530,293,735,361,337,741
669,488,594,867,57,71,137,500,809,55,923,557,741,264,169,329,862,460,462,149
316,527,264,557,979,760,733,779,647,555,759,722,244,421,260,771,872,658,452,129
767,472,591,930,985,260,694,251,457,834,785,797,719,775,161,258,747,592,665,484
590,88,451,374,158,255,526,666,568,102,760,144,253,189,788,823,741,391,106,332
318,390,321,878,572,105,936,776,480,321,85,457,87,834,204,774,630,394,106,205
796,128,136,947,562,747,858,737,417,778,488,621,134,676,941,810,882,356,728,589
653,803,896,691,75,897,804,267,256,619,765,813,906,253,134,939,440,382,421,854
874,54,328,776,789,395,700,723,149,832,868,57,983,855,90,376,700,762,496,614
334,50,770,488,811,702,185,505,790,91,458,248,248,105,920,690,152,90,257,528
848,244,252,485,571,646,150,168,746,474,694,799,398,615,156,936,921,157,275,895
422,772,942,894,188,130,319,472,726,807,911,71,336,453,687,568,848,473,165,152
374,792,642,335,730,864,787,782,185,797,899,83,203,822,685,191,906,757,264,661
631,935,473,628,813,854,251,208,992,528,601,206,562,528,679,220,52,648,673,886
51,803,571,104,615,67,446,777,371,938,139,186,675,254,676,921,940,71,442,934
164,566,249,174,442,191,764,602,848,445,254,936,907,52,358,693,60,261,607,83
564,489,148,556,292,252,888,947,818,184,190,450,77,840,631,161,190,74,948,64
909,558,385,165,612,627,896,245,622,72,193,778,172,615,667,179,750,907,761,749
662,860,853,361,76,321,362,201,834,402,456,738,187,831,774,368,933,390,140,926
559,609,179,922,866,70,918,548,141,89,622,699,94,194,650,557,248,739,736,165
178,167,581,374,387,358,90,746,895,173,873,293,792,251,855,651,162,254,666,750
644,567,865,793,668,387,596,547,923,597,263,738,199,408,161,366,778,557,105,856
758,67,598,617,139,418,783,385,775,129,176,995,882,244,597,457,167,415,730,880
811,947,495,802,322,243,90,994,859,929,872,365,562,423,786,132,614,69,183,135
364,245,320,368,927,925,882,644,757,286,939,103,591,376,217,53,797,646,53,881
652,647,192,378,187,476,846,604,2,205,249,363,377,531,162,593,528,907,382,607
204,163,282,165,787,257,763,894,153,805,808,347,192,50,176,255,192,887,861,932
364,262,987,687,898,201,643,590,663,649,607,840,323,68,624,450,936,59,145,170
610,334,55,632,253,887,530,164,536,627,176,594,944,85,531,152,378,251,460,593
732,185,142,451,67,147,55,261,252,551,641,644,146,124,475,454,315,394,143,101
88,684,865,126,833,472,596,144,204,930,690,206,368,898,319,282,322,443,60,655
927,796,671,620,879,673,780,660,554,686,721,843,283,648,878,832,488,486,815,159
776,384,750,72,806,571,72,666,858,119,772,495,642,454,876,186,456,609,219,847
109,568,440,91,525,72,145,546,593,741,939,273,795,597,654,51,152,186,83,110
978,553,399,455,264,615,192,172,359,564,85,196,194,829,150,669,783,766,74,72
731,924,442,846,142,196,922,753,586,782,896,838,595,829,76,318,608,624,732,562
398,554,693,8,631,885,492,839,318,154,758,315,52,358,799,878,625,609,221,253
901,682,376,616,726,982,589,185,774,134,832,737,385,330,169,61,679,844,382,204
608,644,667,591,784,490,942,663,795,904,247,57,918,850,588,378,475,196,928,863
364,639,155,131,51,546,294,62,774,674,199,179,217,688,246,586,460,901,455,858
392,371,473,554,858,109,138,184,910,164,856,548,947,851,671,899,696,896,141,835
605,52,715,563,611,693,937,185,888,782,888,183,358,696,141,420,91,472,254,495
564,322,942,658,862,58,99,638,384,182,834,547,170,220,491,173,792,794,941,907
756,755,151,699,564,917,623,656,220,659,362,855,84,729,201,197,777,805,733,561
831,557,438,938,209,612,904,84,376,556,780,458,155,690,847,132,531,217,880,875
85,797,601,127,653,143,419,593,106,603,387,640,438,894,134,657,422,390,454,525
450,928,169,731,865,940,673,455,742,719,189,667,694,322,377,613,877,986,204,842
677,597,841,805,370,178,320,192,864,398,737,887,799,597,75,252,139,735,309,797
755,76,672,883,366,807,333,220,194,217,921,730,884,806,743,103,856,914,834,810
362,696,193,741,835,258,564,382,100,318,720,154,949,256,939,697,413,246,441,392
103,790,625,495,176,855,254,834,174,209,315,798,662,882,163,144,276,880,797,334
763,194,897,659,617,177,193,732,423,899,844,472,591,498,838,567,226,702,399,126
336,863,566,800,171,843,569,366,925,92,330,433,81,923,749,861,382,149,667,945
67,858,598,772,207,838,663,447,733,90,664,293,921,855,529,938,131,169,648,415
540,180,599,105,783,173,904,179,262,155,393,730,674,555,788,882,80,668,765,940
90,781,932,850,144,845,791,800,773,829,109,624,836,602,104,5,77,721,71,81
494,361,249,837,946,684,568,431,376,294,744,500,294,796,64,189,486,935,855,787
493,87,932,945,570,395,334,128,526,362,174,565,924,848,418,496,94,704,132,526
157,208,177,133,356,563,142,811,907,654,151,644,186,442,949,727,157,198,697,862
701,848,177,735,497,946,799,583,171,895,750,622,944,75,927,875,907,874,209,61
664,868,808,372,186,152,666,72,869,781,721,256,261,250,882,276,293,722,562,664
476,456,906,613,897,252,785,747,167,818,170,101,397,804,667,256,561,831,552,61
898,719,201,761,781,319,822,795,677,264,564,205,96,832,767,196,219,561,899,797
552,158,909,332,888,571,189,76,872,451,813,639,77,70,838,856,181,847,758,756
889,370,678,938,880,390,552,836,195,836,769,726,666,446,598,592,457,323,202,565
940,936,149,844,648,485,55,350,137,876,793,331,282,191,82,247,672,417,939,63
922,83,166,840,63,673,106,650,175,164,907,146,361,702,829,647,717,396,135,71
59,692,263,247,736,136,147,851,322,800,457,921,495,436,382,154,204,658,646,847
665,422,70,451,780,752,185,461,55,901,163,64,947,545,561,933,662,132,924,85
726,829,366,903,165,392,19,733,394,617,672,335,179,684,133,400,259,688,455,885
136,572,173,930,845,787,422,93,930,70,799,94,319,943,151,343,753,595,185,423
89,294,250,423,556,946,550,856,784,546,590,62,549,882,798,839,100,476,445,460
554,175,170,500,251,474,862,617,317,243,145,680,737,252,804,797,249,157,512,568
60,388,976,732,529,247,368,374,748,639,589,146,937,796,700,832,832,929,652,385
76,75,275,210,616,602,422,752,498,655,420,686,255,801,804,209,71,246,454,648
686,101,594,800,493,371,774,685,315,140,902,625,713,949,152,655,130,461,395,883
527,767,594,196,756,590,194,852,731,172,791,675,610,12,667,834,591,808,391,683
202,494,608,776,750,94,596,647,185,133,643,927,382,606,140,827,664,372,482,762
594,683,902,147,674,607,95,440,929,574,555,813,71,358,181,760,372,325,568,763
922,196,344,762,701,460,252,453,360,668,857,750,799,207,85,452,789,166,369,204
859,838,906,650,728,773,774,317,870,244,887,805,803,927,373,232,219,754,530,925
109,198,846,474,773,220,691,696,359,875,172,102,453,926,477,813,297,257,778,205
172,206,745,875,769,246,484,907,387,221,54,443,749,110,453,655,760,898,776,558
655,384,949,419,723,196,257,198,778,355,162,893,70,642,719,920,936,616,282,104
258,868,176,12,93,101,882,498,363,725,854,842,907,218,496,526,627,66,646,850
419,245,496,155,158,535,160,64,648,744,780,871,746,547,947,769,376,138,529,529
133,837,850,218,361,827,924,186,896,913,884,829,423,947,140,647,551,925,188,595
148,249,481,358,97,722,893,169,840,250,659,97,654,729,92,558,399,362,831,632
197,209,599,16,195,848,926,363,564,155,877,606,798,906,568,770,555,552,128,588
198,670,692,663,654,185,252,604,320,830,187,919,949,192,884,383,66,247,701,67
167,136,85,529,453,945,91,789,219,74,387,947,760,217,748,629,579,644,874,647
62,753,781,319,805,862,100,140,634,395,692,867,648,614,88,497,161,292,668,691
828,203,568,764,786,61,684,884,270,69,359,728,488,92,208,853,489,485,194,801
723,179,137,50,17,250,397,791,895,655,885,652,923,846,871,938,649,143,720,371
163,248,323,650,852,851,601,863,560,728,600,581,245,195,833,126,374,379,649,496
838,4,728,779,550,942,732,396,59,592,811,894,600,679,61,58,947,317,571,757
657,689,903,589,283,174,793,132,303,605,565,887,372,87,641,588,610,621,840,109
104,91,605,74,985,159,943,318,764,180,132,165,896,803,557,752,875,684,686,880
275,460,894,188,158,133,645,333,564,397,495,723,252,94,611,104,204,649,599,931
889,565,106,799,90,486,740,321,422,207,616,653,472,729,854,148,487,90,636,109
106,190,883,221,591,771,938,936,373,105,143,199,697,460,366,66,827,179,290,569
754,493,138,389,168,785,676,526,61,476,197,669,848,481,841,810,245,856,661,211
495,250,555,450,95,613,262,208,738,735,735,327,187,144,187,623,688,278,293,595
500,792,385,319,765,89,559,838,504,861,782,667,884,935,65,940,440,329,477,797
219,320,645,531,195,53,188,807,887,589,858,876,346,547,141,394,401,488,560,218
659,607,205,908,461,628,179,769,563,391,109,659,899,605,140,812,936,323,939,657
789,766,320,364,684,563,372,146,290,631,882,931,204,316,897,832,160,80,262,60
550,641,668,564,381,377,864,728,191,56,364,668,383,880,485,688,113,51,789,549
327,456,127,699,432,855,867,632,763,324,807,608,86,62,721,103,195,612,527,830
799,441,569,590,131,692,834,600,133,738,715,59,737,248,500,131,328,642,937,698
362,748,527,788,740,855,54,822,171,755,859,860,451,877,136,737,415,396,612,905
362,477,135,648,693,743,390,488,370,693,18,105,382,55,885,156,936,333,699,570
855,282,179,327,102,346,643,661,942,321,52,105,629,330,96,167,724,924,188,383
801,132,734,855,262,143,675,869,759,847,126,531,448,107,854,102,166,678,217,888
53,560,810,327,91,816,416,658,328,160,398,160,400,547,146,263,206,59,696,859
551,558,601,490,756,783,931,367,104,395,701,294,614,202,593,912,858,561,103,623
161,931,767,864,127,208,192,866,413,884,71,457,758,183,750,555,246,766,557,737
190,320,649,547,873,688,922,401,108,616,921,646,50,881,135,500,789,154,534,283
677,329,623,739,73,762,935,166,791,246,930,870,835,261,671,596,821,732,196,330
395,65,788,593,940,491,399,889,208,431,171,177,873,744,944,841,485,948,82,886
528,525,102,799,395,856,90,714,449,87,845,74,151,600,193,68,559,683,282,765
803,860,839,809,924,437,197,592,292,207,671,765,676,802,677,721,92,208,681,946
854,191,527,767,849,323,743,203,56,268,245,836,933,888,71,882,868,595,786,784
812,797,776,486,173,936,929,75,748,330,170,175,556,401,415,655,481,813,860,632
665,830,626,833,191,895,607,567,650,674,151,807,323,234,422,251,127,593,614,883
834,571,58,159,871,592,652,285,493,903,696,866,765,809,366,218,367,749,722,182
560,990,759,750,359,758,805,442,612,768,597,127,721,695,144,799,88,329,151,65
905,13,146,806,932,160,94,851,336,135,56,674,493,184,205,142,400,475,450,251
842,390,68,589,727,782,54,69,355,619,871,731,630,368,191,568,494,883,419,376
141,382,325,646,729,975,659,923,927,548,719,897,168,486,243,920,490,324,931,138
75,167,101,763,560,659,398,259,559,109,23,745,127,164,200,65,204,52,832,248
741,921,331,658,564,106,86,719,687,377,628,390,827,152,184,334,177,152,325,20
609,862,603,432,769,460,390,256,699,790,173,557,644,221,639,645,889,834,861,566
870,766,610,370,906,568,933,734,735,320,851,882,659,338,417,642,630,81,55,664
632,811,938,130,680,848,601,570,182,129,22,54,650,325,163,640,871,101,66,55
807,394,883,827,610,789,376,903,600,721,389,835,931,854,764,157,200,599,241,473
752,327,411,420,185,322,455,650,401,319,453,546,87,749,740,377,364,475,204,364
905,148,294,530,571,83,13,172,152,667,842,100,943,892,110,932,591,68,132,65
525,737,322,319,492,126,246,794,848,381,641,177,368,753,555,90,202,258,112,652
283,834,888,792,640,725,883,198,65,479,797,948,921,391,719,59,550,867,848,71
552,201,456,178,128,683,186,769,119,735,420,196,734,682,138,158,51,617,795,797
687,253,561,610,933,794,551,93,801,88,82,560,176,585,691,139,778,422,257,68
263,721,763,128,82,650,854,996,455,696,598,86,601,669,391,82,159,63,840,386
437,589,719,400,875,65,700,130,632,83,735,756,531,592,748,947,882,863,106,808
557,362,785,832,811,384,755,212,907,455,623,164,837,132,154,400,881,797,858,138
768,639,18,494,362,644,152,616,782,873,662,477,790,600,676,680,145,591,107,922
695,143,929,658,488,498,389,651,697,641,645,85,389,789,178,701,700,161,582,676
484,528,843,106,109,196,867,189,843,743,795,776,281,415,127,719,393,320,900,376
423,761,843,550,601,73,838,811,625,572,866,814,89,387,801,398,71,170,866,454
608,777,292,898,97,195,939,827,755,110,110,667,104,610,128,258,538,103,677,531
256,386,895,417,871,831,811,133,494,175,843,606,945,244,603,656,359,543,571,749
665,641,808,264,922,767,61,163,208,739,737,106,255,334,253,857,456,555,433,775
594,875,841,98,686,110,17,647,330,110,243,936,377,488,785,806,60,699,733,98
387,746,899,255,764,358,906,697,231,920,76,497,556,924,906,92,167,177,184,596
550,197,629,73,315,209,863,879,317,596,209,190,738,358,551,261,84,245,934,338
866,942,294,588,336,370,933,322,721,208,876,558,813,692,666,718,259,691,791,488
846,144,86,794,103,84,651,792,396,860,806,693,536,602,604,735,558,599,85,868
207,294,600,176,93,902,794,648,77,233,178,152,783,678,98,262,830,803,764,160
150,140,72,189,611,394,74,702,622,566,108,878,159,423,620,480,552,107,753,560
623,594,373,333,746,218,425,126,732,294,850,743,188,495,75,693,698,697,785,129
326,664,561,498,498,851,684,572,745,561,378,94,781,705,606,384,691,781,570,257
570,556,807,758,892,530,330,68,939,158,930,220,782,105,625,173,564,13,488,876
392,164,456,886,759,84,805,600,424,846,940,764,330,164,564,791,734,865,398,733
871,394,655,858,928,921,699,922,683,459,139,691,666,669,759,572,863,820,719,390
748,317,484,136,688,591,735,806,549,793,425,562,853,807,786,806,655,473,853,859
264,552,846,450,55,627,67,412,838,488,894,927,801,688,565,206,654,626,725,895
933,66,452,907,198,165,800,846,247,453,725,401,248,759,58,144,986,258,799,923
208,61,748,72,564,767,50,106,333,596,375,595,161,390,54,188,844,478,611,547
58,723,124,797,760,683,735,596,801,807,566,933,67,793,84,790,84,756,763,601
144,647,292,149,690,94,142,786,172,940,209,867,769,615,720,219,918,593,220,140
190,628,883,834,549,932,925,323,200,849,792,419,564,287,56,164,605,606,365,806
688,852,609,619,492,396,725,332,714,762,665,572,456,889,929,647,882,767,160,803
807,251,127,624,949,811,136,98,530,634,531,689,877,893,201,897,766,396,904,690
86,753,415,839,54,601,211,171,568,564,250,800,800,217,456,746,589,90,788,433
158,386,857,900,668,986,89,179,850,779,484,253,316,368,595,589,475,933,246,833
60,397,165,423,671,601,727,651,554,705,422,157,809,639,156,643,127,166,812,99
881,109,741,134,892,835,689,741,179,661,139,743,292,548,440,439,441,762,148,691
68,773,247,334,103,569,182,597,207,450,840,736,802,737,765,681,829,631,405,878
494,652,248,367,679,753,891,150,166,853,800,771,689,588,104,924,425,155,768,897
921,895,748,863,380,732,733,787,50,797,628,381,803,372,55,85,901,926,708,372
824,380,665,259,400,748,251,605,449,197,149,756,737,282,663,423,639,887,489,935
322,798,809,622,733,842,202,564,67,13,397,375,752,371,691,90,201,721,145,935
71,4,552,853,785,885,449,101,77,921,932,875,626,866,655,805,282,597,723,920
139,920,896,903,317,569,493,331,219,568,868,593,148,164,803,564,186,408,765,854
283,992,390,610,85,177,640,152,613,419,250,846,889,67,387,931,191,887,283,848
878,282,665,244,639,85,893,800,734,688,348,58,785,416,771,929,558,726,873,837
925,79,76,127,602,877,442,902,416,799,741,660,474,846,678,445,659,813,441,528
353,920,388,842,565,420,697,783,421,589,162,855,528,206,750,188,630,449,662,811
667,751,875,903,754,166,277,832,719,488,394,171,592,931,143,923,873,881,461,188
378,317,814,891,197,452,761,827,152,903,603,245,383,200,169,623,185,673,217,925
245,62,368,693,165,127,421,55,126,854,246,261,841,360,910,260,769,79,199,68
996,795,872,682,795,829,360,682,856,737,141,184,949,484,763,330,420,259,883,451
387,742,785,777,644,358,178,885,741,840,559,332,166,137,992,365,320,661,454,390
144,623,170,735,493,695,149,646,146,832,170,74,251,925,52,572,8,156,262,899
73,645,853,369,330,180,99,604,168,324,450,873,983,940,776,594,604,493,83,770
660,896,623,456,643,158,331,179,447,131,186,775,694,193,95,942,698,865,610,58
795,78,443,108,319,531,73,681,741,380,931,51,568,422,449,947,82,323,699,594
387,218,536,934,668,421,171,126,180,147,166,73,368,656,683,681,460,173,151,194
165,494,893,570,101,762,262,185,930,788,81,332,151,199,499,476,746,862,912,937
697,53,695,728,806,807,808,78,372,619,794,260,822,809,628,375,774,59,902,599
590,856,326,378,147,799,681,664,109,849,623,721,92,206,148,881,753,978,870,316
132,560,812,498,196,745,382,899,380,495,243,747,639,697,854,948,661,903,617,539
148,850,368,652,639,173,69,145,699,987,852,549,330,812,855,386,618,322,792,853
62,813,336,329,744,139,801,539,556,178,632,889,259,891,181,52,527,654,177,319
379,877,218,361,809,904,745,804,109,840,283,149,792,366,83,60,443,210,843,62
378,751,655,451,690,639,366,646,784,5,335,453,165,99,670,166,494,376,592,760
651,106,779,997,748,334,190,774,667,929,632,672,812,830,938,728,888,931,456,848
186,383,399,130,937,188,180,162,527,942,252,321,146,631,565,869,288,611,359,700
927,549,186,889,172,928,691,568,321,657,890,294,610,644,679,532,263,874,726,132
548,722,526,649,376,949,459,305,187,834,668,372,398,696,610,93,476,473,777,756
162,324,143,474,648,631,487,223,252,201,358,378,333,905,70,316,787,666,159,929
";

    }
}