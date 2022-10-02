using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace MobRoulette.Core.Configs
{
    [CreateAssetMenu(menuName = "MobRoulette/Configs/GunConfig")]
    public class GunConfig : ScriptableObject
    {
        [Header("Gun")]
        [SerializeField] private float fireRate;
        [SerializeField] private int damage;
        [SerializeField] private float aimSpeed;
      
        
        [Header("Explosion")]
        [SerializeField] private bool isExplosive;
        [SerializeField] private float explosionRadius;
        [SerializeField] private EffectType explosionEffect;
        
        [Header("Effects")]

        [SerializeField] private EffectType hitEffect;
        [SerializeField] private DecalType decalType;
        [SerializeField] private SoundType shootSound;
        [SerializeField] private int hitEffectParticleCount;
        [SerializeField] private bool placeSmokeOnHit;
        [SerializeField] private float extraDecalDuration;
        
        [Header("Projectile")] 
        [SerializeField] private ProjectileBehaviour projectile;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileAcceleration;
        [SerializeField] private float projectileMaxSpeed;
        [SerializeField] private float projectileAutoAimSpeed;
        [SerializeField] private bool explodeOnExpire;
        [SerializeField] private float projectileLifetime;
        
        public bool PlaceSmokeOnHit => placeSmokeOnHit;
        public int HitEffectParticleCount => hitEffectParticleCount;
        public bool ExplodeOnExpire => explodeOnExpire;
        public float ProjectileMaxSpeed => projectileMaxSpeed;
        public float ProjectileAutoAimSpeed => projectileAutoAimSpeed;
        public SoundType ShootSound => shootSound;
        public DecalType DecalType => decalType;
        public EffectType HitEffect => hitEffect;
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