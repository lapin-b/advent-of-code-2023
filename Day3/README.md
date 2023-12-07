# Advent of Code 2023, day 3

While the code seems to work on the example given in the puzzle instructions, it doesn't on the actual puzzle input. Since this particular puzzle has caused me a headache, I'm leaving it unfinished for now.

I might revisit this with a different approach or leave it as is.

[Link to the puzzle](https://adventofcode.com/2023/day/3)

## Summary 
The puzzle asks to fetch numbers adjacent to a symbol and, for the first part, sum them.

## Approach idea

The idea is to read the file like it was a "tape", that is the whole file on a single line and calculate the "coordinates" on the engine schematic on the fly. The file reading is a sort of state machine keeping track of read digits and adjacent symbols.