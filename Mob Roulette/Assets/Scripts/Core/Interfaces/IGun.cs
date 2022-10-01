using MobRoulette.Core.Domain;

namespace MobRoulette.Core.Interfaces
{
    public interface IGun
    {
        bool TryShoot(out IProjectile projectile);
        void RegisterHit(IProjectile projectile, IHitTarget target, HitPoint hitPoint);
        float CalculateDamage();
    }
}