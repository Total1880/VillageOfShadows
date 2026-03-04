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

        // niets gevonden => gewoon niets tekenen (of debug)
        // throw new InvalidOperationException($"No renderer for {entity.GetType().Name}");
    }

    //public void DrawVillagers(SpriteBatch sb, World world)
    //{
    //    foreach (var v in world.Villagers)
    //    {
    //        int size = 6;
    //        var rect = new Rectangle(
    //            (int)(v.Position.X - size / 2f),
    //            (int)(v.Position.Y - size / 2f),
    //            size, size);

    //        sb.Draw(_pixel, rect, Color.LightBlue);
    //    }
    //}
}