using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Events
{
    [CreateAssetMenu(menuName = "MobRoulette/Events/OnGunShot", fileName = nameof(OnGunShot))]
    public class OnGunShot : BaseEventWithArgs<(IProjectile projectile, IGun gun)>
    {
        
    }
}