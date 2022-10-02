using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobCarAI : MobBehaviour
    {
        [SerializeField] private CarBehaviour car;
        protected override void OnUpdate()
        {
            var target = Game.Instance.Player.transform.position;
            var direction = target - transform.position;
            car.Move(direction.normalized.x, 10000);
        }
    }
}