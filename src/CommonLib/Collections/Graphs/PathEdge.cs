/// <summary>
/// Представляет ребро пути в графе, содержащее целевую вершину и накопленный вес пути.
/// </summary>
/// <typeparam name="T">Тип вершины</typeparam>
public struct PathEdge<T>
{
    /// <summary>
    /// Целевая вершина ребра пути.
    /// </summary>
    public T Target { get; init; }

    /// <summary>
    /// Накопленный вес пути до этой вершины.
    /// </summary>
    public double PathWeight { get; set; }
    public PathEdge(T target, double pathWeight)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (double.IsNaN(pathWeight) || double.IsInfinity(pathWeight))
            throw new ArgumentOutOfRangeException(nameof(pathWeight));

        Target = target;
        PathWeight = pathWeight;
    }
}