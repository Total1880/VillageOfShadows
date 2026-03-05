using System;
using System.Collections.Generic;
using System.Text;

namespace VillageOfShadows.Core.Entities
{
    public class Grass : Entity
    {
        public override Entity Create()
        {
            return new Grass();
        }
    }
}
