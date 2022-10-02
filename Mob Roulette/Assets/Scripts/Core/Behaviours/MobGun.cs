using Core;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobGun : MobBehaviour
    {
        [SerializeField] private bool followPlayer;
        private IGun gun;
        
        private void Start()
        {
            gun = GetComponent<IGun>();
            delay = Random.Range(0f, 1f);
        }

        private float delay;

        protected override void OnUpdate()
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }
            
            if (gun == null)
            {
                return;
            }

            if (gun.TryShoot(out var projectile))
            {
                if (followPlayer)
                {
                    projectile.SetFollowTarget(Game.Instance.Player);
                }
            }
            gun.Aim(Game.Instance.Player.transform.position);
        }
    }
}