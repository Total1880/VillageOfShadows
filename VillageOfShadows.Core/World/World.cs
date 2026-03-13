using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using VillageOfShadows.Core.Config;
using VillageOfShadows.Core.Entities;

namespace VillageOfShadows.Core.World;

public sealed class World
{
    private readonly List<Job> _jobs = new();

    public int Width { get; }
    public int Height { get; }
    public WorldConfig Config { get; }
    public Tile[] Tiles { get; }
    public Resources Resources { get; }

    public Dictionary<EntityId, Entity> Entities { get; } = new();
    public IReadOnlyList<Job> Jobs => _jobs;
    private readonly List<Entity> _pendingAdds = new();
    private readonly List<EntityId> _pendingRemoves = new();
    public IEnumerable<TileEntity> GetTileEntitiesOnTile(int tx, int ty)
    {
        var tile = GetTile(tx, ty);

        foreach (var id in tile.EntityIds)
        {
            if (Entities.TryGetValue(id, out var entity) && entity is TileEntity te)
                yield return te;
        }
    }

    public IEnumerable<TActor> GetActors<TActor>() where TActor : Actor
    {
        return Entities.Values.OfType<TActor>();
    }

    public World(int width, int height, WorldConfig config)
    {
        Width = width;
        Height = height;
        Config = config;
        Resources = new Resources();

        Tiles = new Tile[width * height];
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i] = new Tile { Type = TileType.Grass };
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InBounds(int tx, int ty) => tx >= 0 && ty >= 0 && tx < Width && ty < Height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Index(int tx, int ty) => ty * Width + tx;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tile GetTile(int tx, int ty) => Tiles[Index(tx, ty)];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int tx, int ty) WorldToTile(Vector2 worldPos)
    {
        int tx = (int)(worldPos.X / Config.TileSize);
        int ty = (int)(worldPos.Y / Config.TileSize);
        return (tx, ty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TileToWorldTopLeft(int tx, int ty) =>
        new(tx * Config.TileSize, ty * Config.TileSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TileToWorldCenter(int tx, int ty) =>
        new(
            tx * Config.TileSize + Config.TileSize * 0.5f,
            ty * Config.TileSize + Config.TileSize * 0.5f
        );

    public bool IsWalkableTile(int tx, int ty)
    {
        if (!InBounds(tx, ty))
            return false;

        var tile = GetTile(tx, ty);
        if (!tile.IsWalkable)
            return false;

        return true;
    }

    public bool IsTileEmpty(int tx, int ty)
    {
        if (!InBounds(tx, ty))
            return false;

        return GetTile(tx, ty).EntityIds.Count == 0;
    }

    public bool TryGetEntity(EntityId id, out Entity entity) =>
        Entities.TryGetValue(id, out entity!);

    public void AddEntity(Entity entity) => _pendingAdds.Add(entity);

    public bool RemoveEntity(EntityId id)
    {
        if (!Entities.TryGetValue(id, out var entity))
            return false;

        var (tx, ty) = WorldToTile(entity.Position);
        if (InBounds(tx, ty))
        {
            GetTile(tx, ty).EntityIds.Remove(id);
        }

        _pendingRemoves.Add(id);
        return true;
    }

    public bool TryPlaceTileEntity(TileEntity entity, int tx, int ty)
    {
        if (!InBounds(tx, ty)) return false;

        var tile = GetTile(tx, ty);
        if (!tile.IsWalkable) return false;

        AddEntity(entity);

        entity.Tile = new Point(tx, ty);
        entity.Position = TileToWorldCenter(tx, ty);

        if (!tile.EntityIds.Contains(entity.EntityId))
            tile.EntityIds.Add(entity.EntityId);

        return true;
    }

    public bool TrySpawnActor(Actor actor, int tx, int ty)
    {
        if (!InBounds(tx, ty)) return false;

        var tile = GetTile(tx, ty);
        if (!tile.IsWalkable) return false;

        AddEntity(actor);
        actor.SetPosition(TileToWorldCenter(tx, ty));

        return true;
    }

    public IEnumerable<T> GetEntities<T>() where T : Entity
    {
        foreach (var entity in Entities.Values)
        {
            if (entity is T typed)
            {
                yield return typed;
            }
        }
    }

    public void AddJob(Job job) => _jobs.Add(job);

    public IEnumerable<T> GetJobs<T>() where T : Job
       => _jobs.OfType<T>();

    public void RemoveCompletedJobs()
    {
        _jobs.RemoveAll(j => j.IsCompleted);
    }

    public void RemoveEntityDeferred(EntityId id) => _pendingRemoves.Add(id);

    public void FlushEntityChanges()
    {
        foreach (var entity in _pendingAdds)
            Entities[entity.EntityId] = entity;

        foreach (var id in _pendingRemoves)
            Entities.Remove(id);

        _pendingAdds.Clear();
        _pendingRemoves.Clear();
    }
}