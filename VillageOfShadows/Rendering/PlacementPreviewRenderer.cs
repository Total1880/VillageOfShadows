using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;
using VillageOfShadows.Game.State;

namespace VillageOfShadows.Game.Rendering
{
    public class PlacementPreviewRenderer
    {
        private readonly Texture2D _pixel;

        public PlacementPreviewRenderer(Texture2D pixel)
        {
            _pixel = pixel;
        }

        public void Draw(SpriteBatch sb, World world, BuildState state, Point mouseWorld)
        {
            if (!state.IsPlacing)
                return;

            int ts = world.Config.TileSize;

            int tileX = mouseWorld.X / ts;
            int tileY = mouseWorld.Y / ts;

            if (!world.InBounds(tileX, tileY))
                return;

            bool canPlace =
                world.GetTile(tileX, tileY).IsWalkable &&
                !world.GetTileEntitiesOnTile(tileX, tileY).Any(e => e is Building);

            Color color = canPlace ? Color.Lime * 0.4f : Color.Red * 0.4f;

            sb.Draw(_pixel, new Rectangle(tileX * ts, tileY * ts, ts, ts), color);
        }
    }
}
