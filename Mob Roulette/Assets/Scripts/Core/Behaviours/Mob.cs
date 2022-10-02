using System;
using System.Collections.Generic;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class Mob : MonoBehaviour, IMob
    {
        public event Action OnDestroyed = () => { };
        public bool IsDead { get; private set; }
        private readonly List<MobPart> parts = new();

        private void Start()
        {
            GetComponentsInChildren(parts);
            foreach (MobPart part in parts)
            {
                part.OnBeforeKilled += OnBeforeKilled;
            }
        }

        private void OnDestroy()
        {
            OnDestroyed();
        }

        private void OnBeforeKilled(MobPart part)
        {
            if (IsDead)
            {
                return;
            }
            parts.Remove(part);
            if (parts.Count == 0)
            {
                IsDead = true;
                Destroy(gameObject);
                return;
            }
            if (!part.IsMain)
            {
                return;
            }
            IsDead = true;
            for (var i = parts.Count - 1; i >= 0; i--)
            {
                var other = parts[i];
                if (other == part)
                {
                    continue;
                }
                other.OnBeforeKilled -= OnBeforeKilled;
                other.Kill();
                other.Body.velocity = new Vector2(Random.Range(-5f, 5f), 20);
                TimerUtils.Delay(Random.Range(3f, 6f), () => other.Explode());
            }

            Destroy(gameObject);
        }
    }
}