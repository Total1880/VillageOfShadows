namespace VillageOfShadows.Core.Domain;

public sealed class Villager
{
    public Guid Id { get; } = Guid.NewGuid();

    public int AgeDays { get; set; }
    public int Health { get; set; } = 100;
    public Profession Profession { get; set; }

    public bool IsAlive => Health > 0;

    // Simple positioning for MonoGame (tile-ish)
    public int X { get; set; }
    public int Y { get; set; }
}