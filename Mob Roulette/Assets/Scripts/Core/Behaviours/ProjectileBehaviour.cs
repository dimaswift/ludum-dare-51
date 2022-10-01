using System.Collections;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class ProjectileBehaviour : MonoBehaviour, IProjectile
    {
        public Rigidbody2D Body => body;
        public int PrefabId { get; set; }
        
        [SerializeField] private int bounces;
        [SerializeField] private float fadeOutSpeed;
        
        private Rigidbody2D body;
        private IHitTarget lastHit;
        private IGun currentGun;
        private int bouncesLeft;

        private TrailRenderer trail;
        private Vector2 trailStartPos;


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

            var target = col.collider.GetComponent<IHitTarget>();
            
            if (target == null || target == lastHit)
            {
                return;
            }

            lastHit = target;

            bouncesLeft--;

            var contact = col.GetContact(0);
            
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

       

        public void Prepare()
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

        public void CleanUp()
        {
           
        }

      
        public void Shoot(IGun gun, Vector2 point, Vector2 velocity)
        {
            currentGun = gun;
            transform.position = point;
            body.velocity = velocity;
            if (trail != null)
            {
                trail.Clear();
            }
        }

        public void OnHit(IHitTarget hit, HitPoint hitPoint)
        {
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
    }
}