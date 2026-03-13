using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VillageOfShadows.Game.Rendering;

public sealed class Camera2D
{
    public Vector2 Position { get; private set; } = Vector2.Zero; // top-left in world pixels
    public float Zoom { get; private set; } = 1f;

    public float MinZoom { get; set; } = 0.5f;
    public float MaxZoom { get; set; } = 4.0f;

    public Matrix GetViewMatrix()
        => Matrix.CreateTranslation(-Position.X, -Position.Y, 0f)
         * Matrix.CreateScale(Zoom, Zoom, 1f);

    public void Move(Vector2 deltaWorldPixels)
    {
        Position += deltaWorldPixels;
    }

    public void SetPosition(Vector2 worldTopLeft)
    {
        Position = worldTopLeft;
    }

    public void ZoomAtScreenPoint(float zoomFactor, Vector2 screenPoint, GraphicsDevice gd)
    {
        // World coord under mouse BEFORE zoom
        var before = ScreenToWorld(screenPoint, gd);

        Zoom = MathHelper.Clamp(Zoom * zoomFactor, MinZoom, MaxZoom);

        // World coord under mouse AFTER zoom
        var after = ScreenToWorld(screenPoint, gd);

        // Move camera so the point under cursor stays stable
        Position += (before - after);
    }

    public Vector2 ScreenToWorld(Vector2 screenPoint, GraphicsDevice gd)
    {
        // Inverse of view transform
        var inv = Matrix.Invert(GetViewMatrix());
        return Vector2.Transform(screenPoint, inv);
    }

    public Vector2 WorldToScreen(Vector2 worldPoint)
    {
        return Vector2.Transform(worldPoint, GetViewMatrix());
    }

    public void ClampToWorld(int worldPixelWidth, int worldPixelHeight, GraphicsDevice gd)
    {
        // Visible size in world pixels depends on zoom
        float viewW = gd.Viewport.Width / Zoom;
        float viewH = gd.Viewport.Height / Zoom;

        // If world smaller than view, center it
        float maxX = Math.Max(0, worldPixelWidth - viewW);
        float maxY = Math.Max(0, worldPixelHeight - viewH);

        float x = MathHelper.Clamp(Position.X, 0, maxX);
        float y = MathHelper.Clamp(Position.Y, 0, maxY);

        Position = new Vector2(x, y);
    }

    public Matrix GetTransform()
    {
        return
            Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
            Matrix.CreateScale(Zoom, Zoom, 1f);
    }
}