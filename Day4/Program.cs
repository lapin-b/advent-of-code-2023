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
        var total_scratchcards_scores = scratchcards.Select(CalculateScratchcardPoints).Sum();

        Console.WriteLine($"Part 1: Scratchcards total points is {total_scratchcards_scores}");

        // Part 2: How many scratchcards do we end up with ?
        var cards_to_won_cards_count = scratchcards
            .Select(card => (card.CardId, Points: CalculateMatchingNumbersCount(card)))
            .ToDictionary(tuple => tuple.CardId, tuple => tuple.Points);

        var cards_buckets = scratchcards
            .Select(card => card.CardId)
            .ToDictionary(card => card, card => 1);

        for(int card_id = 1; card_id <= cards_buckets.Count; card_id++) {
            var cards_won = cards_to_won_cards_count[card_id];

            if(cards_won > 0) {
                Console.WriteLine($"Card ID {card_id} won {cards_won}, from ID {card_id + 1} to {card_id + cards_won}");
            } else {
                Console.WriteLine($"Card ID {card_id} won nothing");
            }

            for(int i = 1; i <= cards_won; i++) {
                cards_buckets[card_id + i] += cards_buckets[card_id];
            }
        }

        var total_cards = cards_buckets.Values.Sum();
        Console.WriteLine($"Part 2: We have a total of {total_cards} scratchcards in the pile");
    }

    private static int CalculateScratchcardPoints(Scratchcard card)
    {
        return card.Numbers.Aggregate(0, (points, number) =>
        {
            if (!card.WinningNumbers.Contains(number))
            {
                return points;
            }

            return points == 0 ? 1 : points * 2;
        });
    }

    private static int CalculateMatchingNumbersCount(Scratchcard card) {
        return card.Numbers.Select(number => card.WinningNumbers.Contains(number) ? 1 : 0).Sum();
    }
}