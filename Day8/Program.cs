
using System.Text.RegularExpressions;

record MapNode(string NodeName, string LeftNode, string RightNode);

class Program {
    private static Regex NODE_PARSING_REGEX = new Regex(@"(?<start_node>[A-Z]+) = \((?<node_left>[A-Z]+), (?<node_right>[A-Z]+)\)");

    public static void Main(string[] args) {
        var filename = args.Length > 0 ? args[0] : "files/test.txt";
        var file_content = File.ReadAllLines(filename);

        var left_right_tape = file_content[0].ToCharArray();
        var nodes = file_content[2..].Select(line => {
            var match = NODE_PARSING_REGEX.Match(line);
            return new MapNode(
                match.Groups["start_node"].Value,
                match.Groups["node_left"].Value,
                match.Groups["node_right"].Value
            );
        })
        .ToDictionary(node => node.NodeName);

        var current_node = nodes["AAA"];
        var tape_step = 0;

        do {
            var current_instruction = left_right_tape[tape_step % left_right_tape.Length];
            var next_node_name = current_instruction == 'L' ? current_node.LeftNode : current_node.RightNode;
            Console.WriteLine($"{tape_step} {current_node.NodeName} {current_instruction} => {next_node_name}");
            current_node = nodes[next_node_name];
            tape_step += 1;
        } while(current_node.NodeName != "ZZZ");

        Console.WriteLine($"Part 1 of steps required: {tape_step} steps");
    }
}