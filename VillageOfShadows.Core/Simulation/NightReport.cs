namespace VillageOfShadows.Core.Simulation;

public sealed class NightReport
{
    public List<string> Lines { get; } = new();

    public int SuspicionDelta { get; set; }
    public int Deaths { get; set; }
    public int HungerDelta { get; set; }
}