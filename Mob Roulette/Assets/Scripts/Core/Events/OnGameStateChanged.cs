using Core;
using UnityEngine;

namespace MobRoulette.Core.Events
{
    [CreateAssetMenu(menuName = "MobRoulette/Events/OnGameStateChanged", fileName = nameof(OnGameStateChanged))]
    public class OnGameStateChanged : BaseEventWithArgs<GameState>
    {

    }
}