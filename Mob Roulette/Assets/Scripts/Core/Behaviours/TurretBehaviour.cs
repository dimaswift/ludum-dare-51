using System.Collections;
using MobRoulette.Core.Configs;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public sealed class TurretBehaviour : MonoBehaviour, IGun
    {
        [SerializeField] private float shootVolume = 0.5f;
        [SerializeField] private float minDistanceToShoot;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GunConfig config;
        [SerializeField] private float maxAngle;
        private float lastShotTime;
        private Vector2 targetPoint;
        private float currentAimAngle;
        public GunConfig Config => config;
        public GameObject Owner { get; set; }

        public bool TryShoot(out IProjectile projectile)
        {
            if (Time.time - lastShotTime < config.FireRate)
            {
                projectile = null;
                return false;
            }
            var projectileBehaviour = Pool<ProjectileBehaviour>.GetFromPool(config.Projectile);
            projectileBehaviour.Shoot(this, projectileSpawnPoint.position, projectileSpawnPoint.up * config.ProjectileSpeed);
            projectileBehaviour.transform.rotation = projectileSpawnPoint.rotation;
            lastShotTime = Time.time;
            Sounds.PlaySound(config.ShootSound, projectileSpawnPoint.position, shootVolume);
            EventBus.Raise<OnGunShot, (IProjectile, IGun)>((projectileBehaviour, this));
            projectile = projectileBehaviour;
            return true;
        }
        
        public void Aim(Vector2 target)
        {
            targetPoint = target;
        }

        public void SetEquipped(bool equipped)
        {
            gameObject.SetActive(equipped);
        }

        private void Update()
        {
            if (targetPoint == Vector2.zero)
                return;
            
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

       
    }
}



