using MobRoulette.Core.Domain;

namespace MobRoulette.Core.Interfaces
{
    public interface IGun
    {
        bool TryShoot();
        void RegisterHit(IProjectile projectile, IHitTarget target, HitPoint hitPoint);
        float CalculateDamage();
    }
}