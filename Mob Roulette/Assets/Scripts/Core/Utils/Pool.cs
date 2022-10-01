using System.Collections.Generic;
using MobRoulette.Core.Interfaces;
using UnityEngine;
using UnityEngine.Pool;

namespace MobRoulette.Core.Utils
{
    public static class Pool<T> where T : MonoBehaviour, IPooled
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            foreach (var pool in pools)
            {
                pool.Value.Dispose();
            }
            pools = new();
        }
        
        private static Dictionary<int, ObjectPool<T>> pools = new();

        public static T GetFromPool(T prefab)
        {
            if (pools.TryGetValue(prefab.GetInstanceID(), out var pool) == false)
            {
                pool = new ObjectPool<T>(() =>
                    {
                        var instance = Object.Instantiate(prefab.gameObject).GetComponent<T>();
                        instance.PrefabId = prefab.GetInstanceID();
                        instance.Init();
                        return instance;
                    }, OnCreate,
                    OnCleanUp);
                pools.Add(prefab.GetInstanceID(), pool);
            }
            return pool.Get();
        }

        public static void ReleaseAll(T prefab)
        {
            if (pools.TryGetValue(prefab.GetInstanceID(), out var pool))
            {
                pool.Clear();
            }
        }

        public static void Release(T instance)
        {
            if (pools.TryGetValue(instance.PrefabId, out var pool))
            {
                instance.transform.SetParent(null);
                pool.Release(instance);
            }
        }

        private static void OnCreate(T obj)
        {
            obj.gameObject.SetActive(true);
            obj.Prepare();
        }

        private static void OnCleanUp(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.CleanUp();
        }
    }
}