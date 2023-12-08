
using System.Text.RegularExpressions;

record MapNode(string NodeName, string LeftNode, string RightNode);

class Program
{
    private static Regex NODE_PARSING_REGEX = new Regex(@"(?<start_node>[0-9A-Z]+) = \((?<node_left>[0-9A-Z]+), (?<node_right>[0-9A-Z]+)\)");

    public static void Main(string[] args)
    {
        var filename = args.Length > 0 ? args[0] : "files/p2_test.txt";
        var file_content = File.ReadAllLines(filename);

        var left_right_tape = file_content[0].ToCharArray();
        var nodes = file_content[2..].Select(line =>
        {
            var match = NODE_PARSING_REGEX.Match(line);
            return new MapNode(
                match.Groups["start_node"].Value,
                match.Groups["node_left"].Value,
                match.Groups["node_right"].Value
            );
        })
        .ToDictionary(node => node.NodeName);

        // Part 1
        var tape_step = 0;

        {
            var current_node = nodes["AAA"];
            do
            {
                var current_instruction = left_right_tape[tape_step % left_right_tape.Length];
                var next_node_name = current_instruction == 'L' ? current_node.LeftNode : current_node.RightNode;
                Console.WriteLine($"{tape_step} {current_node.NodeName} {current_instruction} => {next_node_name}");
                current_node = nodes[next_node_name];
                tape_step += 1;
            } while (current_node.NodeName != "ZZZ");
        }

        Console.WriteLine($"Part 1 of steps required: {tape_step} steps");

        // Part 2
        /*tape_step = 0;
        {
            var current_nodes = nodes.Where(pair => pair.Key.EndsWith("A")).Select(pair => pair.Value).ToArray();
            // Sanity check the puzzle input before going into the loop
            if (nodes.Where(pair => pair.Key.EndsWith("Z")).Count() != current_nodes.Length)
                throw new Exception("Sanity check: Count of nodes ending in A is not equal to the count of nodes ending in Z");

            do
            {
                var current_instruction = left_right_tape[tape_step % left_right_tape.Length];
                for(int i = 0; i < current_nodes.Length; i++) {
                    var current_node = current_nodes[i];
                    var next_node_name = current_instruction == 'L' ? current_node.LeftNode : current_node.RightNode;
                    current_nodes[i] = nodes[next_node_name];
                    //Console.WriteLine($"{tape_step} N{i + 1} {current_node.NodeName} {current_instruction} => {next_node_name}");
                }
                tape_step += 1;

                if(tape_step % 500000 == 0){
                    Console.WriteLine($"Pending: current step = {tape_step}");
                }
            } while (current_nodes.Where(node => node.NodeName.EndsWith("Z")).Count() != current_nodes.Length);
        }

        Console.WriteLine($"Part 2 of steps required: {tape_step} steps");*/
    }
}