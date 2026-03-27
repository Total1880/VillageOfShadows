using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VillageOfShadows.Game.State;

namespace VillageOfShadows.Game.UI
{
    public class BuildMenuRenderer
    {
        private readonly Texture2D _pixel;
        private readonly SpriteFont _font;

        public BuildMenuRenderer(Texture2D pixel, SpriteFont font)
        {
            _pixel = pixel;
            _font = font;
        }

        public void Draw(SpriteBatch sb, BuildState state)
        {
            if (!state.IsBuildMenuOpen)
                return;

            sb.Draw(_pixel, new Rectangle(20, 20, 220, 100), Color.Black * 0.7f);

            sb.DrawString(_font, "Build Menu", new Vector2(30, 30), Color.White);
            sb.DrawString(_font, "[1] Stockpile", new Vector2(30, 60), Color.White);
        }
    }
}
