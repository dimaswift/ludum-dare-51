using System;
using System.Collections.Generic;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobPart : MonoBehaviour, IHitTarget, IPooled
    {
        [SerializeField] private float maxHealth = 1f;
        public int PrefabId { get; set; }
        public bool IsInUse { get; set; }
        private Vector3? lastDecalPoint;
        private float lastDecalTime;
        private MobVisuals[] mobVisuals;

        private float health;
        private readonly List<Decal> addedDecals = new();
        private MobPartRandomizer[] randomizers;
        

        private void Awake()
        {
            randomizers = GetComponents<MobPartRandomizer>();
            mobVisuals = GetComponentsInChildren<MobVisuals>();
        }

        private void Start()
        {
            Reuse();
        }

        public void Randomize()
        {
            foreach (MobPartRandomizer randomizer in randomizers)
            {
                randomizer.Randomize();
            }
            foreach (MobVisuals visual in mobVisuals)
            {
                visual.ApplyNewScale();
            }
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
 
            var decal = DecalsPool.AddDecal(DecalType.MeltedMetal, hitPoint.Point, transform, new DecalData()
            {
                Color = projectile.HitColor,
                Duration = Random.Range(1f, 3f) + proximityMultiplier,
                Size = Random.Range(2f, 3f) + proximityMultiplier * 5,
                ExtraIntensity = proximityMultiplier
            });
            
            addedDecals.Add(decal);
            
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
            Effects.Play(EffectType.Explosion, transform.position);
            Pool<MobPart>.Release(this);
        }
        

        public void Reuse()
        {
            foreach (Decal decal in addedDecals)
            {
                if (!decal.IsInUse)
                {
                    continue;
                }
                Pool<Decal>.Release(decal);
            }

            addedDecals.Clear();
            gameObject.SetActive(true);
            lastDecalTime = 0;
            lastDecalPoint = null;
            foreach (MobVisuals visuals in mobVisuals)
            {
                visuals.SetDamaged(0);
                visuals.Reuse();
            }
            health = maxHealth;
        }
        
        public void OnCleanUp()
        {
            
        }

        public void OnDestroy()
        {
            
        }

        public void Init()
        {
            
        }
    }
}