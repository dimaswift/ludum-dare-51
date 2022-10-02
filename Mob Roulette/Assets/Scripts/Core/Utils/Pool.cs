using System.Collections.Generic;
using System.Linq;
using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Interfaces;
using UnityEngine;
using UnityEngine.Pool;

namespace MobRoulette.Core.Utils
{
    public static class Pool<T> where T : MonoBehaviour, IPooled
    {
        public static void Dispose()
        {
            pools = new();
        }
        
        private static Dictionary<int, (ObjectPool<T> pool, HashSet<T> items)> pools = new();
    
        
        public static T GetFromPool(T prefab)
        {
            if (pools.ContainsKey(prefab.GetInstanceID()) == false)
            {
                var set = new HashSet<T>();
                var pool = new ObjectPool<T>(() =>
                    {
                        var instance = Object.Instantiate(prefab.gameObject).GetComponent<T>();
                        instance.PrefabId = prefab.GetInstanceID();
                        instance.Init();
                        return instance;
                    }, i=>
                    {
                        set.Add(i);
                        OnCreate(i);
                    },
                    i =>
                    {
                        set.Remove(i);
                        OnCleanUp(i);
                    },
                    OnDestroy);
                pools.Add(prefab.GetInstanceID(), (pool, set));
            }
            return pools[prefab.GetInstanceID()].pool.Get();
        }

        private static void OnDestroy(T obj)
        {
            obj.OnDestroy();
            Object.Destroy(obj.gameObject);
        }

        public static void ReleaseAll(T prefab)
        {
            if (pools.TryGetValue(prefab.GetInstanceID(), out var pool))
            {
                foreach (T item in pool.items.ToArray())
                {
                    pool.pool.Release(item);
                }
            }
        }

        public static void ReleaseAll()
        {
            foreach (var data in pools)
            {
                foreach (T item in data.Value.items.ToArray())
                {
                    data.Value.pool.Release(item);
                }
            }
        }
        
        public static void Release(T instance)
        {
            if (pools.TryGetValue(instance.PrefabId, out var pool))
            {
                instance.transform.SetParent(null);
                pool.pool.Release(instance);
            }
        }

        private static void OnCreate(T obj)
        {
            obj.gameObject.SetActive(true);
            obj.Reuse();
            obj.IsInUse = true;
        }

        private static void OnCleanUp(T obj)
        {
            if (obj == null || obj.IsInUse == false)
            {
                return;
            }
            obj.IsInUse = false;
            obj.OnCleanUp();
            obj.gameObject.SetActive(false);
        }
    }

    public static class PoolDisposal
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void DisposeAll()
        {
            Pool<Decal>.Dispose();
            Pool<ProjectileBehaviour>.Dispose();
        }

        public static void ReleaseAll()
        {
            Pool<Decal>.ReleaseAll();
            Pool<ProjectileBehaviour>.ReleaseAll();
        }
    }
}