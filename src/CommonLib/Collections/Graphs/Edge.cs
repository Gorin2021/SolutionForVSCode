namespace GraphLibrary;

/// <summary>
/// Представляет неизменяемое ребро графа.
/// </summary>
public record struct Edge<T>
    where T : notnull
{
    public T Target { get; init; }
    public double Weight { get; set; }
    public Edge(T Target, double Weight)
    {
        ArgumentNullException.ThrowIfNull(Target);

        if (double.IsNaN(Weight))
            throw new ArgumentOutOfRangeException(nameof(Weight));

        if (double.IsNegativeInfinity(Weight))
            throw new ArgumentOutOfRangeException(nameof(Weight));

        this.Target = Target;
        this.Weight = Weight;
    }

    public override string ToString()
        => $"{Target} (w: {Weight})";
}