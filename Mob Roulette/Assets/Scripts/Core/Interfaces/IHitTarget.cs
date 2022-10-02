using MobRoulette.Core.Domain;
using UnityEngine;

namespace MobRoulette.Core.Interfaces
{
    public interface IHitTarget
    {
        void OnHit(IProjectile projectile, HitPoint hitPoint);
        void OnExplode(float maxRadius, int damage, Vector2 center);
    }
}