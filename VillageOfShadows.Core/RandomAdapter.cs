namespace VillageOfShadows.Core;

public interface IRandom
{
    int Next(int minInclusive, int maxExclusive);
    double NextDouble();
}

public sealed class RandomAdapter : IRandom
{
    private readonly Random _rng;

    public RandomAdapter(int seed) => _rng = new Random(seed);

    public int Next(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);
    public double NextDouble() => _rng.NextDouble();
}