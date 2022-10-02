using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Utils;
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
        [SerializeField] private bool isExplosive;
        [SerializeField] private float explosionRadius;
        [SerializeField] private EffectType explosionEffect;
        [SerializeField] private DecalType decalType = DecalType.MeltedMetal;
        [SerializeField] private float projectileAcceleration;
        [SerializeField] private float extraDecalDuration;
        
        public DecalType DecalType => decalType;
        public float ExtraDecalDuration => extraDecalDuration;
        public float ProjectileAcceleration => projectileAcceleration;
        public bool IsExplosive => isExplosive;
        public  float ExplosionRadius => explosionRadius;
        public EffectType ExplosionEffect => explosionEffect;
        public float FireRate => fireRate;
        public int Damage => damage;
        public float AimSpeed => aimSpeed;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;
        public ProjectileBehaviour Projectile => projectile;
        
    }
}