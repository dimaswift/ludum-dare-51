using System;
using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobFlyAI : MobBehaviour
    {
        [SerializeField] private FlyingBehaviour flyingBehaviour;
        [SerializeField] private float minDistance = 5;
        [SerializeField] private float maxDistance = 5;

        protected override void OnUpdate()
        {
            var target = Game.Instance.Player.transform.position;
            var direction = target - transform.position;
            var dist = direction.magnitude;
            if (dist > minDistance)
            {
                flyingBehaviour.SpeedMultiplier = 1;
                flyingBehaviour.Move(direction.normalized);
            }
            else if (dist < maxDistance)
            {
                flyingBehaviour.SpeedMultiplier = 0.5f;
                flyingBehaviour.Move(-direction.normalized);
            }
            else
            {
                flyingBehaviour.SpeedMultiplier = 0.5f;
                flyingBehaviour.Move(Vector2.up);
            }
        }
    }
}