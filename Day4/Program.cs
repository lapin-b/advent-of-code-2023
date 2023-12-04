using System.Text.RegularExpressions;

record Scratchcard(int CardId, HashSet<int> WinningNumbers, List<int> Numbers);

class Program
{
    private static Regex SCRATCH_CARD_REGEX = new(@"Card *(?<card_id>\d+):(?: *(?<winning_numbers>\d+) )+\| (?: *(?<available_numbers>\d+) ?)+");

    public static void Main(string[] args)
    {
        var input_filename = args.Length > 0 ? args[0] : "files/test.txt";
        var scratchcards = File.ReadAllLines(input_filename)
            .Select(line =>
            {
                var regex_match = SCRATCH_CARD_REGEX.Match(line);
                var card_id = Convert.ToInt32(regex_match.Groups["card_id"].Value);
                var winning_numbers = regex_match.Groups["winning_numbers"].Captures
                    .Select(cap => Convert.ToInt32(cap.Value))
                    .ToHashSet();

                var numbers = regex_match.Groups["available_numbers"].Captures
                    .Select(cap => Convert.ToInt32(cap.Value))
                    .ToList();

                return new Scratchcard(card_id, winning_numbers, numbers);
            })
            .ToList();

        // Part 1: How many points are the scratchcards worth in total ?
        var total_scratchcards_scores = scratchcards.Select(card =>
        {
            var points = card.Numbers.Aggregate(0, (points, number) => {
                if(!card.WinningNumbers.Contains(number)) {
                    return points;
                }

                return points == 0 ? 1 : points * 2;
            });

            return points;
        })
        .Sum();

        Console.WriteLine($"Part 1: Scratchcards total points is {total_scratchcards_scores}");
    }
}