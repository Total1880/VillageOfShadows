namespace VillageOfShadows.Core.Simulation;

public enum NightOrderType
{
    StayHidden,
    FeedCarefully,
    FeedAggressively,
    TargetVillager // optioneel
}

public sealed record NightOrder(
    NightOrderType Type,
    Guid? TargetVillagerId = null
);