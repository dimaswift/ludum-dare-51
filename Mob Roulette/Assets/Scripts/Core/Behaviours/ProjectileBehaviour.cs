using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class ProjectileBehaviour : MonoBehaviour, IProjectile
    {
        public Color HitColor => hitColor;
        public float Damage { get; set; }
        public Rigidbody2D Body => body;
        public int PrefabId { get; set; }
        public bool IsInUse { get; set; }

        [SerializeField] private int bounces;

        [ColorUsage(true, true)]
        [SerializeField] private Color hitColor;
        
        
        private Rigidbody2D body;
        private IHitTarget lastHit;
        private IGun currentGun;
        private int bouncesLeft;

        private TrailRenderer trail;
        private Vector2 trailStartPos;

        private float lastBounceTime;

        public void OnDestroy()
        {
            if (trail != null && trail.transform.parent != transform)
            {
                Destroy(trail.gameObject);
            }
        }

        public void Init()
        {
            trail = GetComponentInChildren<TrailRenderer>();
            body = GetComponent<Rigidbody2D>();
            trailStartPos = trail.transform.localPosition;
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
                    OnExplode();
                }
                return;
            }

            lastHit = target;


            OnHit(target, new HitPoint()
            {
                Point = Body.position,
                Impulse = contact.normalImpulse,
                Normal = contact.normal
            });

            if (bouncesLeft <= 0)
            {
                OnExplode();
            }
        }
        

        public void OnCleanUp()
        {
            
        }

      
        public void Shoot(IGun gun, Vector2 point, Vector2 velocity)
        {
            currentGun = gun;
            Damage = currentGun.CalculateDamage();
            transform.position = new Vector3(point.x,point.y, -1);
            body.velocity = velocity;
            if (trail != null)
            {
                trail.Clear();
            }
        }

        public void OnHit(IHitTarget hit, HitPoint hitPoint)
        {
            Effects.Emit(EffectType.Spark, 30, hitPoint.Point, hitPoint.Normal);
            Effects.Play(EffectType.Smoke, hitPoint.Point);
            currentGun.RegisterHit(this, hit, hitPoint);
            hit.OnHit(this, hitPoint);
        }

        public void OnExplode()
        {
            if (trail != null)
            {
                trail.transform.SetParent(null);
            }
            Pool<ProjectileBehaviour>.Release(this);
        }


        public void Reuse()
        {
            bouncesLeft = bounces;
            body.velocity = Vector3.zero;
            body.angularVelocity = 0;
            lastHit = null;
            if (trail != null)
            {
                var trailTransform = trail.transform;
                trailTransform.SetParent(transform);
                trailTransform.localPosition = trailStartPos;
            }
        }
    }
}