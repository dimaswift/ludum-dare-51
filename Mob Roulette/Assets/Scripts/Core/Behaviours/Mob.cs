using System;
using System.Collections.Generic;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class Mob : MonoBehaviour, IMob
    {
        public MobPart MainPart => mainPart;
        public Vector2 Position => mainPart != null 
                ? mainPart.transform.position : parts.Count > 0 
                ? parts[0].transform.position : transform.position;
        
        public event Action OnDestroyed = () => { };
        public bool IsDead { get; private set; }
        private readonly List<MobPart> parts = new();

        private MobPart mainPart;

        private void Start()
        {
            GetComponentsInChildren(parts);
            foreach (MobPart part in parts)
            {
                part.OnBeforeKilled += OnPartKilled;
                if (part.IsMain)
                {
                    mainPart = part;
                }
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
                other.AttachPermanentDecal(DecalType.SmallFire, 1);
                other.Body.velocity = new Vector2(Mathf.Sign(dir.x) * Random.Range(5f, 10f), Random.Range(10f, 20f));
                other.Body.AddTorque(Random.Range(-1000f, 1000f), ForceMode2D.Impulse);
                TimerUtils.Delay(Random.Range(3f, 6f), () => other.Explode());
            }

            Destroy(gameObject);
        }
    }
}