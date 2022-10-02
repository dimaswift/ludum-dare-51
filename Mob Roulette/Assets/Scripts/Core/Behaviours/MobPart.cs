using System;
using System.Collections.Generic;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobPart : MonoBehaviour, IHitTarget, IRocketTarget
    {
        public bool Exploded => exploded;
        public bool ShouldFollow => !Deactivated;
        public Vector2 Position => body.position;
        public event Action<MobPart> OnDeactivate = m => { };
        public event Action<DecalType> OnDecalAttached = d => { };
        public Rigidbody2D Body => body;
        public bool Deactivated { get; private set; }
        public bool IsOnFire => permanentDecals.ContainsKey(DecalType.SmallFire);
        public bool IsSmoking => permanentDecals.ContainsKey(DecalType.Smoke);
        
        public bool IsMain => isMainPart;
        
       
        [SerializeField] private EffectType explosionEffect = EffectType.Explosion;
        [SerializeField] private bool isMainPart;
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private Transform firePosition;
        [SerializeField] private float dealDamageIfOnFireRate;
        
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
        private float lastHealthFireDrainTime;
        private float explodeDelay;

        private readonly Dictionary<DecalType, Decal> permanentDecals = new ();

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

        private void Update()
        {
            if (Deactivated && !exploded)
            {
                explodeDelay -= Time.deltaTime;
                if (explodeDelay <= 0)
                {
                    Explode();
                }
            }

            if (dealDamageIfOnFireRate > 0 && IsOnFire)
            {
                if (Time.time - lastHealthFireDrainTime >= dealDamageIfOnFireRate)
                {
                    DealDamage(1);
                    lastHealthFireDrainTime = Time.time;
                }
            }
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
                return;
            }

            if (health <= maxHealth / 4f && !IsOnFire)
            {
                DetachPermanentDecal(DecalType.Smoke);
                AttachPermanentDecal(DecalType.SmallFire, 3);
                return;
            }
            
            if (health <= maxHealth / 2f && !IsSmoking)
            {
                AttachPermanentDecal(DecalType.Smoke, 3);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (exploded)
            {
                return;
            }
            if (IsOnFire && collision.relativeVelocity.magnitude > 20
                         && collision.gameObject.GetComponent<IProjectile>() == null)
            {
                Explode();
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
            Body.AddTorque(Random.Range(-1000f, 1000f), ForceMode2D.Impulse);
        }

        public void Explode()
        {
            if (exploded)
            {
                return;
            }
            
            exploded = true;
            
            Deactivate(0);
            
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

            foreach (var permanentDecal in permanentDecals)
            {
                Pool<Decal>.Release(permanentDecal.Value);
            }

            permanentDecals.Clear();
            addedDecals.Clear();
            Destroy(gameObject);
        }

        public void AttachPermanentDecal(DecalType type, float size)
        {
            if (exploded)
            {
                return;
            }
            
            if (permanentDecals.ContainsKey(type))
            {
                return;
            }

            var pos = firePosition != null ? firePosition.position : transform.position;
            pos.z = decalPositionZ;
            permanentDecals[type] = DecalsPool.AddDecal(type, pos, transform,
                new DecalData()
                {
                    Duration = -1,
                    Size = size
                });
            OnDecalAttached(type);
        }

        public void DetachPermanentDecal(DecalType type)
        {
            if (!permanentDecals.ContainsKey(type))
            {
                return;
            }

            Pool<Decal>.Release(permanentDecals[type]);
            permanentDecals.Remove(type);
        }
        
        public void Deactivate(float explodeDelay)
        {
            if (Deactivated)
            {
                return;
            }
            this.explodeDelay = explodeDelay;
            transform.SetParent(null);
            Deactivated = true;
            foreach (Joint2D joint in joints)
            {
                Destroy(joint);
            }
            OnDeactivate(this);
        }

    }
}