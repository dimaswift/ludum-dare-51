using UnityEngine;

namespace MobRoulette.Core.Interfaces
{
    public interface IRocketTarget
    {
        bool ShouldFollow { get; }
        Vector2 Position { get; }
    }
}