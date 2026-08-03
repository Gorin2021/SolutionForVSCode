/// <summary>
/// Представляет неизменяемое ребро графа.
/// </summary>
public record struct Edge<T>
    where T : notnull
{
    public T Target { get; init; }
    public double Weight { get; init; }

    public Edge(T target, double weight)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (double.IsNaN(weight))
            throw new ArgumentOutOfRangeException(nameof(weight));

        if (double.IsNegativeInfinity(weight))
            throw new ArgumentOutOfRangeException(nameof(weight));

        Target = target;
        Weight = weight;
    }

    public override string ToString()
        => $"{Target} (w: {Weight})";
}