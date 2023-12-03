using System.Text;

class EngineSchematic
{
    private char[] engine_map;
    private int map_valid_idx_h;
    private int map_valid_idx_v;

    public EngineSchematic(string schematic_content)
    {
        map_valid_idx_h = schematic_content.IndexOf('\n');
        map_valid_idx_v = schematic_content.Split('\n').Length - 1;
        engine_map = schematic_content.Replace("\n", string.Empty).ToCharArray();
    }

    public char CharAtCoordinates(int vertical, int horizontal)
    {
        if (vertical < 0 || horizontal < 0 || vertical > map_valid_idx_v || horizontal > map_valid_idx_h)
        {
            return '.';
        }

        return engine_map[TapeCoordinate(vertical, horizontal)];
    }

    public IEnumerable<((int, int), char)> Symbols()
    {
        for (int i = 0; i < engine_map.Length; i++)
        {
            var vertical_coordinate = i / (map_valid_idx_v + 1);
            var horizontal_coordinate = i % (map_valid_idx_v + 1);
            yield return ((vertical_coordinate, horizontal_coordinate), engine_map[i]);
        }
    }

    private int TapeCoordinate(int vertical, int horizontal) => vertical * map_valid_idx_h + horizontal;
}

class Program
{
    public static void Main(string[] args)
    {
        var input_filename = args.Length > 0 ? args[0] : "files/input.txt";
        var engine_schematic = new EngineSchematic(File.ReadAllText(input_filename).Trim());

        var schematic_part_numbers = new List<int>();

        // Engine schematic reader state machine variables
        var number_string_builder = new StringBuilder();
        var number_has_symbol_nearby = false;
        var symbol_nearby_coordinate_offsets = new (int, int)[] {
            (-1, -1),
            (-1, 0),
            (-1 , 1),
            (0, -1),
            (0, 1),
            (1, -1),
            (1, 0),
            (1, 1),
        };

        foreach (var (coordinate, symbol) in engine_schematic.Symbols())
        {
            var actual_symbol = engine_schematic.CharAtCoordinates(coordinate.Item1, coordinate.Item2);
            Console.WriteLine($"({coordinate.Item1}, {coordinate.Item2}) = {symbol} {actual_symbol}");
            if (symbol != actual_symbol)
            {
                throw new Exception("Symbol at coordinates don't match");
            }
            // If we have a digit, we append it to the builder of number then
            // check the surrounding characters for a symbol other than 0-9 and period.
            if (symbol >= '0' && symbol <= '9')
            {
                number_string_builder.Append(symbol);
                foreach (var (y_offset, x_offset) in symbol_nearby_coordinate_offsets)
                {
                    var symbol_at_offset = engine_schematic.CharAtCoordinates(coordinate.Item1 + y_offset, coordinate.Item2 + x_offset);
                    if (symbol_at_offset != '.' && (symbol_at_offset < '0' || symbol_at_offset > '9'))
                    {
                        number_has_symbol_nearby = true;
                    }
                }
                ;
            }
            else if (symbol == '.')
            {
                if (number_string_builder.Length > 0 && number_has_symbol_nearby)
                {
                    schematic_part_numbers.Add(Convert.ToInt32(number_string_builder.ToString()));
                    number_string_builder.Clear();
                    number_has_symbol_nearby = false;
                }
                else if (number_string_builder.Length > 0)
                {
                    number_string_builder.Clear();
                }
            }
        }

        // Part 1: Sum of engine parts
        var part1_sum = schematic_part_numbers.Sum();
        Console.WriteLine($"Part 1 schematic engine parts sum: {part1_sum}");
    }
}