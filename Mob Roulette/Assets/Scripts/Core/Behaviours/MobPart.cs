using System;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobPart : MonoBehaviour, IHitTarget
    {
        private Vector3? lastDecalPoint;
        private float lastDecalTime;

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
                Duration = Random.Range(0.5f, 2f) + proximityMultiplier,
                Size = Random.Range(1f, 1.5f) + proximityMultiplier * 5,
                ExtraIntensity = proximityMultiplier
            });
        }

        public void CleanUp()
        {
            lastDecalPoint = null;
        }
    }
}