using BenchmarkDotNet.Running;

internal static class Program
{
    private const string AppModeMessage = "Проект Benchmarks выполняется в режиме";

    private static void Main()
    {
        PrintBuildConfiguration();
        PrintSeparator();
        RunBenchmarks();
    }

    private static void PrintBuildConfiguration()
    {
#if DEBUG
        Console.WriteLine($"{AppModeMessage} DEBUG.");
#else
        Console.WriteLine($"{AppModeMessage} RELEASE.");
#endif
    }

    private static void PrintSeparator()
    {
        Console.WriteLine(new string('-', 30));
    }

    private static void RunBenchmarks()
    {
        BenchmarkRunner.Run<AddVertexBenchmarks>();
        BenchmarkRunner.Run<EdgeBenchmark>();
    }
}


