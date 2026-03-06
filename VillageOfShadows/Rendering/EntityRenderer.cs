using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering;

public sealed class EntityRenderer : IEntityRenderer
{
    private readonly List<IEntityTypeRenderer> _renderers;

    public EntityRenderer(params IEntityTypeRenderer[] renderers)
    {
        _renderers = renderers.ToList();
    }

    public void Draw(SpriteBatch sb, World world, Entity entity, int tileX, int tileY)
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            var renderer = _renderers[i];
            if (renderer.CanDraw(entity))
            {
                renderer.Draw(sb, world, entity, tileX, tileY);
                return;
            }
        }
    }

    public void DrawEntities(SpriteBatch sb, World world, IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
        {
            int tileX = (int)(entity.Position.X / world.Config.TileSize);
            int tileY = (int)(entity.Position.Y / world.Config.TileSize);

            Draw(sb, world, entity, tileX, tileY);
        }
    }
}