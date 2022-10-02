using UnityEngine;

namespace MobRoulette.Core.Interfaces
{
    public interface IDefaultStateSaver
    {
        void Save();
        void Restore();
        void OnParentDestroyed();
    }
}