using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering;

public sealed class WorldRenderer
{
    private readonly Texture2D _grass;
    private readonly Texture2D[] _trees;

    public WorldRenderer(Texture2D grass, Texture2D[] trees)
    {
        _grass = grass;
        _trees = trees;
    }

    public void Draw(SpriteBatch sb, World world)
    {
        int ts = world.Config.TileSize;

        for (int y = 0; y < world.Height; y++)
            for (int x = 0; x < world.Width; x++)
            {
                var t = world.Get(x, y);

                sb.Draw(_grass, new Rectangle(x * ts, y * ts, ts, ts), Color.White);

                if (t.HasTree)
                {
                    var treeTex = _trees[(int)t.TreeStage];
                    int drawX = x * ts + (ts / 2) - (treeTex.Width / 2);
                    int drawY = y * ts + ts - treeTex.Height;
                    sb.Draw(treeTex, new Vector2(drawX, drawY), Color.White);
                }
            }
    }
}