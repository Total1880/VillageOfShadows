using System.Collections.Generic;

namespace VillageOfShadows.Game.UI;

public sealed class HudViewModel
{
    public int DayNumber { get; set; }
    public int Population { get; set; }
    public int Food { get; set; }
    public int Wood { get; set; }
    public int Suspicion { get; set; }
    public int Hunger { get; set; }

    public List<string> LastReportLines { get; set; } = new();
}