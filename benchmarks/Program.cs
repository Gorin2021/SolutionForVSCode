using BenchmarkDotNet.Running;

internal static class Program
{
    private const string AppModeMessage = "Проект Benchmarks выполняется в режиме";

    public static void Main(string[] args)
    {
        PrintBuildConfiguration();

        Console.WriteLine(new string('-', 30));
        if (args.Length > 0)
        {
            foreach (var benchmarkName in args)
            {
                Console.WriteLine($"Запуск бенчмарка: {benchmarkName}");
                RunOneBenchmarks(benchmarkName);
                PrintSeparator();
            }
        }
        else
        {
            Console.WriteLine("Запуск всех бенчмарков.");
            PrintSeparator();
            RunAllBenchmarks();
        }
    }

    private static void PrintSeparator()
    {
        Console.WriteLine(new string('-', 30));
    }
    private static void PrintBuildConfiguration()
    {
#if DEBUG
        Console.WriteLine($"{AppModeMessage} DEBUG.");
#else
        Console.WriteLine($"{AppModeMessage} RELEASE.");
#endif
    }

    private static void RunAllBenchmarks()
    {
       BenchmarkSwitcher
        .FromAssembly(typeof(Program).Assembly)
        .Run(["--filter", "*"]);
    }
    private static void RunOneBenchmarks(string benchmarkName)
    {
        switch (benchmarkName)
        {
            case "AddVertex":
                BenchmarkRunner.Run<AddVertexBenchmarks>();
                break;
            case "Edge":
                BenchmarkRunner.Run<EdgeBenchmark>();
                break;
            case "AddVertexWithWeight":
                BenchmarkRunner.Run<AddVertexWithWeightBenchmarks>();
                break;
            case "EdgeWithWeight":
                BenchmarkRunner.Run<EdgeWithWeightBenchmark>();
                break;
            case "AddVertexWithListOfWeight":
                BenchmarkRunner.Run<AddVertexWithListOfWeightBenchmarks>();
                break;
            case "EdgeWithListOfWeight":
                BenchmarkRunner.Run<EdgeWithListOfWeightBenchmark>();
                break;
            case "PathGenerator":
                BenchmarkRunner.Run<PathGeneratorBenchmarks>();
                break;
            default:
                Console.WriteLine($"Бенчмарк с именем '{benchmarkName}' не найден.");
                break;
        }
    }
}


