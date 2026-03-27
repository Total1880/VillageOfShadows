
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Game.State
{
    public sealed class BuildState
    {
        public bool IsBuildMenuOpen { get; set; }
        public BuildType SelectedBuild { get; set; } = BuildType.None;

        public bool IsPlacing => SelectedBuild != BuildType.None;

        public void StartPlacement(BuildType buildType)
        {
            SelectedBuild = buildType;
            IsBuildMenuOpen = false;
        }

        public void CancelPlacement()
        {
            SelectedBuild = BuildType.None;
        }
    }
}
