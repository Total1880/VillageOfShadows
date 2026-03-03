using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.World
{
    public sealed class World
    {
        public int Width { get; }
        public int Height { get; }
        public int TileSize { get; }

        public Tile[] Tiles { get; }

        private readonly Random _rng;

        // Tuning (MVP)
        private const float GrowthPerSecondSapling = 0.030f;
        private const float GrowthPerSecondYoung = 0.020f;
        private const float SpreadChancePerSecondMature = 0.0025f; // chance to try to spawn a sapling

        public World(int width, int height, int tileSize, int seed = 12345)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            _rng = new Random(seed);

            Tiles = new Tile[width * height];
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new Tile { Type = TileType.Grass };
        }

        public Tile Get(int x, int y) => Tiles[y * Width + x];
        public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

        public void SeedTrees(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = _rng.Next(0, Width);
                int y = _rng.Next(0, Height);
                var t = Get(x, y);
                t.HasTree = true;
                t.TreeStage = TreeStage.Sapling;
                t.TreeGrowth = (float)_rng.NextDouble() * 0.25f;
            }
        }

        public void Update(float dt)
        {
            // Simpel en duidelijk: loop over tiles en update bomen
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var tile = Get(x, y);
                    if (!tile.HasTree) continue;

                    // Growth
                    float g = tile.TreeStage switch
                    {
                        TreeStage.Sapling => GrowthPerSecondSapling,
                        TreeStage.Young => GrowthPerSecondYoung,
                        TreeStage.Mature => 0f,
                        _ => 0f
                    };

                    if (g > 0f)
                    {
                        tile.TreeGrowth += g * dt;
                        if (tile.TreeGrowth >= 1f)
                        {
                            tile.TreeGrowth = 0f;
                            if (tile.TreeStage != TreeStage.Mature)
                                tile.TreeStage++;
                        }
                    }
                    else
                    {
                        // Spread (alleen mature)
                        if (tile.TreeStage == TreeStage.Mature)
                        {
                            if (_rng.NextDouble() < SpreadChancePerSecondMature * dt)
                                TrySpawnSaplingNear(x, y);
                        }
                    }
                }
        }

        private void TrySpawnSaplingNear(int x, int y)
        {
            // probeer een paar keer in een kleine radius (2)
            for (int attempt = 0; attempt < 6; attempt++)
            {
                int nx = x + _rng.Next(-2, 3);
                int ny = y + _rng.Next(-2, 3);
                if (!InBounds(nx, ny)) continue;

                var t = Get(nx, ny);
                if (t.HasTree) continue;

                t.HasTree = true;
                t.TreeStage = TreeStage.Sapling;
                t.TreeGrowth = 0f;
                return;
            }
        }
    }
}
