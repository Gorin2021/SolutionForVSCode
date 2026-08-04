public struct PathEdge<T>
{
    public T Target { get; init; }
    public double PathWeight { get; set; }
    public PathEdge(T target, double pathWeight)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (double.IsNaN(pathWeight))
            throw new ArgumentOutOfRangeException(nameof(pathWeight));

        if (double.IsNegativeInfinity(pathWeight))
            throw new ArgumentOutOfRangeException(nameof(pathWeight));

        Target = target;
        PathWeight = pathWeight;
    }
}