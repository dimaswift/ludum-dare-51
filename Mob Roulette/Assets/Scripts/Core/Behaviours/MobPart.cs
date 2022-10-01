using System;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobPart : MonoBehaviour, IHitTarget, IReusable
    {
        private Vector3? lastDecalPoint;
        private float lastDecalTime;
        private MobVisuals[] mobVisuals;

        private float health;

        private void Awake()
        {
            mobVisuals = GetComponentsInChildren<MobVisuals>();
        }

        private void Start()
        {
            Reuse();
        }

        public void OnHit(IProjectile projectile, HitPoint hitPoint)
        {
            float proximityMultiplier = 0;
            
            if (lastDecalPoint.HasValue && Time.time - lastDecalTime < 1f)
            {
                var dist = Vector2.Distance(lastDecalPoint.Value, hitPoint.Point);
                proximityMultiplier = (1f - Mathf.Min(1, Time.time - lastDecalTime)) * Mathf.Clamp(1 - dist, 0, 1);
            }
            
            lastDecalPoint = hitPoint.Point;
            lastDecalTime = Time.time;
 
            DecalsPool.AddDecal(DecalType.MeltedMetal, hitPoint.Point, transform, new DecalData()
            {
                Color = projectile.HitColor,
                Duration = Random.Range(0.5f, 1f) + proximityMultiplier,
                Size = Random.Range(1f, 1.5f) + proximityMultiplier * 3,
                ExtraIntensity = proximityMultiplier
            });
            
            health = Mathf.Max(0, health - projectile.Damage);

            foreach (MobVisuals visuals in mobVisuals)
            {
                visuals.SetDamaged(1f - health);
            }
            
            if (health <= 0)
            {
                Explode();
            }
        }

        public void Explode()
        {
            foreach (MobVisuals visuals in mobVisuals)
            {
                visuals.Explode();
            }
            gameObject.SetActive(false);
            Effects.Play(EffectType.Explosion, transform.position);
        }
        

        public void Reuse()
        {
            gameObject.SetActive(true);
            lastDecalTime = 0;
            lastDecalPoint = null;
            foreach (MobVisuals visuals in mobVisuals)
            {
                visuals.SetDamaged(0);
                visuals.Reuse();
            }
            health = 1;
        }
    }
}