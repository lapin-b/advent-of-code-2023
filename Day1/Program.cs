
using System.Text.RegularExpressions;

class Program
{
    public static void Main(string[] args)
    {
        var input_file = args.Length > 0 ? args[0] : "files/test.txt";
        var input_content = File.ReadAllLines(input_file);

        // Part 1: Determine where the numbers from the start of the line
        // and from the end.

        // Part 2: There's extra processing to be done, namely count spelled
        // out digits as valid digits

        var replacements = new (string, string)[] {
            ("one", "1"),
            ("two", "2"),
            ("three", "3"),
            ("four", "4"),
            ("five", "5"),
            ("six", "6"),
            ("seven", "7"),
            ("eight", "8"),
            ("nine", "9"),
        }.ToDictionary(t => t.Item1, t => t.Item2);

        // The positive lookahead operator allows us to count overlapping spelled out digits as
        // valid
        var line_correction_regex = new Regex(@"(?=(one|two|three|four|five|six|seven|eight|nine))");
        var calibration_values = input_content.Select((line) =>
        {
            var corrected_line = line_correction_regex.Replace(line, match => replacements[match.Groups[1].Value]);
            Console.WriteLine($"{line} => {corrected_line}");

            // For part 1 result, change the variable `corrected_line` to `line`
            var numbers = corrected_line.ToCharArray()
                .Where(c => c >= '0' && c <= '9')
                .Select(c => Convert.ToInt32(c.ToString()))
                .ToArray();

            var calibration_value = numbers[0] * 10 + numbers[^1];
            return calibration_value;
        });

        var calibration_values_sum = calibration_values.Sum();
        Console.WriteLine($"Sum of calibration values is {calibration_values_sum}");
    }
}