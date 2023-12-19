
record ExtrapolatedValues(int Forward, int Backward);

class Program
{
    public static void Main(string[] args)
    {
        var filename = args.Length > 0 ? args[0] : "files/test.txt";
        var file_content = File.ReadAllLines(filename);

        var puzzle_input = file_content.Select(line => line.Split(' ').Select(i => Convert.ToInt32(i)).ToArray()).ToArray();
        var extrapolated_values = puzzle_input.Select(i => OASIS_Extrapolation(i)).ToArray();
        var extrapolated_forward_sum = extrapolated_values.Sum(v => v.Forward);
        var extrapolated_backward_sum = extrapolated_values.Sum(v => v.Backward);

        Console.WriteLine($"Forward extrapolated values sum (part 1): {extrapolated_forward_sum}");
        Console.WriteLine($"Backward extrapolated values sum (part 2): {extrapolated_backward_sum}");
    }

    private static ExtrapolatedValues OASIS_Extrapolation(int[] input) {
        return OASIS_Extrapolation(input, 1);
    }

    private static ExtrapolatedValues OASIS_Extrapolation(int[] input, int step)
    {
        Console.WriteLine($"Current step {step}: {string.Join(' ', input)}");
        // End of recursion clause: if all the input items are zero, we halt here by returning a zero
        if(input.All(item => item == 0)){
            Console.WriteLine($"Extrapolated step {step}: 0 0");
            return new ExtrapolatedValues(0, 0);
        }

        var initial_sequence = input.Zip(input[1..], (element, next_element) => new int[]{ element, next_element });
        var next_input = new List<int>(input.Length - 1);

        foreach(var item in initial_sequence) {
            next_input.Add(item[1] - item[0]);
        }

        var extrapolated_values_step_below = OASIS_Extrapolation(next_input.ToArray(), step + 1);
        var extrapolated_forward = extrapolated_values_step_below.Forward + input[^1];
        var extrapolated_backward = input[0] - extrapolated_values_step_below.Backward;
        var extrapolated_values = new ExtrapolatedValues(extrapolated_forward, extrapolated_backward);
        Console.WriteLine($"Extrapolated step {step}: {extrapolated_values}");

        return extrapolated_values;
    }
}