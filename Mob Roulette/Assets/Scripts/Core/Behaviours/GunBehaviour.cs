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
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GunConfig config;

        private float lastShotTime;
        
        public bool TryShoot(out IProjectile projectile)
        {
            if (Time.time - lastShotTime < config.FireRate)
            {
                projectile = null;
                return false;
            }
            var projectileBehaviour = Pool<ProjectileBehaviour>.GetFromPool(config.Projectile);
            projectileBehaviour.transform.SetPositionAndRotation(projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            projectileBehaviour.Body.velocity = transform.up * config.ProjectileSpeed;
            projectileBehaviour.SetCurrentGun(this);
            lastShotTime = Time.time;
            projectile = projectileBehaviour;
            EventBus.Raise<OnGunShot, (IProjectile, IGun)>((projectile, this));
            return true;
        }

        public void RegisterHit(IProjectile projectile, IHitTarget hit, HitPoint hitPoint)
        {
            
        }
    }
}



