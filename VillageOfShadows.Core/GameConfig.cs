namespace VillageOfShadows.Core;

public sealed class GameConfig
{
    // World
    public int InitialVillagers { get; init; } = 20;
    public int StartFood { get; init; } = 40;
    public int StartWood { get; init; } = 20;

    // Day production
    public int WoodPerLumberjackPerDay { get; init; } = 2;
    public int FoodPerHunterPerDay { get; init; } = 2;

    // Consumption & health
    public int FoodConsumedPerVillagerPerDay { get; init; } = 1;
    public int StarvationHealthLossPerDay { get; init; } = 10;

    // Suspicion
    public int SuspicionDecayPerDay { get; init; } = 1; // passively cools down
    public int SuspicionGainFeedCarefully { get; init; } = 1;
    public int SuspicionGainFeedAggressively { get; init; } = 6;
    public int SuspicionGainKill { get; init; } = 12;

    // Vampire hunger
    public int HungerIncreasePerDay { get; init; } = 5;
    public int HungerIncreasePerNightIfStayHidden { get; init; } = 2;
    public int HungerDecreaseFeedCarefully { get; init; } = 20;
    public int HungerDecreaseFeedAggressively { get; init; } = 40;

    // Risk (simple)
    public double AggressiveKillChanceBase { get; init; } = 0.20; // 20%
    public double KillChancePerHungerPointOver80 { get; init; } = 0.01; // +1% per point
}