namespace VillageOfShadows.Core.Utils
{
    public interface IRandom
    {
        int Next(int minInclusive, int maxExclusive);
        double NextDouble();
    }
}
