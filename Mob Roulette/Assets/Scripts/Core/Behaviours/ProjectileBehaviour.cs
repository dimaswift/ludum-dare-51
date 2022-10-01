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
   
        private Rigidbody2D body;
        private IHitTarget lastHit;
        private IGun currentGun;
        private int bouncesLeft;
        
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
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
            
            OnHit(target, new HitPoint()
            {
                Point = Body.position,
                Impulse = col.GetContact(0).normalImpulse,
                Normal = col.GetContact(0).normal
            });

            if (bouncesLeft <= 0)
            {
                OnExplode();
            }
        }

       

        public void Prepare()
        {
            bouncesLeft = bounces;
            body.velocity = Vector2.zero;
            body.angularVelocity = 0;
            lastHit = null;
        }

        public void CleanUp()
        {
            
        }

        public void SetCurrentGun(IGun gun)
        {
            currentGun = gun;
        }

        public void OnHit(IHitTarget hit, HitPoint hitPoint)
        {
            currentGun.RegisterHit(this, hit, hitPoint);
            hit.OnHit(this, hitPoint);
        }

        public void OnExplode()
        {
            Pool<ProjectileBehaviour>.Release(this);
        }
    }
}