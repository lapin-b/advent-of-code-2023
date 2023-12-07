using System.Text.RegularExpressions;

class AlmanacMap
{
    private static Regex MAP_RESOURCES_REGEX = new(@"^(?<from_resource>[a-z]*)-to-(?<to_resource>[a-z]*) map:$");

    public string FromResource { get; private set; }
    public string ToResource { get; private set; }

    public List<MapRange> Ranges { get; private set; } = new();

    public uint ConvertResourceLocation(uint source_location)
    {
        var range_array = Ranges
            .Where(map_range => source_location >= map_range.SourceRangeStart && source_location <= map_range.SourceRangeStart + map_range.RangeLength)
            .ToArray();

        uint destination_value;
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
            return new MapRange(Convert.ToUInt32(split_ranges[0]), Convert.ToUInt32(split_ranges[1]), Convert.ToUInt32(split_ranges[2]));
        });

        return new AlmanacMap()
        {
            FromResource = resources_matches.Groups["from_resource"].Value,
            ToResource = resources_matches.Groups["to_resource"].Value,
            Ranges = parsed_ranges.ToList()
        };
    }
}

record MapRange(uint DestinationRangeStart, uint SourceRangeStart, uint RangeLength);

record SeedRange(uint SeedStart, uint SeedCount)
{
    public IEnumerable<uint> EnumerateSeeds()
    {
        for (uint i = 0; i < SeedCount; i++)
        {
            yield return SeedStart + i;
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
        var seeds_part_1 = map_groups[0].Split(":", StringSplitOptions.TrimEntries)[1].Split(" ").Select(n => Convert.ToUInt32(n)).ToArray();
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

        var lowest_location_part2 = seeds_part_2.Aggregate(uint.MaxValue, (current_seed_minimum, seed_range) =>
        {
            var range_minimum = seed_range.EnumerateSeeds()
                .Aggregate(uint.MaxValue, (range_minimum, seed) =>
                {
                    var seed_location = maps.Aggregate(seed, (current_location, map) => map.ConvertResourceLocation(current_location));
                    return seed_location < range_minimum ? seed_location : range_minimum;
                });

            return range_minimum < current_seed_minimum ? range_minimum : current_seed_minimum;
        });

        Console.WriteLine($"Lowest seed location part 2: {lowest_location_part2}");
    }
}