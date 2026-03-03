using Microsoft.Xna.Framework;
using VillageOfShadows.Core.Config;

namespace VillageOfShadows.Core.World;

public sealed class World
{
    public int Width { get; }
    public int Height { get; }
    public WorldConfig Config { get; }

    public Tile[] Tiles { get; }

    // Entities (voor MVP: enkel villagers; later kan je hier uitbreiden)
    public List<Entities.Villager> Villagers { get; } = new();

    public World(int width, int height, WorldConfig config)
    {
        Width = width;
        Height = height;
        Config = config;

        Tiles = new Tile[width * height];
        for (int i = 0; i < Tiles.Length; i++)
            Tiles[i] = new Tile { Type = TileType.Grass };
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
    public Tile Get(int x, int y) => Tiles[y * Width + x];

    public Point WorldToTile(Vector2 worldPos)
        => new((int)(worldPos.X / Config.TileSize), (int)(worldPos.Y / Config.TileSize));

    public Vector2 TileCenter(int tx, int ty)
        => new(tx * Config.TileSize + Config.TileSize / 2f,
               ty * Config.TileSize + Config.TileSize / 2f);

    public bool IsWalkableTile(int tx, int ty)
        => InBounds(tx, ty) && !Get(tx, ty).HasTree; // later: buildings/water etc.
}