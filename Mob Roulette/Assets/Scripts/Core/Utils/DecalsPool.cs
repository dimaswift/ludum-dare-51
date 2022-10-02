using System;
using System.Collections.Generic;
using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Domain;
using UnityEngine;

namespace MobRoulette.Core.Utils
{
    public static class DecalsPool
    {
        private static Dictionary<DecalType, Decal> prefabs = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            prefabs = new();
        }

        private static Decal GetFromPool(DecalType type)
        {
            if (!prefabs.TryGetValue(type, out var prefab))
            {
                prefab = Resources.Load<GameObject>($"Decals/{type}").GetComponent<Decal>();
                prefabs.Add(type, prefab);
            }

            if (prefab == null)
            {
                throw new Exception($"Decal prefab not found in Resources/Decals/{type}");
            }
            return Pool<Decal>.GetFromPool(prefab);
        }
        
        public static Decal AddDecal(DecalType type, Vector3 point, Transform target, DecalData data)
        {
            var decal = GetFromPool(type);
            decal.Place(target, point, data);
            return decal;
        }

    }
}