using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public abstract class MobBehaviour : MonoBehaviour
    {
        private Mob mob;

        private void Awake()
        {
            mob = GetComponentInParent<Mob>();
        }

        protected abstract void OnUpdate();
        
        private void Update()
        {
            if (mob.IsDead || Game.Instance.State.Value != GameState.Playing)
            {
                return;
            }
            OnUpdate();
        }
    }
}