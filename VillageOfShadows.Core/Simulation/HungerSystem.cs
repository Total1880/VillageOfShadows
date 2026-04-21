using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Entities.Jobs;
using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation
{
    public sealed class HungerSystem : IWorldSystem
    {
        private float _hungerIncreaseRate = 0.1f; // per second
        public void Update(World.World world, float dt, IRandom rng)
        {
            var actors = world.GetActors<Actor>();
            var hungerIncrease = _hungerIncreaseRate * dt;

            foreach (var actor in actors)
            {
                actor.Hunger -= hungerIncrease;
                if (actor.Hunger < 0)
                {
                    actor.Hunger = 0;
                }
            }

            var searchFoodJobs = world.GetJobs<SearchFoodJob>().ToList();

            // eerst in z'n stockpile kijken of er al eten ligt, als dat er is dat pakken, 
            // Anders een stockpile zoeken waar eten ligt,
            // Anders een building zoeken waar eten ligt,

            for (var i = 0; i < searchFoodJobs.Count; i++)
            {
                var job = searchFoodJobs[i];
                var actor = world.GetActors<Actor>().First(a => a.EntityId == job.ClaimedByEntityId);

                if (job.State != SearchFoodJobState.MovingToFood && actor.IsCarrying && actor.Carrying.IsEdible)
                {
                    job.State = SearchFoodJobState.Eating;
                    actor.Hunger += actor.Carrying.FoodValuePerUnit;
                    actor.Carrying.Amount -= 1;
                    if (actor.Carrying.Amount <= 0)
                    {
                        actor.Carrying = null;
                    }
                }

                if (job.State != SearchFoodJobState.MovingToFood && actor.Hunger < 10)
                {
                    var stockpilesWithFood = world.GetEntities<Stockpile>().Where(s => s.Inventory.Any(i => i.IsEdible)).ToList().OrderBy(s => Vector2.DistanceSquared(s.Position, actor.Position)).FirstOrDefault();
                    if (stockpilesWithFood != null)
                    {
                        actor.Movement.Target = stockpilesWithFood.Position;
                        job.Target = stockpilesWithFood.EntityId;
                        job.State = SearchFoodJobState.MovingToFood;
                    }
                }

                if (job.State == SearchFoodJobState.MovingToFood && Vector2.DistanceSquared(actor.Position, actor.Movement.Target) < 1f)
                {
                    var stockpile = world.GetEntities<Stockpile>().FirstOrDefault(s => s.Position == actor.Movement.Target);
                    if (stockpile == null)
                        throw new NotImplementedException();
                    var foodItem = stockpile.Inventory.First(i => i.IsEdible);
                    job.State = SearchFoodJobState.Eating;

                    actor.Hunger += foodItem.FoodValuePerUnit;
                    foodItem.Amount -= 1;
                    if (foodItem.Amount <= 0)
                    {
                        stockpile.Inventory.Remove(foodItem);
                    }
                }

                if (actor.Hunger >= 10 || job.Target.IsEmpty)
                {
                    job.IsCompleted = true;
                    ResetVillager((Villager)actor);
                }

                nog iets doen met SearchFoodJobState.Eating, Nu kan het zijn dat stockpile leeg is, maar job blijft bestaan en al. Ook stockpile blijft bestaan indien leeg. Mss recuring job zoals removeCompletedJobs
            }
        }

        private void ResetVillager(Villager villager)
        {
            villager.CurrentJobId = null;
            villager.WorkProgress = 0f;
            villager.State = VillagerState.Idle;
            villager.Movement.Target = villager.Position;
        }
    }
}
