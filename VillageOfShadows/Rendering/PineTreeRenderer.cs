using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    internal class PineTreeRenderer : IEntityRenderer
    {
        private readonly Texture2D[] _trees;

        public PineTreeRenderer(Texture2D[] trees) => _trees = trees;

        public bool CanDraw(Entity e) => e is Tree;

        public void Draw(SpriteBatch sb, World world, Entity e, int tileX, int tileY)
        {
            var tree = (Tree)e;
            int ts = world.Config.TileSize;

            var tex = _trees[(int)tree.Stage];
            int drawX = tileX * ts + (ts / 2) - (tex.Width / 2);
            int drawY = tileY * ts + ts - tex.Height;

            sb.Draw(tex, new Vector2(drawX, drawY), Color.White);
        }
    }
}