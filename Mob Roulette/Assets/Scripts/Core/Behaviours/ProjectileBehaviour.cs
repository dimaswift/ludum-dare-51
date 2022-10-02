using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class ProjectileBehaviour : MonoBehaviour, IProjectile
    {
        [SerializeField] private int bounces;
        public int Damage { get; set; }
        public IGun CurrentGun => currentGun;

        public Rigidbody2D Body => body;
        public int PrefabId { get; set; }
        public bool IsInUse { get; set; }

        
        private Rigidbody2D body;
        private IHitTarget lastHit;
        private IGun currentGun;
        private bool exploded;
        private int bouncesLeft;

        private TrailRenderer trail;
        private ParticleSystem effect;

        private float lifeTime;
        private bool expired;
        private float lastBounceTime;
        
        private IDefaultStateSaver[] positionSavers;
        private IRocketTarget followTarget;
        private float currentAimAcceleration;

        public void SetFollowTarget(IRocketTarget target)
        {
            followTarget = target;
        }


        public void OnDestroy()
        {
            foreach (IDefaultStateSaver saver in positionSavers)
            {
                saver.OnParentDestroyed();
            }
        }

        public void Init()
        {
            positionSavers = GetComponentsInChildren<IDefaultStateSaver>();
            foreach (IDefaultStateSaver positionSaver in positionSavers)
            {
                positionSaver.Save();
            }
            trail = GetComponentInChildren<TrailRenderer>();
            effect = GetComponentInChildren<ParticleSystem>();
            body = GetComponent<Rigidbody2D>();
        }
        
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (bouncesLeft <= 0)
            {
                return;
            }

            var contact = col.GetContact(0);
            
            if (Time.time - lastBounceTime >= 0.1f)
            {
                Effects.Emit(EffectType.Spark, 3, transform.position, contact.normal);
            }

            lastBounceTime = Time.time;
            
            var target = col.collider.GetComponent<IHitTarget>();

            bouncesLeft--;

            if (target == null || target == lastHit)
            {
                if (bouncesLeft <= 0)
                {
                    Effects.Emit(EffectType.Spark, 5, contact.point, contact.normal);
                    OnExplode(contact.normal);
                }
                return;
            }

            lastHit = target;


            OnHit(target, new HitPoint()
            {
                Point = Body.position,
                Impulse = contact.normalImpulse,
                Normal = contact.normal,
                ExtraDecalSize = currentGun.Config.IsExplosive ? currentGun.Config.ExplosionRadius : 1f,
                ExtraDecalDuration = currentGun.Config.ExtraDecalDuration
            });

            if (bouncesLeft <= 0)
            {
                OnExplode(contact.normal);
            }
        }
        

        public void OnCleanUp()
        {
            
        }

      
        public void Shoot(IGun gun, Vector2 point, Vector2 velocity)
        {
            currentGun = gun;
            var info = currentGun.GetProjectileInfo();
            lifeTime = info.Lifetime;
            Damage = info.Damage;
            transform.position = new Vector3(point.x,point.y, -1);
            body.velocity = velocity;
            if (trail != null)
            {
                trail.Clear();
            }
            if (effect != null)
            {
                effect.Play();
            }
        }

        private void ProcessAutoFollow()
        {
            if (expired)
            {
                return;
            }
            
            if (followTarget == null)
            {
                return;
            }

            if (followTarget.ShouldFollow == false)
            {
                followTarget = null;
                return;
            }

            var pos = transform.position;
            var targetPoint = followTarget.Position;
            var angle =
                Mathf.Atan2(targetPoint.y - pos.y, targetPoint.x - pos.x) * Mathf.Rad2Deg -
                90;
            currentAimAcceleration = Mathf.Lerp(currentAimAcceleration, currentGun.Config.ProjectileAutoAimSpeed,
                Time.deltaTime * 5);
            body.MoveRotation(Mathf.LerpAngle(body.rotation, angle,
                Time.deltaTime * currentAimAcceleration));
            body.velocity = transform.up * body.velocity.magnitude;
        }

        private void ProcessAcceleration()
        {
            if (expired || exploded || currentGun == null)
            {
                return;
            }
           
            if (currentGun.Config.ProjectileAcceleration <= 0)
            {
                return;
            }

            var velocity = body.velocity;
            velocity += velocity.normalized * (currentGun.Config.ProjectileAcceleration * Time.deltaTime);
            body.velocity = Vector2.ClampMagnitude(velocity, currentGun.Config.ProjectileMaxSpeed);
        }

        private void ProcessExpiration()
        {
            if (expired || exploded)
            {
                return;
            }
            lifeTime -= Time.deltaTime;
            if (lifeTime > 0)
            {
                return;
            }

            expired = true;
            if (currentGun.Config.ExplodeOnExpire)
            {
                OnExplode(transform.forward);
                return;
            }
            
            DetachEffects();
            body.gravityScale = 1;
        }
        
        private void Update()
        {
            ProcessExpiration();
            ProcessAutoFollow();
            ProcessAcceleration();
            
        }

        public void OnHit(IHitTarget hit, HitPoint hitPoint)
        {
            Effects.Emit(EffectType.Spark, 30, hitPoint.Point, hitPoint.Normal);
            Effects.Play(EffectType.Smoke, hitPoint.Point);
            currentGun.RegisterHit(this, hit, hitPoint);
            hit.OnHit(this, hitPoint);
        }

        private void DetachEffects()
        {
            if (trail != null)
            {
                trail.transform.SetParent(null);
            }

            if (effect != null)
            {
                effect.transform.SetParent(null);
            }
        }

        public void OnExplode(Vector2 normal)
        {
            if (exploded)
            {
                return;
            }

            if (currentGun.Config.IsExplosive)
            {
                Effects.Play(currentGun.Config.ExplosionEffect, transform.position, normal);
                Effects.Explode(transform.position, currentGun.Config.ExplosionRadius, currentGun.Config.Damage);
            }

            DetachEffects();
            exploded = true;
            
            Pool<ProjectileBehaviour>.Release(this);
        }


        public void Reuse()
        {
            expired = false;
            currentAimAcceleration = 0;
            followTarget = null;
            if (effect != null)
            {
                effect.Pause();
            }
            foreach (IDefaultStateSaver positionSaver in positionSavers)
            {
                positionSaver.Restore();
            }
            exploded = false;
            bouncesLeft = bounces;
            body.velocity = Vector3.zero;
            body.angularVelocity = 0;
            lastHit = null;
        }
    }
}