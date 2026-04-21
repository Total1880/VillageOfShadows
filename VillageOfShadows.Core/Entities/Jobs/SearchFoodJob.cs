using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.Entities.Jobs
{
    public sealed class SearchFoodJob : Job
    {
        public EntityId Target { get; set; }
        public SearchFoodJobState State { get; set; } = SearchFoodJobState.SearchingForFood;
    }

    public enum SearchFoodJobState
    {
        SearchingForFood,
        MovingToFood,
        Eating
    }
}
