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
        public event Action<MobPart> OnBeforeKilled = m => { };
        public Rigidbody2D Body => body;
        public bool IsDead { get; private set; }
        public bool IsOnFire { get; private set; }
        public bool IsMain => isMainPart;
        
        [SerializeField] private EffectType explosionEffect = EffectType.Explosion;
        [SerializeField] private bool isMainPart;
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private Transform firePosition;
        
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
        private Decal currentFireDecal;
        

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


        private void DealDamage(int damage)
        {
            health = Mathf.Max(0, health - damage);

            foreach (MobVisuals visuals in mobVisuals)
            {
                visuals.SetDamaged(1f - (float)health / maxHealth);
            }

            if (health <= 0)
            {
                Explode();
            }

            if (health <= maxHealth / 2f && !IsOnFire)
            {
                SetOnFire(true);
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
            Transform t = transform;
            Vector3 decalPos = t.InverseTransformPoint(hitPoint.Point);
            decalPos.z = decalPositionZ;
            var decal = DecalsPool.AddDecal(projectile.CurrentGun.Config.DecalType, t.TransformPoint(decalPos),
                t,
                new DecalData()
                {
                    Duration = Random.Range(1f, 3f) + proximityMultiplier + hitPoint.ExtraDecalDuration,
                    Size = (Random.Range(2f, 3f) + (proximityMultiplier * 5)) + hitPoint.ExtraDecalSize,
                    ExtraIntensity = proximityMultiplier
                });

            addedDecals.Add(decal);
            DealDamage(projectile.CurrentGun.GetProjectileInfo().Damage);
        }

        public void OnExplode(float maxRadius, int damage, Vector2 center)
        {
            var dir = (Vector2)transform.position - center;
            var dist = Mathf.FloorToInt(dir.magnitude);
            DealDamage(damage - dist);
            Body.AddForce(dir.normalized * (1000 - dist), ForceMode2D.Impulse);
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
            addedDecals.Clear();
            Destroy(gameObject);
        }

        public void SetOnFire(bool isOnFire)
        {
            if (isOnFire == IsOnFire)
            {
                return;
            }
            
            IsOnFire = isOnFire;
            if (IsOnFire)
            {
                var pos = firePosition != null ? firePosition.position : transform.position;
                pos.z = decalPositionZ;
                currentFireDecal = DecalsPool.AddDecal(DecalType.SmallFire, pos, transform,
                    new DecalData()
                    {
                        Duration = -1,
                        Size = 3f
                    });
                addedDecals.Add(currentFireDecal);
            }
            else
            {
                if (currentFireDecal != null)
                {
                    Pool<Decal>.Release(currentFireDecal);
                    currentFireDecal = null;
                }
            }
         
        }
        
        public void Kill()
        {
            if (IsDead)
            {
                return;
            }
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