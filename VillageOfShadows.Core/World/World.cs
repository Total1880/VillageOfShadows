using System.Numerics;
using System.Runtime.CompilerServices;
using VillageOfShadows.Core.Config;
using VillageOfShadows.Core.Entities;

namespace VillageOfShadows.Core.World;

public sealed class World
{
    public int Width { get; }
    public int Height { get; }
    public WorldConfig Config { get; }
    public Tile[] Tiles { get; }

    public Dictionary<EntityId, Entity> Entities { get; } = new();

    public World(int width, int height, WorldConfig config)
    {
        Width = width;
        Height = height;
        Config = config;

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

        foreach (var entity in GetEntitiesOnTile(tx, ty))
        {
            if (entity.BlocksMovement)
                return false;
        }

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

    public void AddEntity(Entity entity)
    {
        Entities[entity.EntityId] = entity;
    }

    public bool RemoveEntity(EntityId id)
    {
        if (!Entities.TryGetValue(id, out var entity))
            return false;

        var (tx, ty) = WorldToTile(entity.Position);
        if (InBounds(tx, ty))
        {
            GetTile(tx, ty).EntityIds.Remove(id);
        }

        return Entities.Remove(id);
    }

    public bool TryPlaceEntityOnTile(Entity entity, int tx, int ty)
    {
        if (!IsWalkableTile(tx, ty))
            return false;

        AddEntity(entity);

        var tile = GetTile(tx, ty);
        if (!tile.EntityIds.Contains(entity.EntityId))
        {
            tile.EntityIds.Add(entity.EntityId);
        }

        entity.Position = TileToWorldCenter(tx, ty);
        return true;
    }

    public bool TryMoveEntityToTile(EntityId entityId, int toTx, int toTy)
    {
        if (!TryGetEntity(entityId, out var entity))
            return false;

        if (!IsWalkableTile(toTx, toTy))
            return false;

        var (fromTx, fromTy) = WorldToTile(entity.Position);

        if (InBounds(fromTx, fromTy))
        {
            GetTile(fromTx, fromTy).EntityIds.Remove(entityId);
        }

        var toTile = GetTile(toTx, toTy);
        if (!toTile.EntityIds.Contains(entityId))
        {
            toTile.EntityIds.Add(entityId);
        }

        entity.Position = TileToWorldCenter(toTx, toTy);
        return true;
    }

    public IEnumerable<Entity> GetEntitiesOnTile(int tx, int ty)
    {
        if (!InBounds(tx, ty))
            yield break;

        var ids = GetTile(tx, ty).EntityIds;
        for (int i = 0; i < ids.Count; i++)
        {
            if (Entities.TryGetValue(ids[i], out var entity))
            {
                yield return entity;
            }
        }
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
}