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
    }
}
