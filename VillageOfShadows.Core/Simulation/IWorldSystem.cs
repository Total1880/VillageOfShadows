using VillageOfShadows.Core.Utils;

namespace VillageOfShadows.Core.Simulation;

public interface IWorldSystem
{
    void Update(World.World world, float dt, IRandom rng);
}