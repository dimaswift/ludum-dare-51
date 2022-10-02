using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobFlyAI : MobBehaviour
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
            var target = Game.Instance.Player.transform.position;
            var pos = transform.position;
            if (target.y > pos.y)
            {
                flyingBehaviour.SpeedMultiplier = 2;
                flyingBehaviour.Move(Vector2.up);
                return;
            }
            flyingBehaviour.SpeedMultiplier = 1;
            flyingBehaviour.Move(target + targetOffset);
           
        }
    }
}