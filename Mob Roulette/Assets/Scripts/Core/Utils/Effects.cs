using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobRoulette.Core.Utils
{
    public static class Effects
    {
        private static Dictionary<EffectType, ParticleSystem> effects = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            effects = new();
            root = null;
        }

        private static Transform root;
        
        private static ParticleSystem Get(EffectType type)
        {
            if (effects.TryGetValue(type, out var effect))
            {
                return effect;
            }

            effect = Object.Instantiate(Resources.Load<GameObject>($"Effects/{type}")).GetComponent<ParticleSystem>();

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
            transform.position = new Vector3(point.x, point.y, -9);
            transform.up = normal;
            effect.Emit(amount);
        }

        public static void Play(EffectType type, Vector2 point, Vector2 normal = default)
        {
            var effect = Get(type);
            var transform = effect.transform;
    
            transform.position = new Vector3(point.x, point.y, -9);
            transform.up = normal;
            effect.Play();
        }
    }
}