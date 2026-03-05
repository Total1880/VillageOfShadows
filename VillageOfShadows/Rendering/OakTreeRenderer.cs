using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    internal class OakTreeRenderer : IEntityTypeRenderer
    {
        private Texture2D _pixel;

        public OakTreeRenderer(Texture2D pixel)
        {
            _pixel = pixel;
        }

        public bool CanDraw(Entity e) => e is OakTree;

        public void Draw(SpriteBatch sb, World world, Entity e, int tileX, int tileY)
        {
            var tree = (Tree)e;
            int ts = world.Config.TileSize;

            int centerX = (int)tileX * ts + ts / 2;
            int groundY = (int)tileY * ts + ts;

            float scale = 0.7f + (int)tree.Stage * 0.25f;

            int trunkWidth = (int)(ts / 4 * scale);
            int trunkHeight = (int)(ts * 0.6f * scale);

            // ----- TRUNK -----
            sb.Draw(
                _pixel,
                new Rectangle(
                    centerX - trunkWidth / 2,
                    groundY - trunkHeight,
                    trunkWidth,
                    trunkHeight),
                new Color(110, 80, 50)
            );


            // ----- OAK CANOPY (round blob) -----
            int crownWidth = (int)(ts * 1.4f * scale);
            int crownHeight = (int)(ts * scale);

            int crownCenterX = centerX;
            int crownCenterY = groundY - trunkHeight - crownHeight / 2;

            for (int y = -crownHeight / 2; y < crownHeight / 2; y++)
            {
                float t = (float)y / (crownHeight / 2);
                float widthFactor = (float)Math.Sqrt(1 - t * t); // circle shape

                int width = (int)(crownWidth * widthFactor);

                int drawX = crownCenterX - width / 2;
                int drawY = crownCenterY + y;

                sb.Draw(
                    _pixel,
                    new Rectangle(drawX, drawY, width, 1),
                    new Color(34, 139, 34));
            }
        }
    }
}