using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.Entities.Jobs
{
    public sealed class HaulingJob(EntityId sourceStockpileId, EntityId destinationStockpileId) : Job
    {
        public EntityId SourceStockpileId { get; init; } = sourceStockpileId;
        public EntityId DestinationStockpileId { get; init; } = destinationStockpileId;
        public HaulingJobState State { get; set; } = HaulingJobState.MovingToSource;
    }

    public enum HaulingJobState
    {
        MovingToSource,
        MovingToDestination
    }
}
