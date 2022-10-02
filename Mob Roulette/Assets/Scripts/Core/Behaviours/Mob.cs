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
        public Vector2 Position => mainPart.Position;
        
        public event Action OnDestroyed = () => { };
        public bool IsDead { get; private set; }
        private readonly List<MobPart> parts = new();

        private MobPart mainPart;

        private void Start()
        {
            GetComponentsInChildren(parts);
            foreach (MobPart part in parts)
            {
                part.OnDeactivate += OnPartDeactivate;
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

        private void OnPartDeactivate(MobPart part)
        {
            if (IsDead)
            {
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
                if (other == part || other.Exploded)
                {
                    continue;
                }
                other.OnDeactivate -= OnPartDeactivate;
                var dir = other.transform.position - part.transform.position;
                other.AttachPermanentDecal(DecalType.SmallFire, 1);
                other.Body.velocity = new Vector2(Mathf.Sign(dir.x) * Random.Range(5f, 10f), Random.Range(10f, 20f));
                other.Body.AddTorque(Random.Range(-1000f, 1000f), ForceMode2D.Impulse);
                parts.RemoveAt(i);
                other.Deactivate(Random.Range(3f, 6f));
            }
            Destroy(gameObject);
        }
    }
}