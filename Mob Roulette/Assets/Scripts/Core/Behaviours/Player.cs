using System;
using System.Collections.Generic;
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
        public IObservableValue<int> MobsKilled => mobsKilled;
        public IObservableValue<int> MobsKilledRecord => mobsKilledRecord;

        public IObservableValue<int> Coins => coins;
        
        public IGun CurrentGun => currentGun.Gun;
        
        [SerializeField] private int maxHealth = 100;

        private readonly Observable<int> health = new(0);
        private readonly Observable<int> coins = new(0);
        private readonly Observable<int> mobsKilled = new(0);
        private readonly Observable<int> mobsKilledRecord = new(0);

        private readonly List<GunManager> guns = new();
        private GunManager currentGun;
        private Rigidbody2D body;

        private PlayerSave save;
        
        private void Awake()
        {
            coins.Set(Game.Instance.Save.Coins);
            mobsKilledRecord.Set(Game.Instance.Save.MobsKilledRecord);
            body = GetComponent<Rigidbody2D>();
            GetComponentsInChildren(guns);
            guns.Sort((g1, g2) => g1.UnlockPrice.CompareTo(g2.UnlockPrice));
            for (int i = 0; i < guns.Count; i++)
            {
                var g = guns[i];
                guns[i].Gun.Equipped.Subscribe(e =>
                {
                    if (e)
                    {
                        currentGun = g;
                    }
                });
                g.Order = i;
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

        public bool WithdrawCoins(int amount)
        {
            if (coins.Value < amount)
            {
                return false;
            }
            coins.Set(coins.Value - amount);
            return true;
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

        public void TrackKilledMob(Mob mob)
        {
            coins.Set(coins + mob.KillReward);
            mobsKilled.Set(mobsKilled.Value + 1);
            if (mobsKilledRecord.Value < mobsKilled.Value)
            {
                mobsKilledRecord.Set(mobsKilled.Value);
            }
        }

        public void EquipGun(GunManager gun)
        {
            if (gun.Unlocked.Value == false)
            {
                return;
            }
            
            foreach (var other in guns)
            {
                if (other != gun)
                {
                    other.Gun.SetEquipped(false);
                }
                else
                {
                    gun.Gun.SetEquipped(true);
                }
            }
        }
    }
}