using System.Collections.Generic;
using System.Linq;
using VillageOfShadows.Core;
using VillageOfShadows.Core.Domain;
using VillageOfShadows.Core.Simulation;
using VillageOfShadows.Game.UI;

namespace VillageOfShadows.Game.GameFlow;

public sealed class GameManager
{
    private readonly SimulationService _sim;

    public GamePhase Phase { get; private set; } = GamePhase.Day;

    public Village Village { get; }
    public Vampire Vampire { get; }
    public HudViewModel Hud { get; } = new();

    public NightOrder? PendingOrder { get; private set; }
    public NightReport? LastReport { get; private set; }

    public GameManager(SimulationService sim, Village village, Vampire vampire)
    {
        _sim = sim;
        Village = village;
        Vampire = vampire;
        RefreshHud();
    }

    public void AdvanceFromDayToNight()
    {
        if (Phase != GamePhase.Day) return;

        _sim.ProcessDay(Village, Vampire);
        Phase = GamePhase.NightPlanning;
        RefreshHud();
    }

    public void ChooseNightOrder(NightOrder order)
    {
        if (Phase != GamePhase.NightPlanning) return;

        PendingOrder = order;
        Phase = GamePhase.NightResolve;
    }

    public void ResolveNight()
    {
        if (Phase != GamePhase.NightResolve || PendingOrder is null) return;

        LastReport = _sim.ProcessNight(Village, Vampire, PendingOrder);
        PendingOrder = null;

        // Win/Lose MVP
        if (Village.Suspicion >= 100 || Village.Villagers.Count(v => v.IsAlive) <= 0)
            Phase = GamePhase.GameOver;
        else
            Phase = GamePhase.Day;

        RefreshHud();
    }

    private void RefreshHud()
    {
        Hud.DayNumber = Village.DayNumber;
        Hud.Population = Village.Villagers.Count(v => v.IsAlive);
        Hud.Food = Village.Resources.Food;
        Hud.Wood = Village.Resources.Wood;
        Hud.Suspicion = Village.Suspicion;
        Hud.Hunger = Vampire.Hunger;

        Hud.LastReportLines = LastReport?.Lines.ToList() ?? new List<string>();
    }
}