using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Game.Rendering
{
    public sealed class StockpileRenderer : IEntityTypeRenderer
    {
        private readonly Texture2D _pixel;

        public StockpileRenderer(Texture2D pixel)
        {
            _pixel = pixel;
        }

        public bool CanDraw(Entity entity)
        {
            return entity is Stockpile;
        }

        public void Draw(SpriteBatch sb, World world, Entity entity, int tileX, int tileY)
        {
            int ts = world.Config.TileSize;

            int x = tileX * ts;
            int y = tileY * ts;

            if (entity is not Building building)
                return;

            // basis: houten platform / zone op de grond
            sb.Draw(_pixel, new Rectangle(x + 1, y + 1, ts - 2, ts - 2), new Color(110, 85, 55));

            // subtiele rand
            sb.Draw(_pixel, new Rectangle(x, y, ts, 1), new Color(80, 60, 35));
            sb.Draw(_pixel, new Rectangle(x, y + ts - 1, ts, 1), new Color(80, 60, 35));
            sb.Draw(_pixel, new Rectangle(x, y, 1, ts), new Color(80, 60, 35));
            sb.Draw(_pixel, new Rectangle(x + ts - 1, y, 1, ts), new Color(80, 60, 35));

            // inhoud tekenen als stapeltjes
            DrawInventory(sb, building, x, y, ts);

            // optioneel: andere tint voor temporary stockpile
            if (((Stockpile)entity).Kind == StockpileKind.Temporary)
            {
                sb.Draw(_pixel, new Rectangle(x + 3, y + 3, ts - 6, 2), new Color(180, 160, 100, 120));
            }
        }

        public void DrawEntities(SpriteBatch sb, World world, IEnumerable<Entity> entities)
        {
            throw new NotImplementedException();
        }

        private void DrawInventory(SpriteBatch sb, Building building, int x, int y, int ts)
        {
            if (building.Inventory == null || building.Inventory.Count == 0)
                return;

            int cols = 2;
            int stackSize = Math.Max(3, ts / 4);

            for (int i = 0; i < building.Inventory.Count && i < 4; i++)
            {
                int col = i % cols;
                int row = i / cols;

                int sx = x + 3 + col * (stackSize + 2);
                int sy = y + ts - 3 - ((row + 1) * (stackSize + 2));

                var stack = building.Inventory[i];

                Color color = stack.ResourceType switch
                {
                    ResourceType.Wood => new Color(139, 90, 43),      // bruin hout
                    ResourceType.Apples => new Color(200, 40, 40),    // rood
                    ResourceType.Berries => new Color(120, 40, 160),  // paars
                    ResourceType.Stone => new Color(130, 130, 130),   // steen
                    _ => Color.Gray
                };

                sb.Draw(_pixel, new Rectangle(sx, sy, stackSize, stackSize), color);
            }
        }
    }
}
