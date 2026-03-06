namespace VillageOfShadows.Core.Utils
{
    public sealed class RandomAdapter : IRandom
    {
        private readonly Random _rng;
        public RandomAdapter(int seed) => _rng = new Random(seed);
        public RandomAdapter() => _rng = new Random();

        public int Next(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);
        public double NextDouble() => _rng.NextDouble();
    }
}
