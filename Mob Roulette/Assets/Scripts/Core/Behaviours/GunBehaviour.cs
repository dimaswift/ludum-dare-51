using System.Collections;
using MobRoulette.Core.Configs;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class GunBehaviour : MonoBehaviour, IGun
    {
        [SerializeField] private float minDistanceToShoot;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GunConfig config;
        [SerializeField] private float maxAngle;
        private float lastShotTime;
        private Vector2 targetPoint;
        private float currentAimAngle;
        
        public bool TryShoot()
        {
            if (Time.time - lastShotTime < config.FireRate)
            {
                return false;
            }
            var projectileBehaviour = Pool<ProjectileBehaviour>.GetFromPool(config.Projectile);
            projectileBehaviour.Shoot(this, projectileSpawnPoint.position, projectileSpawnPoint.up * config.ProjectileSpeed);
            lastShotTime = Time.time;
            EventBus.Raise<OnGunShot, (IProjectile, IGun)>((projectileBehaviour, this));
            return true;
        } 

        public void Aim(Vector2 target)
        {
            targetPoint = target;
        }

        protected virtual void Update()
        {
            var distanceToTarget = Vector2.Distance(targetPoint, transform.position);

            if (minDistanceToShoot > 0 && distanceToTarget > minDistanceToShoot)
            {
                return;
            }
            
            var turretPos = transform.position;
            var angle =
                Mathf.Atan2(targetPoint.y - turretPos.y, targetPoint.x - turretPos.x) * Mathf.Rad2Deg -
                90;
            currentAimAngle = Mathf.LerpAngle(currentAimAngle, angle, Time.fixedDeltaTime * config.AimSpeed);
            if (maxAngle > 0)
            {
                currentAimAngle = Utils.Utils.NormalizeAngle(Mathf.Clamp(currentAimAngle, -maxAngle, maxAngle));
            }
            transform.eulerAngles = new Vector3(0, 0, currentAimAngle);
        }
        
        public void RegisterHit(IProjectile projectile, IHitTarget hit, HitPoint hitPoint)
        {
            
        }

        public ProjectileInfo GetProjectileInfo()
        {
            return new ProjectileInfo()
            {
                Damage = config.Damage,
                Speed = config.ProjectileSpeed,
                Lifetime = config.ProjectileLifetime
            };
        }

        public float CalculateDamage()
        {
            return config.Damage;
        }
    }
}



