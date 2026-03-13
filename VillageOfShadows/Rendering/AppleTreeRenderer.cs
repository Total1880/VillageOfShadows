using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    internal class AppleTreeRenderer : IEntityTypeRenderer
    {
        private Texture2D _pixel;

        public AppleTreeRenderer(Texture2D pixel)
        {
            _pixel = pixel;
        }

        public bool CanDraw(Entity e) => e is AppleTree;

        public void Draw(SpriteBatch sb, World world, Entity e, int tileX, int tileY)
        {
            var tree = (AppleTree)e; // of (Tree)e als AppleTree Tree erft
            int ts = world.Config.TileSize;

            int centerX = tileX * ts + ts / 2;
            int groundY = tileY * ts + ts;

            float scale = 0.7f + (int)tree.Stage * 0.25f;

            int trunkWidth = (int)(ts / 4f * scale);
            int trunkHeight = (int)(ts * 0.6f * scale);

            // ----- TRUNK -----
            sb.Draw(
                _pixel,
                new Rectangle(
                    centerX - trunkWidth / 2,
                    groundY - trunkHeight,
                    trunkWidth,
                    trunkHeight),
                new Color(120, 85, 55)
            );

            // ----- APPLE TREE CANOPY (round blob) -----
            int crownWidth = (int)(ts * 1.45f * scale);
            int crownHeight = (int)(ts * 1.05f * scale);

            int crownCenterX = centerX;
            int crownCenterY = groundY - trunkHeight - crownHeight / 2;

            // iets lichter/gele-groen dan oak
            var leafColor = new Color(60, 165, 60);

            for (int y = -crownHeight / 2; y < crownHeight / 2; y++)
            {
                float t = (float)y / (crownHeight / 2f);
                float widthFactor = (float)Math.Sqrt(1 - t * t); // circle shape

                int width = (int)(crownWidth * widthFactor);

                int drawX = crownCenterX - width / 2;
                int drawY = crownCenterY + y;

                sb.Draw(
                    _pixel,
                    new Rectangle(drawX, drawY, width, 1),
                    leafColor
                );
            }

            // ----- APPLES (small dots, deterministic so no flicker) -----
            // Meer appels naarmate stage hoger is (optioneel)
            int apples = (int)tree.Stage == 2 ? (int)tree.FoodValue : 0; // bv: stage0=2, stage1=4, stage2=6...

            // Deterministische seed: gebruik iets stabiels (EntityId / Guid hash / int id)
            int seed = tree.EntityId.GetHashCode(); // pas aan naar jouw Id property
            var rng = new Random(seed);

            // appel grootte: 1..2 pixels (bij grotere bomen soms 2)
            int appleSize = (scale >= 1.2f) ? 2 : 1;

            for (int i = 0; i < apples; i++)
            {
                // Kies random y in de kroon, en bereken de "cirkel" breedte op die y
                int yy = rng.Next(-crownHeight / 2, crownHeight / 2);
                float tt = (float)yy / (crownHeight / 2f);
                float wf = (float)Math.Sqrt(1 - tt * tt);
                int rowWidth = (int)(crownWidth * wf);

                // x binnen die rij (iets marge zodat appels niet buiten kroon vallen)
                int margin = Math.Min(2, Math.Max(0, rowWidth / 4));
                int minX = -rowWidth / 2 + margin;
                int maxX = rowWidth / 2 - margin;

                if (minX > maxX)
                    continue;

                int xx = rng.Next(minX, maxX + 1);

                int ax = crownCenterX + xx;
                int ay = crownCenterY + yy;

                // variatie rood/oranje
                Color appleColor = (rng.NextDouble() < 0.25)
                    ? new Color(220, 120, 40)  // wat oranje
                    : new Color(200, 40, 35);  // rood

                sb.Draw(
                    _pixel,
                    new Rectangle(ax - appleSize / 2, ay - appleSize / 2, appleSize, appleSize),
                    appleColor
                );
            }

            // ----- (optioneel) highlight bovenop voor "volume" -----
            // Een subtiele licht-spot linksboven in de kroon
            int highlightW = (int)(crownWidth * 0.35f);
            int highlightH = (int)(crownHeight * 0.25f);
            sb.Draw(
                _pixel,
                new Rectangle(
                    crownCenterX - (int)(crownWidth * 0.25f),
                    crownCenterY - (int)(crownHeight * 0.25f),
                    highlightW,
                    highlightH),
                new Color(80, 190, 80) * 0.35f
            );
        }
    }
}