using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobGun : MobBehaviour
    {
        [SerializeField] private GunBehaviour gunBehaviour;
        protected override void OnUpdate()
        {
            gunBehaviour.TryShoot();
            gunBehaviour.Aim(Game.Instance.Player.transform.position);
        }
    }
}