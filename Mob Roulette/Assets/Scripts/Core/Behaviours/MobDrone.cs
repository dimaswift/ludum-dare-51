using System;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobDrone : MobBehaviour
    {
        [SerializeField] private FlyingBehaviour flyingBehaviour;
        [SerializeField] private float minDistance = 5;
        [SerializeField] private float maxDistance = 5;

        private Vector3 targetOffset;

        private void Start()
        {
            targetOffset = new Vector2(Random.Range(minDistance, maxDistance), Random.Range(minDistance, maxDistance));
        }

        protected override void OnUpdate()
        {
            if (mobPart.Deactivated)
            {
                mobPart.Body.drag = 0;
                mobPart.Body.gravityScale = 1;
                return;
            }
            
            var target = Game.Instance.Player.transform.position;
            var pos = transform.position;
            if (target.y > pos.y)
            {
                flyingBehaviour.SpeedMultiplier = 2;
                flyingBehaviour.Move(Vector2.up);
                return;
            }

            var dir = (target + targetOffset) - pos;
            flyingBehaviour.SpeedMultiplier = 1;
            flyingBehaviour.Move(dir.normalized);

        }
    }
}