using MobRoulette.Core.Configs;
using MobRoulette.Core.Domain;
using UnityEngine;

namespace MobRoulette.Core.Interfaces
{
    public interface IGun
    {
        bool TryShoot();
        void Aim(Vector2 target);
        void SetEquipped(bool equipped);
        void RegisterHit(IProjectile projectile, IHitTarget target, HitPoint hitPoint);
        ProjectileInfo GetProjectileInfo();
        GunConfig Config { get; }
    }
}