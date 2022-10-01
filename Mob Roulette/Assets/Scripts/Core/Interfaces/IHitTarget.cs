using MobRoulette.Core.Domain;

namespace MobRoulette.Core.Interfaces
{
    public interface IHitTarget
    {
        void OnHit(IProjectile projectile, HitPoint hitPoint);
    }
}