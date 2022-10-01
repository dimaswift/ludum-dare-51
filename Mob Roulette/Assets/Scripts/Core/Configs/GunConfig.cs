using MobRoulette.Core.Behaviours;
using UnityEngine;
using UnityEngine.Pool;

namespace MobRoulette.Core.Configs
{
    [CreateAssetMenu(menuName = "MobRoulette/Configs/GunConfig")]
    public class GunConfig : ScriptableObject
    {
        [SerializeField] private float fireRate;
        [SerializeField] private float damage;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float aimSpeed;
        [SerializeField] private ProjectileBehaviour projectile;

        public float FireRate => fireRate;
        public float Damage => damage;
        public float AimSpeed => aimSpeed;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileBehaviour Projectile => projectile;
        
    }
}