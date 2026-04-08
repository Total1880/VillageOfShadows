using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Systems
{
    public sealed class PlacementSystem
    {
        public bool TryPlace(World world, BuildType buildType, int tileX, int tileY)
        {
            if (!world.InBounds(tileX, tileY))
                return false;

            var tile = world.GetTile(tileX, tileY);
            if (!tile.IsWalkable)
                return false;

            // Zorg dat er geen blocking entity staat
            if (world.GetTileEntitiesOnTile(tileX, tileY).Any(e => e is Building))
                return false;

            TileEntity entity = buildType switch
            {
                BuildType.Stockpile => new Stockpile(),
                _ => throw new InvalidOperationException($"Unsupported build type: {buildType}")
            };

            return world.TryPlaceTileEntity(entity, tileX, tileY);
        }

        internal bool TryPlaceGeneralJob(World world, GeneralJobType generalJobType, int tileX, int tileY)
        {
            if (!world.InBounds(tileX, tileY))
                return false;

            if (generalJobType == GeneralJobType.ChopTree || generalJobType == GeneralJobType.GatherApples)
                foreach (var entity in world.GetTileEntitiesOnTile(tileX, tileY))
                {
                    if (generalJobType == GeneralJobType.ChopTree)
                        if (entity is Tree tree)
                            return MarkTreeForChop(world, tree);
                    if (generalJobType == GeneralJobType.GatherApples)
                        if (entity is AppleTree treeWithApples && treeWithApples.HasFood)
                            return MarkTreeForGathering(world, treeWithApples);
                }

            return false;
        }

        private bool MarkTreeForGathering(World world, AppleTree treeWithApples)
        {
            if (treeWithApples.MarkedForFoodGathering)
                return false;

            bool alreadyHasJob = world.GetJobs<GatherFoodFromTreeJob>()
                .Any(j => j.TreeId == treeWithApples.EntityId && !j.IsCompleted);

            if (alreadyHasJob)
                return false;

            treeWithApples.MarkedForFoodGathering = true;
            world.AddJob(new GatherFoodFromTreeJob(treeWithApples.EntityId));
            return true;
        }

        private bool MarkTreeForChop(World world, Tree tree)
        {
            if (tree.MarkedForChop)
                return false;

            bool alreadyHasJob = world.GetJobs<ChopTreeJob>()
                .Any(j => j.TreeId == tree.EntityId && !j.IsCompleted);

            if (alreadyHasJob)
                return false;

            tree.MarkedForChop = true;
            world.AddJob(new ChopTreeJob(tree.EntityId));
            return true;
        }
    }
}
