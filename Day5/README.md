# Advent of Code 2023, day 5

This day is about planting seeds on a seemingly huge field according to maps with ranges.

Since the integers are a bit bigger than your average integer, I settled on using `ulongs` to be sure there are no overflows, although one could get away with `uints` instead.

Part one was mostly straight forward. Part two required a bit of tinkering with paralellism. I've decided to process the seeds sequentially but the seed ranges in parallel. This allows the program to use pretty much all the CPU continuously no matter the range of the seed numbers.

In terms of timing, on my machine, the part two takes:
- Around five minutes in release profile without native code
- A bit more time (around 20 seconds) with native code (*AOT compilation* introduced in dotnet 7). Don't ask me how, I wanted to try.