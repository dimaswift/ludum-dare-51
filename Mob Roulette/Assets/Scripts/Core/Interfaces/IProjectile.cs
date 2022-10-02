using MobRoulette.Core.Domain;
using UnityEngine;

namespace MobRoulette.Core.Interfaces
{
    public interface IProjectile : IPooled
    {
        void Shoot(IGun gun, Vector2 point, Vector2 direction);
        void OnHit(IHitTarget hit, HitPoint hitPoint);
        void OnExplode();
        Color HitColor { get; }
        int Damage { get; set; }
    }
}