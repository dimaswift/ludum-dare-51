using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobCarAI : MobBehaviour
    {
        [SerializeField] private CarBehaviour car;
        [SerializeField] private float minDistance = 5;
        [SerializeField] private float maxDistance = 5;

        protected override void OnUpdate()
        {
            var target = Game.Instance.Player.transform.position;
            var direction = target - transform.position;
            var dist = direction.magnitude;
            if (dist > minDistance)
            {
                car.Move(direction.normalized.x, 10000);
            }
            else if (dist < maxDistance)
            {
                car.Move(-direction.normalized.x, 10000);
            }
            else
            {
                car.Move(0, 0);
            }
        }
    }
}