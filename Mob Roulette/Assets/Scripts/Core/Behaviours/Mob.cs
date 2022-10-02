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
                part.OnBeforeKilled += OnPartKilled;
            }
        }

        private void OnDestroy()
        {
            OnDestroyed();
        }

        private void OnPartKilled(MobPart part)
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
                other.OnBeforeKilled -= OnPartKilled;
                other.Kill();
                var dir = other.transform.position - part.transform.position;
                other.SetOnFire(true);
                other.Body.velocity = new Vector2(Mathf.Sign(dir.x) * Random.Range(5f, 10f), Random.Range(10f, 20f));
                TimerUtils.Delay(Random.Range(3f, 6f), () => other.Explode());
            }

            Destroy(gameObject);
        }
    }
}