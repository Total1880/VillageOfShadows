using System.Drawing;
using System.Numerics;
using VillageOfShadows.Core.Config;
using VillageOfShadows.Core.Entities;

namespace VillageOfShadows.Core.World;

public sealed class World
{
    public int Width { get; }
    public int Height { get; }
    public WorldConfig Config { get; }

    public Tile[] Tiles { get; }

    public Dictionary<EntityId, Entity> Entities { get; set; }

    public World(int width, int height, WorldConfig config)
    {
        Width = width;
        Height = height;
        Config = config;
        Entities = new Dictionary<EntityId, Entity>();

        Tiles = new Tile[width * height];
        for (int i = 0; i < Tiles.Length; i++)
            Tiles[i] = new Tile { Type = TileType.Grass };
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public Point WorldToTile(Vector2 worldPos)
        => new((int)(worldPos.X / Config.TileSize), (int)(worldPos.Y / Config.TileSize));

    public Vector2 TileCenter(int tx, int ty)
        => new(tx * Config.TileSize + Config.TileSize / 2f,
               ty * Config.TileSize + Config.TileSize / 2f);

    public bool IsWalkableTile(int tx, int ty)
        => InBounds(tx, ty);

    public void AddEntity(Entity entity)
    {
        Entities.Add(entity.EntityId, entity);
    }
    public void RemoveEntity(EntityId id) { }
    public void TryGetEntity(EntityId id, out Entity e)
    {
        if (Entities.TryGetValue(id, out var entity))
        {
            e = entity;
            return;
        }

        e = null!;
    }
    public bool TryPlaceEntityOnTile(Entity e, int x, int y)
    {
        if (IsWalkableTile(x, y) && InBounds(x, y))
        {
            AddEntity(e);
            Tiles[x * y].EntityIds.Add(e.EntityId);
            e.Position = new Vector2(x, y);
            return true;
        }
        return false;
    }
    public IEnumerable<T> GetEntities<T>()
    {
        return Entities.Values.OfType<T>();
    }

    public IList<Entity> GetEntitiesPerPosition(int x, int y)
    {
        return Entities.Where(_ => _.Value.Position.Y == y && _.Value.Position.X == x).Select(_ => _.Value).ToList();
    }
}