
using System.Text.Json;
using System.Text.RegularExpressions;

class Program
{
    private const string CUBE_RED = "red";
    private const string CUBE_GREEN = "green";
    private const string CUBE_BLUE = "blue";

    public static void Main(string[] args)
    {
        var input_filename = args.Length > 0 ? args[0] : "files/test.txt";
        var input_lines = File.ReadAllLines(input_filename);

        var cubes_regex = new Regex(@"(?<cubes_count>\d+) (?<cube_color>red|green|blue)");
        var games = input_lines.Select(line =>
        {
            var split_line = line.Split(':', 2);
            var game_id = Convert.ToInt32(split_line[0]["Game ".Length..]);
            // One entry: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            var game_rounds = split_line[1]
                .Split(';', StringSplitOptions.TrimEntries)
                .Select(round => cubes_regex
                    .Matches(round)
                    .Select(match => (Count: Convert.ToInt32(match.Groups["cubes_count"].Value), Color: match.Groups["cube_color"].Value))
                    .ToArray()
                )
                .ToArray();

            return (
                GameId: game_id,
                GameRounds: game_rounds
            );
        }).ToArray();

        // Part 1: What games are possible with 12 red, 13 green and 14 blue cubes
        var grouped_games_enumerable = games
            .Select(game =>
            {
                return (
                    game.GameId,
                    CubeCountsByColor: game.GameRounds
                        .SelectMany(round => round)
                        .GroupBy(round => round.Color)
                        .Select(round_groups => (round_groups.Key, Count: round_groups.Max(round => round.Count)))
                        .ToDictionary(tuple => tuple.Key, tuple => tuple.Count)
                );
            });

        var games_meeting_criteria_part1 = grouped_games_enumerable.Where(game =>
                game.CubeCountsByColor[CUBE_RED] <= 12 &&
                game.CubeCountsByColor[CUBE_GREEN] <= 13 &&
                game.CubeCountsByColor[CUBE_BLUE] <= 14)
            .Select(g => {
                Console.WriteLine($"Matching game ID: {g.GameId}: {JsonSerializer.Serialize(g.CubeCountsByColor)}");
                return g;
            })
            .Sum(g => g.GameId);

        Console.WriteLine($"Sum of games ID part 1: {games_meeting_criteria_part1}");

        // For each game, find the minimum of cubes that could have been present
        var games_power_total = grouped_games_enumerable.Select(
            game => game.CubeCountsByColor[CUBE_RED] * game.CubeCountsByColor[CUBE_GREEN] * game.CubeCountsByColor[CUBE_BLUE]
        ).Sum();

        Console.WriteLine($"Game power: {games_power_total}");
    }
}