using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering;

public sealed class WorldRenderer
{
    private readonly Texture2D _grass;
    private readonly IEntityRenderer _entityRenderer;

    public WorldRenderer(Texture2D grass, IEntityRenderer entityRenderer)
    {
        _grass = grass;
        _entityRenderer = entityRenderer;
    }

    public void Draw(SpriteBatch sb, World world)
    {
        int ts = world.Config.TileSize;

        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                sb.Draw(_grass, new Rectangle(x * ts, y * ts, ts, ts), Color.White);

                foreach (var entity in world.GetTileEntitiesOnTile(x, y))
                {
                    _entityRenderer.Draw(sb, world, entity, x, y);
                }
            }
        }

        foreach (var actor in world.GetEntities<Actor>())
        {
            int tileX = (int)(actor.Position.X / ts);
            int tileY = (int)(actor.Position.Y / ts);

            _entityRenderer.Draw(sb, world, actor, tileX, tileY);
        }
    }
}