using MobRoulette.Core.Domain;

namespace MobRoulette.Core.Interfaces
{
    public interface IProjectile : IPooled
    {
        void SetCurrentGun(IGun gun);
        void OnHit(IHitTarget hit, HitPoint hitPoint);
        void OnExplode();
    }
}