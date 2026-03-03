namespace VillageOfShadows.Core.Domain;

public sealed class Village
{
    public List<Villager> Villagers { get; } = new();
    public VillageResources Resources { get; } = new();

    public int Suspicion { get; set; } = 0; // 0..100
    public int DayNumber { get; set; } = 1;
}