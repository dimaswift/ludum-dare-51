using Core;

namespace MobRoulette.Core.Behaviours
{
    public class MobGun : GunBehaviour
    {
        protected override void Update()
        {
            if (Game.Instance.State.Value != GameState.Playing)
            {
                return;
            }
            base.Update();
            TryShoot();
            Aim(Game.Instance.Player.transform.position);
        }
    }
}