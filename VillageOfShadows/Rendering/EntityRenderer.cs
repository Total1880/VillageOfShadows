using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering;

public sealed class EntityRenderer
{
    private readonly Texture2D _pixel;

    public EntityRenderer(Texture2D pixel) => _pixel = pixel;

    public void DrawVillagers(SpriteBatch sb, World world)
    {
        foreach (var v in world.Villagers)
        {
            int size = 6;
            var rect = new Rectangle(
                (int)(v.Position.X - size / 2f),
                (int)(v.Position.Y - size / 2f),
                size, size);

            sb.Draw(_pixel, rect, Color.LightBlue);
        }
    }
}