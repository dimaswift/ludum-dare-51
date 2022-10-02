using System;
using System.Collections.Generic;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobRoulette.Core.Utils
{
    public static class Effects
    {
        private static Dictionary<EffectType, Effect> effects = new();

        private static Collider2D[] colliderBuffer = new Collider2D[32];
         
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            colliderBuffer = new Collider2D[32];
            effects = new();
            root = null;
        }

        private static Transform root;
        
        private static Effect Get(EffectType type)
        {
            if (effects.TryGetValue(type, out var effect))
            {
                return effect;
            }

            effect = Object.Instantiate(Resources.Load<GameObject>($"Effects/{type}")).GetComponent<Effect>();

            if (root == null)
            {
                root = new GameObject("Effects").transform;
            }
            
            effect.transform.SetParent(root.transform);
            
            if (effect == null)
            {
                throw new InvalidOperationException($"Effect not found. Should be here: Resources/Effects/{type}");
            }
            
            effects.Add(type, effect);

            return effect;
        }

        public static void Emit(EffectType type, int amount, Vector2 point, Vector2 normal = default)
        {
            var effect = Get(type);
            var transform = effect.transform;
          
            effect.Emit(point, normal, amount);
        }
        
        public static void Explode(Vector2 point, float radius, int damage)
        {
            int collidersCount = Physics2D.OverlapCircleNonAlloc(point, radius, colliderBuffer);
            for (int i = 0; i < collidersCount; i++)
            {
                var collider = colliderBuffer[i];
                var hitTarget = collider.GetComponent<IHitTarget>();
                if (hitTarget == null)
                {
                    continue;
                }
                hitTarget.OnExplode(radius, damage, point);
            }
        }

        public static void Play(EffectType type, Vector2 point, Vector2 normal = default)
        {
            var effect = Get(type);
            effect.Play(point, normal);
        }
    }
}