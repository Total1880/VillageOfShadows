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
    public sealed class GenericJobSystem : IWorldSystem
    {
        public void Update(World.World world, float dt, IRandom rng)
        {
            CreateGenericJob(world);
            UpdateGenericJobs(world);
        }

        private void UpdateGenericJobs(World.World world)
        {
            UpdateHaulingJobs(world);
        }

        private void UpdateHaulingJobs(World.World world)
        {
            var haulingSourceJobs = world.GetJobs<HaulingJob>().ToList().Where(_ => _.IsClaimed && _.State == HaulingJobState.MovingToSource);
            foreach (var haulingJob in haulingSourceJobs)
            {
                var villager = world.GetActors<Villager>().FirstOrDefault(_ => _.CurrentJobId == haulingJob.Id);
                if (villager == null) continue;

                var stockpile = world.GetEntities<Stockpile>().First(_ => _.EntityId == haulingJob.SourceStockpileId);
                if (Vector2.DistanceSquared(villager.Position, stockpile.Position) < 1f)
                {
                    world.TransferInventoryFromStockPileToActor(stockpile.EntityId, villager.EntityId);
                    haulingJob.State = HaulingJobState.MovingToDestination;
                    villager.Movement.Target = world.GetEntities<Stockpile>().First(_ => _.EntityId == haulingJob.DestinationStockpileId).Position;
                }
            }

            var haulingDestinationJobs = world.GetJobs<HaulingJob>().ToList().Where(_ => _.IsClaimed && _.State == HaulingJobState.MovingToDestination);
            foreach (var haulingJob in haulingDestinationJobs)
            {
                var villager = world.GetActors<Villager>().FirstOrDefault(_ => _.CurrentJobId == haulingJob.Id);
                if (villager == null || !villager.IsCarrying) continue;

                var stockpile = world.GetEntities<Stockpile>().First(_ => _.EntityId == haulingJob.DestinationStockpileId);

                if (Vector2.DistanceSquared(villager.Position, stockpile.Position) < 1f)
                {
                    world.TransferInventoryFromActorToStockpile(villager.EntityId, stockpile.EntityId);
                    haulingJob.IsCompleted = true;
                    ResetVillager(villager);
                }
            }

            var completedHaulingJobs = world.GetJobs<HaulingJob>().Where(_ => _.IsCompleted).ToList();
        }

        private void ResetVillager(Villager villager)
        {
            villager.CurrentJobId = null;
            villager.WorkProgress = 0f;
            villager.State = VillagerState.Idle;
            villager.Movement.Target = villager.Position;
        }
        private void CreateGenericJob(World.World world)
        {
            HaulingJob(world);
        }

        private bool HaulingJob(World.World world)
        {
            var tempStockPiles = world.GetEntities<Stockpile>().Where(_ => _.Kind == StockpileKind.Temporary);
            if (!tempStockPiles.Any()) return false;

            var playerBuiltStockPile = world.GetEntities<Stockpile>().Where(_ => _.Kind == StockpileKind.PlayerBuilt);
            if (!playerBuiltStockPile.Any()) return false;

            var existingJobs = world.GetJobs<HaulingJob>().ToList();

            foreach (var tempStockPile in tempStockPiles)
            {
                if (existingJobs.Any(_ => _.SourceStockpileId == tempStockPile.EntityId)) continue;
                var closestPlayerBuiltStockPile = playerBuiltStockPile.OrderBy(s => Vector2.DistanceSquared(s.Position, tempStockPile.Position)).First();

                world.AddJob(new HaulingJob(tempStockPile.EntityId, closestPlayerBuiltStockPile.EntityId));
            }

            return true;

        }
    }
}
