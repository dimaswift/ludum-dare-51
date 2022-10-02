using Core;
using MobRoulette.Core.Domain;
using UnityEngine;


namespace MobRoulette.Core.Behaviours
{
    public class MobFlyAI : MobBehaviour
    {
        [SerializeField] private Turbine leftTurbine;
        [SerializeField] private Turbine rightTurbine;

        private int groundLayer;

        private void Start()
        {
            leftTurbine.Part.OnDeactivate += p => { mobPart.AttachPermanentDecal(DecalType.SmallFire, 3); };
            rightTurbine.Part.OnDeactivate += p => { mobPart.AttachPermanentDecal(DecalType.SmallFire, 3); };
        }

        protected override void OnUpdate()
        {
            var target = Game.Instance.Player.transform.position;
            var pos = transform.position;

            var dir = (target - pos);

            if (dir.x > 5)
            {
                leftTurbine?.SetRotation(-15);
                rightTurbine?.SetRotation(-15);
            }

            else if (dir.x < -5)
            {
                leftTurbine?.SetRotation(15);
                rightTurbine?.SetRotation(15);
            }
            else
            {
                leftTurbine?.SetRotation(0);
                rightTurbine?.SetRotation(0);
            }

            var force = 0f;

            if (dir.y > 0)
            {
                force = dir.y;
            }
            else if (dir.y < -50)
            {
                force = -5;
            }
            else
            {
                force = 0;
            }

            if (mobPart.IsOnFire)
            {
                leftTurbine.SetSpeed(100000, false);
                rightTurbine.SetSpeed(100000, false);
                return;
            }

            leftTurbine?.SetSpeed(force, true);
            rightTurbine?.SetSpeed(force, true);

        }
    }

}