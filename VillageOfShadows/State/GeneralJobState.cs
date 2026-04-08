
using System;
using VillageOfShadows.Core.Entities.Components;

namespace VillageOfShadows.Game.State
{
    public sealed class GeneralJobState
    {
        public bool IsGeneralJobMenuOpen { get; set; }
        public GeneralJobType SelectedGeneralJob { get; set; } = GeneralJobType.None;

        public bool IsPlacing => SelectedGeneralJob != GeneralJobType.None;

        internal void StartJob(GeneralJobType generalJobType)
        {
            SelectedGeneralJob = generalJobType;
            IsGeneralJobMenuOpen = false;
        }
    }
}
