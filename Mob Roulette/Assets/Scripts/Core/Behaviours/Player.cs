using Core;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class Player : MonoBehaviour, IHitTarget
    {
        public IObservableValue<int> Health => health;

        [SerializeField] private int maxHealth = 100;

        private readonly Observable<int> health = new(0);

        public void OnHit(IProjectile projectile, HitPoint hitPoint)
        {
            health.Set(health - projectile.Damage);
            if (health.Value <= 0)
            {
                Effects.Play(EffectType.Explosion, transform.position);
                Destroy(gameObject);
                Game.Instance.GameOver();
            }
        }
        
        

        public void Prepare()
        {
            health.Set(maxHealth);
        }
    }
}