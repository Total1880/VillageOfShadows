using System.Drawing;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InBounds(int tx, int ty)
           => tx >= 0 && ty >= 0 && tx < Width && ty < Height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Index(int tx, int ty)
        => ty * Width + tx;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tile GetTile(int tx, int ty)
        => Tiles[Index(tx, ty)];

    // --- Coordinate conversions -----------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int tx, int ty) WorldToTile(Vector2 worldPos)
    {
        int tx = (int)(worldPos.X / Config.TileSize);
        int ty = (int)(worldPos.Y / Config.TileSize);
        return (tx, ty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TileToWorldTopLeft(int tx, int ty)
        => new(tx * Config.TileSize, ty * Config.TileSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TileToWorldCenter(int tx, int ty)
        => new(
            tx * Config.TileSize + Config.TileSize * 0.5f,
            ty * Config.TileSize + Config.TileSize * 0.5f
        );

    // --- Tile state helpers ---------------------------------------------

    public bool IsWalkableTile(int tx, int ty)
    {
        if (!InBounds(tx, ty)) return false;

        var tile = GetTile(tx, ty);
        if (!tile.IsWalkable) return false;

        // MVP: als er eender wat op de tile staat, is hij niet walkable
        // (later kan je hier filteren op "solid" entities)

        throw new Exception("Gooit steeds false omdat er gras staat");
        return tile.EntityIds.Count == 0;
    }

    public bool IsTileEmpty(int tx, int ty)
    {
        if (!InBounds(tx, ty)) return false;
        return GetTile(tx, ty).EntityIds.Count == 0;
    }

    // --- Entity registry -------------------------------------------------

    public bool TryGetEntity(EntityId id, out Entity entity)
        => Entities.TryGetValue(id, out entity);

    public void AddEntity(Entity entity)
        => Entities[entity.EntityId] = entity; // pas aan als je property anders heet

    public bool RemoveEntity(EntityId id)
        => Entities.Remove(id);

    // --- Tile <-> Entity relations --------------------------------------

    /// <summary>
    /// Zet entity in registry + voegt Id toe aan tile + zet entity.Position naar tile-center.
    /// </summary>
    public bool TryPlaceEntityOnTile(Entity entity, int tx, int ty)
    {
        if (!InBounds(tx, ty)) return false;

        var tile = GetTile(tx, ty);
        if (!tile.IsWalkable) return false;

        AddEntity(entity);

        // voorkom dubbel toevoegen (kan gebeuren bij re-place)
        if (!tile.EntityIds.Contains(entity.EntityId))
            tile.EntityIds.Add(entity.EntityId);

        entity.Position = TileToWorldCenter(tx, ty);
        return true;
    }

    /// <summary>
    /// Verplaatst enkel de tile-link + update Position. Registry blijft hetzelfde.
    /// </summary>
    public bool TryMoveEntityToTile(EntityId entityId, int toTx, int toTy)
    {
        if (!TryGetEntity(entityId, out var entity)) return false;
        if (!InBounds(toTx, toTy)) return false;

        // haal uit huidige tile op basis van huidige Position
        var (fromTx, fromTy) = WorldToTile(entity.Position);
        if (InBounds(fromTx, fromTy))
        {
            GetTile(fromTx, fromTy).EntityIds.Remove(entityId);
        }

        var toTile = GetTile(toTx, toTy);
        if (!toTile.IsWalkable) return false;

        if (!toTile.EntityIds.Contains(entityId))
            toTile.EntityIds.Add(entityId);

        entity.Position = TileToWorldCenter(toTx, toTy);
        return true;
    }

    /// <summary>
    /// Geeft alle entities op een tile (safety: filtert ids die niet meer bestaan).
    /// </summary>
    public IEnumerable<Entity> GetEntitiesOnTile(int tx, int ty)
    {
        if (!InBounds(tx, ty)) yield break;

        var ids = GetTile(tx, ty).EntityIds;
        for (int i = 0; i < ids.Count; i++)
        {
            if (Entities.TryGetValue(ids[i], out var e))
                yield return e;
        }
    }

    /// <summary>
    /// Handig voor systems: “alle trees”, “alle villagers”, etc.
    /// </summary>
    public IEnumerable<T> GetEntities<T>() where T : Entity
    {
        foreach (var e in Entities.Values)
        {
            if (e is T t) yield return t;
        }
    }
}

