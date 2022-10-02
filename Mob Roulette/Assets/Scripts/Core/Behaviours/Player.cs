using System;
using Core;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class Player : MonoBehaviour, IHitTarget, IRocketTarget
    {
        public bool ShouldFollow => health > 0;
        public Vector2 Position => body.position;
        public IObservableValue<int> Health => health;

        [SerializeField] private int maxHealth = 100;

        private readonly Observable<int> health = new(0);

        private IGun[] guns;
        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            guns = GetComponentsInChildren<IGun>();
            for (int i = 0; i < guns.Length; i++)
            {
                guns[i].SetEquipped(i == 0);
            }
            health.Subscribe(h =>
            {
                if (h <= 0)
                {
                    Effects.Play(EffectType.Explosion, transform.position);
                    Destroy(gameObject);
                    Game.Instance.GameOver();
                }
            });
        }

        public void OnHit(IProjectile projectile, HitPoint hitPoint)
        {
            health.Set(health - projectile.CurrentGun.GetProjectileInfo().Damage);
        }

        public void OnExplode(float maxRadius, int damage, Vector2 center)
        {
            var dir = body.position - center;
            body.AddForce(dir.normalized * (1000 / Mathf.Max(1, dir.magnitude)), ForceMode2D.Impulse);
            health.Set(health - damage);
        }

        public void Prepare()
        {
            health.Set(maxHealth);
        }

    }
}