using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    internal class VillagerRenderer : IEntityTypeRenderer
    {
        private readonly Texture2D _pixel;

        public VillagerRenderer(Texture2D pixel)
        {
            _pixel = pixel;
        }

        public bool CanDraw(Entity e) => e is Villager;

        public void Draw(SpriteBatch sb, World world, Entity e, int tileX, int tileY)
        {
            int ts = world.Config.TileSize;

            int baseX = tileX * ts;
            int baseY = tileY * ts;

            // "Feet" on ground
            int groundY = baseY + ts;

            // Body proportions (scale with tile size)
            int bodyH = (int)(ts * 0.42f);
            int bodyW = (int)(ts * 0.22f);

            int head = Math.Max(2, (int)(ts * 0.18f));

            int centerX = baseX + ts / 2;

            int bodyX = centerX - bodyW / 2;
            int bodyY = groundY - bodyH;

            int headX = centerX - head / 2;
            int headY = bodyY - head;

            // Colors (tweak later, or vary per villager)
            var outline = new Color(20, 20, 20);
            var skin = new Color(235, 205, 175);
            var shirt = new Color(60, 110, 200);
            var pants = new Color(55, 55, 65);

            // ----- OUTLINE SHADOW (simple 1px) -----
            sb.Draw(_pixel, new Rectangle(bodyX - 1, bodyY - 1, bodyW + 2, bodyH + 2), outline);

            // ----- BODY (shirt + pants split) -----
            int shirtH = (int)(bodyH * 0.55f);
            int pantsH = bodyH - shirtH;

            sb.Draw(_pixel, new Rectangle(bodyX, bodyY, bodyW, shirtH), shirt);
            sb.Draw(_pixel, new Rectangle(bodyX, bodyY + shirtH, bodyW, pantsH), pants);

            // ----- HEAD (with outline) -----
            sb.Draw(_pixel, new Rectangle(headX - 1, headY - 1, head + 2, head + 2), outline);
            sb.Draw(_pixel, new Rectangle(headX, headY, head, head), skin);

            // ----- ARMS (tiny) -----
            int armW = Math.Max(1, bodyW / 4);
            int armH = Math.Max(2, (int)(bodyH * 0.35f));

            int leftArmX = bodyX - armW;
            int rightArmX = bodyX + bodyW;
            int armY = bodyY + (int)(shirtH * 0.25f);

            sb.Draw(_pixel, new Rectangle(leftArmX, armY, armW, armH), shirt);
            sb.Draw(_pixel, new Rectangle(rightArmX, armY, armW, armH), shirt);

            // ----- FEET -----
            int footW = Math.Max(1, bodyW / 3);
            int footH = Math.Max(1, ts / 16);

            sb.Draw(_pixel, new Rectangle(bodyX, groundY - footH, footW, footH), outline);
            sb.Draw(_pixel, new Rectangle(bodyX + bodyW - footW, groundY - footH, footW, footH), outline);

            // OPTIONAL: show facing direction / selection (if you have it)
            // sb.Draw(_pixel, new Rectangle(baseX + 1, baseY + 1, ts - 2, 1), Color.White);
        }
    }
}