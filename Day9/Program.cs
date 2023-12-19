
class Program
{
    public static void Main(string[] args)
    {
        var filename = args.Length > 0 ? args[0] : "files/test.txt";
        var file_content = File.ReadAllLines(filename);

        var puzzle_input = file_content.Select(line => line.Split(' ').Select(i => Convert.ToInt32(i)).ToArray()).ToArray();
        var predictions = puzzle_input.Select(i => OASIS_Prediction(i)).ToArray();
        Console.WriteLine($"Part 1: extrapolated values {string.Join(' ', predictions)}");
        var predictions_sum = predictions.Sum();
        Console.WriteLine($"Part 1: sum {predictions_sum}");
    }

    private static int OASIS_Prediction(int[] input) {
        return OASIS_Prediction(input, 1);
    }

    private static int OASIS_Prediction(int[] input, int step)
    {
        Console.WriteLine($"Current step {step}: {string.Join(' ', input)}");
        // End of recursion clause: if all the input items are zero, we halt here by returning a zero
        if(input.All(item => item == 0)){
            Console.WriteLine($"Extrapolated step {step}: 0");
            return 0;
        }

        var initial_sequence = input.Zip(input[1..], (element, next_element) => new int[]{ element, next_element });
        var next_input = new List<int>(input.Length - 1);

        foreach(var item in initial_sequence) {
            next_input.Add(item[1] - item[0]);
        }

        var extrapolated_value_step_below = OASIS_Prediction(next_input.ToArray(), step + 1);
        var extrapolated_value_current_step = extrapolated_value_step_below + input[^1];
        Console.WriteLine($"Extrapolated step {step}: {extrapolated_value_current_step}");

        return extrapolated_value_current_step;
    }
}