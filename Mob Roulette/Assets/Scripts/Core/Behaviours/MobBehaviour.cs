using Core;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public abstract class MobBehaviour : MonoBehaviour
    {
        protected Mob mob;
        protected MobPart mobPart;
        

        private void Awake()
        {
            mobPart = GetComponent<MobPart>();
            if (mobPart == null)
            {
                mobPart = GetComponentInParent<MobPart>();
            }
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