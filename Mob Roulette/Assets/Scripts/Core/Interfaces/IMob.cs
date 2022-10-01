using System.Collections.Generic;
using MobRoulette.Core.Behaviours;

namespace MobRoulette.Core.Interfaces
{
    public interface IMob : IPooled
    {
        void Build(List<MobPart> partPrefabs);
    }
}