using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VillageOfShadows.Game.UI;

public sealed class SimpleButton
{
    public Rectangle Bounds { get; }
    public string Text { get; }

    public SimpleButton(Rectangle bounds, string text)
    {
        Bounds = bounds;
        Text = text;
    }

    public bool IsClicked(MouseState current, MouseState previous)
    {
        bool wasDown = previous.LeftButton == ButtonState.Pressed;
        bool isUpNow = current.LeftButton == ButtonState.Released;
        bool released = wasDown && isUpNow;

        return released && Bounds.Contains(current.Position);
    }

    public void Draw(SpriteBatch sb, SpriteFont font, Texture2D pixel)
    {
        sb.Draw(pixel, Bounds, Color.White * 0.15f);
        sb.Draw(pixel, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), Color.White * 0.35f);
        sb.Draw(pixel, new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.Width, 1), Color.White * 0.35f);
        sb.Draw(pixel, new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height), Color.White * 0.35f);
        sb.Draw(pixel, new Rectangle(Bounds.X + Bounds.Width - 1, Bounds.Y, 1, Bounds.Height), Color.White * 0.35f);

        var size = font.MeasureString(Text);
        var pos = new Vector2(
            Bounds.X + (Bounds.Width - size.X) / 2f,
            Bounds.Y + (Bounds.Height - size.Y) / 2f
        );
        sb.DrawString(font, Text, pos, Color.White);
    }
}