using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;

namespace VillageOfShadows.Core.Simulation;

public interface IWorldSystem
{
    void Update(World.World world, float dt, IRandom rng);
}