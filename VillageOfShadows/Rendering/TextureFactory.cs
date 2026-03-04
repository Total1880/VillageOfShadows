using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.World;

public static class TextureFactory
{
    public static Texture2D CreateGrass(GraphicsDevice gd, int size = 16, int seed = 1337)
    {
        var rng = new Random(seed);
        var tex = new Texture2D(gd, size, size);
        var data = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // basis groen + kleine ruis
                int n = rng.Next(-18, 19);
                int r = Clamp(40 + n, 0, 255);
                int g = Clamp(120 + n * 2, 0, 255);
                int b = Clamp(40 + n, 0, 255);

                // sporadisch een “spriet”
                if (rng.NextDouble() < 0.06)
                    g = Clamp(g + 25, 0, 255);

                data[y * size + x] = new Color(r, g, b);
            }

        tex.SetData(data);
        return tex;
    }

    public static Texture2D CreateTreeStage(GraphicsDevice gd, TreeStage stage)
    {
        // 16x32 sprite (transparant rondom)
        int w = 16, h = 32;
        var tex = new Texture2D(gd, w, h);
        var data = new Color[w * h];

        // clear
        for (int i = 0; i < data.Length; i++) data[i] = Color.Transparent;

        // trunk
        int trunkHeight = stage switch
        {
            TreeStage.Sapling => 10,
            TreeStage.Young => 14,
            TreeStage.Mature => 18,
            _ => 12
        };

        int trunkX = 7;
        for (int y = h - 1; y >= h - trunkHeight; y--)
        {
            data[y * w + trunkX] = new Color(110, 75, 40);
            data[y * w + trunkX + 1] = new Color(95, 65, 35);
        }

        // canopy
        int canopyRadius = stage switch
        {
            TreeStage.Sapling => 3,
            TreeStage.Young => 5,
            TreeStage.Mature => 7,
            _ => 5
        };

        int cx = 8;
        int cy = h - trunkHeight - 5;

        for (int y = -canopyRadius; y <= canopyRadius; y++)
            for (int x = -canopyRadius; x <= canopyRadius; x++)
            {
                int px = cx + x;
                int py = cy + y;
                if (px < 0 || py < 0 || px >= w || py >= h) continue;

                // simpele “bol”
                if (x * x + y * y <= canopyRadius * canopyRadius)
                {
                    // lichte variatie
                    int shade = (x + y) % 3;
                    var c = shade switch
                    {
                        0 => new Color(40, 140, 60),
                        1 => new Color(35, 125, 55),
                        _ => new Color(45, 150, 70)
                    };
                    data[py * w + px] = c;
                }
            }

        tex.SetData(data);
        return tex;
    }

    private static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);
}