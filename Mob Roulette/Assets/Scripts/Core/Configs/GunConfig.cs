using MobRoulette.Core.Behaviours;
using UnityEngine;
using UnityEngine.Pool;

namespace MobRoulette.Core.Configs
{
    [CreateAssetMenu(menuName = "MobRoulette/Configs/GunConfig")]
    public class GunConfig : ScriptableObject
    {
        [SerializeField] private float fireRate;
        [SerializeField] private int damage;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float aimSpeed;
        [SerializeField] private float projectileLifetime;
        [SerializeField] private ProjectileBehaviour projectile;

        public float FireRate => fireRate;
        public int Damage => damage;
        public float AimSpeed => aimSpeed;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;
        public ProjectileBehaviour Projectile => projectile;
        
    }
}