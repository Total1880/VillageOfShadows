using Microsoft.Xna.Framework;
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
            var r = _renderers[i];
            if (r.CanDraw(entity))
            {
                r.Draw(sb, world, entity, tileX, tileY);
                return;
            }
        }
    }

    public void DrawEntities(SpriteBatch sb, World world, IEnumerable<Entity> entities)
    {
        foreach (var e in entities)
        {
            int tileX = (int)(e.Position.X / world.Config.TileSize);
            int tileY = (int)(e.Position.Y / world.Config.TileSize);
            Draw(sb, world, e, tileX, tileY);
        }
    }
}