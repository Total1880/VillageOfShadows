namespace VillageOfShadows.Core.Domain;

public sealed class Vampire
{
    public int Hunger { get; set; } = 20;       // 0..100
    public int Discipline { get; set; } = 60;   // 0..100 (later effect on risk)
    public int Stress { get; set; } = 0;        // 0..100 (later)
}