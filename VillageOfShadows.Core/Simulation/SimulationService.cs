using VillageOfShadows.Core.Domain;

namespace VillageOfShadows.Core.Simulation;

public sealed class SimulationService
{
    private readonly GameConfig _cfg;
    private readonly IRandom _rng;

    public SimulationService(GameConfig cfg, IRandom rng)
    {
        _cfg = cfg;
        _rng = rng;
    }

    public void ProcessDay(Village village, Vampire vampire)
    {
        // Production
        int woodGain = village.Villagers.Count(v => v.IsAlive && v.Profession == Profession.Lumberjack) * _cfg.WoodPerLumberjackPerDay;
        int foodGain = village.Villagers.Count(v => v.IsAlive && v.Profession == Profession.Hunter) * _cfg.FoodPerHunterPerDay;

        village.Resources.Wood += woodGain;
        village.Resources.Food += foodGain;

        // Consumption
        int alive = village.Villagers.Count(v => v.IsAlive);
        int neededFood = alive * _cfg.FoodConsumedPerVillagerPerDay;

        if (village.Resources.Food >= neededFood)
        {
            village.Resources.Food -= neededFood;
        }
        else
        {
            // Not enough food: everybody loses health (simple MVP)
            village.Resources.Food = 0;
            foreach (var v in village.Villagers.Where(v => v.IsAlive))
                v.Health = Math.Max(0, v.Health - _cfg.StarvationHealthLossPerDay);
        }

        // Age & small health normalization (optional)
        foreach (var v in village.Villagers.Where(v => v.IsAlive))
            v.AgeDays++;

        // Suspicion cool-down
        village.Suspicion = Clamp01_100(village.Suspicion - _cfg.SuspicionDecayPerDay);

        // Vampire hunger increases across the day
        vampire.Hunger = Clamp01_100(vampire.Hunger + _cfg.HungerIncreasePerDay);

        village.DayNumber++;
    }

    public NightReport ProcessNight(Village village, Vampire vampire, NightOrder order)
    {
        var report = new NightReport();

        int suspicionDelta = 0;
        int hungerDelta = 0;
        int deaths = 0;

        switch (order.Type)
        {
            case NightOrderType.StayHidden:
                report.Lines.Add("The vampire stayed hidden.");
                suspicionDelta -= 1; // tiny decay at night too
                hungerDelta += _cfg.HungerIncreasePerNightIfStayHidden;
                break;

            case NightOrderType.FeedCarefully:
                report.Lines.Add("The vampire fed carefully.");
                suspicionDelta += _cfg.SuspicionGainFeedCarefully;
                hungerDelta -= _cfg.HungerDecreaseFeedCarefully;
                // no death in MVP for careful feed
                break;

            case NightOrderType.FeedAggressively:
                report.Lines.Add("The vampire fed aggressively.");
                suspicionDelta += _cfg.SuspicionGainFeedAggressively;
                hungerDelta -= _cfg.HungerDecreaseFeedAggressively;

                // Risk: kill chance increases if hunger is very high
                double killChance = _cfg.AggressiveKillChanceBase;
                if (vampire.Hunger > 80)
                    killChance += (vampire.Hunger - 80) * _cfg.KillChancePerHungerPointOver80;

                // Discipline reduces risk slightly (simple)
                killChance *= (1.0 - (vampire.Discipline / 200.0)); // discipline 60 => -30%

                bool killed = _rng.NextDouble() < killChance;

                if (killed)
                {
                    var victim = PickVictim(village, order.TargetVillagerId);
                    if (victim is not null)
                    {
                        victim.Health = 0;
                        deaths++;
                        suspicionDelta += _cfg.SuspicionGainKill;
                        report.Lines.Add("A villager was found dead by morning.");
                    }
                }
                break;

            case NightOrderType.TargetVillager:
                report.Lines.Add("Targeting a specific villager is not fully implemented in MVP.");
                // You can later resolve like FeedCarefully/Aggressively + victim selection
                suspicionDelta += 1;
                hungerDelta += 0;
                break;

            default:
                report.Lines.Add("Unknown night order.");
                break;
        }

        // Apply deltas
        village.Suspicion = Clamp01_100(village.Suspicion + suspicionDelta);
        vampire.Hunger = Clamp01_100(vampire.Hunger + hungerDelta);

        report.SuspicionDelta = suspicionDelta;
        report.HungerDelta = hungerDelta;
        report.Deaths = deaths;

        report.Lines.Add($"Suspicion: {(suspicionDelta >= 0 ? "+" : "")}{suspicionDelta} (now {village.Suspicion})");
        report.Lines.Add($"Hunger: {(hungerDelta >= 0 ? "+" : "")}{hungerDelta} (now {vampire.Hunger})");

        return report;
    }

    private Villager? PickVictim(Village village, Guid? targetId)
    {
        if (targetId is Guid id)
        {
            var target = village.Villagers.FirstOrDefault(v => v.IsAlive && v.Id == id);
            if (target is not null) return target;
        }

        var candidates = village.Villagers.Where(v => v.IsAlive).ToList();
        if (candidates.Count == 0) return null;

        return candidates[_rng.Next(0, candidates.Count)];
    }

    private static int Clamp01_100(int v) => Math.Min(100, Math.Max(0, v));
}