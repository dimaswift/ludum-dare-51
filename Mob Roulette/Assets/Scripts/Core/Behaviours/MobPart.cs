using System;
using System.Collections.Generic;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobPart : MonoBehaviour, IHitTarget
    {
        [SerializeField] private EffectType explosionEffect = EffectType.Explosion;
        public Rigidbody2D Body => body;
        public bool IsDead { get; private set; }
        
        public bool IsMain => isMainPart;
        
        [SerializeField] private bool isMainPart;
        [SerializeField] private float destroyDelay = 3;
        [SerializeField] private int maxHealth = 10;
        public event Action<MobPart> OnBeforeKilled = m => { };
       
        public int PrefabId { get; set; }
        public bool IsInUse { get; set; }
        private Vector3? lastDecalPoint;
        private float lastDecalTime;
        private MobVisuals[] mobVisuals;
        private Rigidbody2D body;
  
        private int health;
        private bool exploded;
        private readonly List<Decal> addedDecals = new();
        
        private Joint2D[] joints;
        private float decalPositionZ;
        

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
            var decalPoint = transform.Find("DecalPoint");
            if (decalPoint != null)
            {
                decalPositionZ = decalPoint.localPosition.z;
            }
            else
            {
                decalPositionZ = -0.5f;
            }
            joints = GetComponentsInChildren<Joint2D>();
            mobVisuals = GetComponentsInChildren<MobVisuals>();

            foreach (MobVisuals visual in mobVisuals)
            {
                visual.SetDamaged(0);
            }

            health = maxHealth;
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

            Vector3 decalPos = transform.InverseTransformPoint(hitPoint.Point);
            decalPos.z = decalPositionZ;
            var decal = DecalsPool.AddDecal(DecalType.MeltedMetal, transform.TransformPoint(decalPos), transform, new DecalData()
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
                visuals.SetDamaged(1f - (float)health / maxHealth);
            }
            
            if (health <= 0)
            {
                Explode();
            }
        }

        public void Explode()
        {
            if (exploded)
            {
                return;
            }
            exploded = true;
            Kill();
            foreach (MobVisuals visuals in mobVisuals)
            {
                visuals.Explode();
            }
            Effects.Play(explosionEffect, transform.position);
            foreach (Decal decal in addedDecals)
            {
                if (!decal.IsInUse || decal == null)
                {
                    continue;
                }

                Pool<Decal>.Release(decal);
            }
            Destroy(gameObject);
        }
        
        public void Kill()
        {
            if (IsDead)
            {
                return;
            }
            addedDecals.Clear();
            transform.SetParent(null);
            IsDead = true;
            OnBeforeKilled(this);
            foreach (Joint2D joint in joints)
            {
                Destroy(joint);
            }
        }
    }
}