using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    internal class PineTreeRenderer : IEntityTypeRenderer
    {
        private readonly Texture2D[] _trees;
        private Texture2D _pixel;

        public PineTreeRenderer(Texture2D[] trees) => _trees = trees;

        public PineTreeRenderer(Texture2D pixel)
        {
            _pixel = pixel;
        }

        public bool CanDraw(Entity e) => e is PineTree;

        public void Draw(SpriteBatch sb, World world, Entity e, int tileX, int tileY)
        {
            var tree = (Tree)e;
            int ts = world.Config.TileSize;

            int centerX = (int)tileX * ts + ts / 2;
            int groundY = (int)tileY * ts + ts;

            int trunkWidth = ts / 6;
            int trunkHeight = ts / 2;


            // ----- TRUNK -----
            sb.Draw(
                _pixel,
                new Rectangle(
                    centerX - trunkWidth / 2,
                    groundY - trunkHeight,
                    trunkWidth,
                    trunkHeight),
                new Color(101, 67, 33)); // brown


            // ----- PINE CROWN -----
            int crownHeight = ts;
            int crownWidth = ts;
            float scale = 0.6f + (int)tree.Stage * 0.2f;
            crownHeight = (int)(ts * scale);
            for (int i = 0; i < crownHeight; i++)
            {
                float t = (float)i / crownHeight;

                int width = (int)(crownWidth * (1 - t));
                int x = centerX - width / 2;
                int y = groundY - trunkHeight - crownHeight + i;

                sb.Draw(
                    _pixel,
                    new Rectangle(x, y, width, 1),
                    new Color(30, 130, 50));
            }
        }
    }
}