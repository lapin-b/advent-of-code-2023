using System.Collections.Concurrent;
using System.Text.RegularExpressions;

class AlmanacMap
{
    private static Regex MAP_RESOURCES_REGEX = new(@"^(?<from_resource>[a-z]*)-to-(?<to_resource>[a-z]*) map:$");

    public string FromResource { get; private set; }
    public string ToResource { get; private set; }

    public List<MapRange> Ranges { get; private set; } = new();

    public ulong ConvertResourceLocation(ulong source_location)
    {
        var range_array = Ranges
            .Where(map_range => source_location >= map_range.SourceRangeStart && source_location < map_range.SourceRangeStart + map_range.RangeLength)
            .ToArray();

        ulong destination_value;
        if (range_array.Length > 0)
        {
            var range = range_array[0];

            var difference = source_location - range.SourceRangeStart;
            destination_value = range.DestinationRangeStart + difference;
            //Console.WriteLine($"{FromResource} -> {ToResource} {source_location} = {destination_value} (diff {difference}) ({range})");
        }
        else
        {
            destination_value = source_location;
            //Console.WriteLine($"{FromResource} -> {ToResource} {source_location} = no match, using source");
        }

        return destination_value;
    }

    public static AlmanacMap FromPuzzleInput(string map)
    {
        var map_items = map.Split("\n").ToList();
        var resources_matches = MAP_RESOURCES_REGEX.Match(map_items[0]);
        map_items.RemoveAt(0);

        var parsed_ranges = map_items.Select(ranges =>
        {
            var split_ranges = ranges.Split(" ");
            return new MapRange(Convert.ToUInt64(split_ranges[0]), Convert.ToUInt64(split_ranges[1]), Convert.ToUInt64(split_ranges[2]));
        });

        return new AlmanacMap()
        {
            FromResource = resources_matches.Groups["from_resource"].Value,
            ToResource = resources_matches.Groups["to_resource"].Value,
            Ranges = parsed_ranges.ToList()
        };
    }
}

record MapRange(ulong DestinationRangeStart, ulong SourceRangeStart, ulong RangeLength);

record SeedRange(ulong SeedStart, ulong SeedCount)
{
    public IEnumerable<ulong> EnumerateSeeds()
    {
        var seed_end = SeedStart + SeedCount;
        for (ulong i = SeedStart; i < seed_end; i++)
        {
            yield return i;
        }
    }
}

class Program
{
    public static void Main(string[] args)
    {
        var filename = args.Length > 0 ? args[0] : "files/input.txt";
        var file_content = File.ReadAllText(filename);

        var map_groups = file_content.Split("\n\n").ToList();
        var seeds_part_1 = map_groups[0].Split(":", StringSplitOptions.TrimEntries)[1].Split(" ").Select(n => Convert.ToUInt64(n)).ToArray();
        var seeds_part_2 = seeds_part_1.Chunk(2).Select((seed_group) => new SeedRange(seed_group[0], seed_group[1])).ToArray();

        map_groups.RemoveAt(0);

        var maps = map_groups.Select(group => AlmanacMap.FromPuzzleInput(group)).ToArray();

        // While I could go the extra mile and implement searching for the right map before looking up the range,
        // I'm going to consider the maps are in the right order because the puzzle input is in the correct order
        // and .NET lists preserve ordering.

        // Part 1: lowest seed location, each item is a seed
        var lowest_location_part1 = seeds_part_1.Select(seed =>
        {
            return maps.Aggregate(seed, (current_location, map) => map.ConvertResourceLocation(current_location));
        })
        .Min();

        Console.WriteLine($"Lowest seed location part 1: {lowest_location_part1}");

        var lowest_location_part2 = seeds_part_2
        .Select(seeds_range =>
        {
            var partitioner = Partitioner.Create(seeds_range.EnumerateSeeds(), EnumerablePartitionerOptions.NoBuffering);

            Console.Write($"Current seed: {seeds_range.SeedStart} -> {seeds_range.SeedStart + seeds_range.SeedCount - 1} ({seeds_range.SeedCount}). ");

            var seed_mimimum = partitioner.AsParallel()
                .AsUnordered()
                .Aggregate(
                    ulong.MaxValue,
                    (thread_minimum, seed) =>
                    {
                        var seed_location = maps.Aggregate(seed, (current_loc, map) => map.ConvertResourceLocation(current_loc));
                        return seed_location < thread_minimum ? seed_location : thread_minimum;
                    },
                    (all_threads_minimum, thread_minimum) => thread_minimum < all_threads_minimum ? thread_minimum : all_threads_minimum,
                    all_threads_minimum => all_threads_minimum
                );

            Console.WriteLine($"eed minimum: {seed_mimimum}");
            return seed_mimimum;
        })
        .Min();

        Console.WriteLine($"Lowest seed location part 2: {lowest_location_part2}");
    }
}