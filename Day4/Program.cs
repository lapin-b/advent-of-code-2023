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
        var cards_to_won_cards_count = scratchcards.Select(card => (card.CardId, Points: CalculateMatchingNumbersCount(card)))
            .ToDictionary(tuple => tuple.CardId, tuple => tuple.Points);

        var cards_workqueue = scratchcards.Select(card => card.CardId).ToList();
        var total_cards = 0;

        // Idea: for each scratchcard, we append the scratchcards we have won, increment the total_cards by one
        // and sort the list until we don't win any more cards
        while(cards_workqueue.Count > 0) {
            var current_card_id = cards_workqueue[0];
            cards_workqueue.RemoveAt(0);
            total_cards++;

            var cards_won_count = cards_to_won_cards_count[current_card_id];
            //Console.WriteLine($"Scratchcard {current_card_id} has won {cards_won_count} cards");
            for(int i = 1; i <= cards_won_count; i++) {
                cards_workqueue.Add(current_card_id + i);
            }

            cards_workqueue.Sort();
        }

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