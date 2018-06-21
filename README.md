# HAMT.NET
Hash Array Mapped Trie implementation for .NET

This is a work in progress. It contains HAMT and CHAMP implementations for .NET. In additional it has an evolution over CHAMP (Inlined CHAMP) that utilizes generics and value types and "inlines" node and key-value array into class layout. Paper will follow.

### Preliminary benchmark results:

`HAMT.NET` vs BCL's `System.Collections.Immutable.ImmutableDictionary<>` vs `ImTools.ImHashMap<>`

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
Frequency=2835937 Hz, Resolution=352.6171 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


```
|          Method |     Mean |     Error |    StdDev |   Gen 0 | Allocated |
|---------------- |---------:|----------:|----------:|--------:|----------:|
|        Contains | 41.49 ns | 0.4914 ns | 0.4596 ns |       - |       0 B |
|     ContainsBCL | 83.90 ns | 1.5870 ns | 1.8276 ns |       - |       0 B |
| ContainsImTools | 64.91 ns | 0.7391 ns | 0.6913 ns | 34.3018 |   72144 B |


|     Method |       Mean |     Error |    StdDev |    Gen 0 |   Gen 1 |  Allocated |
|----------- |-----------:|----------:|----------:|---------:|--------:|-----------:|
|        Add |   537.8 ns |  7.509 ns |  7.024 ns | 646.4844 |       - | 1324.77 KB |
|     AddBCL | 1,010.1 ns | 19.590 ns | 19.240 ns | 277.3438 |       - |  570.27 KB |
| AddImTools |   360.9 ns |  4.289 ns |  4.012 ns | 219.2383 | 49.8047 |  547.64 KB |
