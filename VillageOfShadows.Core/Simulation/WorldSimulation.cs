using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Core.Simulation;

public sealed class WorldSimulation
{
    private readonly List<IWorldSystem> _systems = new();
    private readonly IRandom _rng;

    public WorldSimulation(IRandom rng) => _rng = rng;

    public WorldSimulation AddSystem(IWorldSystem system)
    {
        _systems.Add(system);
        return this;
    }

    public void Update(World.World world, float dt)
    {
        for (int i = 0; i < _systems.Count; i++)
            _systems[i].Update(world, dt, _rng);
    }
}