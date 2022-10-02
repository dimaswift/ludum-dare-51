using Core;
using MobRoulette.Core.Interfaces;

namespace MobRoulette.Core.Behaviours
{
    public class MobGun : MobBehaviour
    {
        private IGun gun;
        
        private void Start()
        {
            gun = GetComponent<IGun>();
        }

        protected override void OnUpdate()
        {
            if (gun == null)
            {
                return;
            }
            gun.TryShoot();
            gun.Aim(Game.Instance.Player.transform.position);
        }
    }
}