using System;
using System.Collections.Generic;
using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobRoulette.Core.Utils
{
    public static class Pool<T> where T : MonoBehaviour, IPooled
    {
        public static void Dispose()
        {
            try
            {
                foreach (var pool in pools)
                {
                    pool.Value.DisposeAll();
                }
            }
            catch (Exception e)
            {
                
            }
           
            pools = new();
        }
        
        private static Dictionary<int, CustomPool<T>> pools = new();
        
        public static T GetFromPool(T prefab)
        {
            if (pools.TryGetValue(prefab.GetInstanceID(), out var pool) == false)
            {
                pool = new CustomPool<T>(() =>
                    {
                        var instance = Object.Instantiate(prefab.gameObject).GetComponent<T>();
                        instance.PrefabId = prefab.GetInstanceID();
                        instance.Init();
                        instance.IsInUse = false;
                        instance.gameObject.SetActive(false);
                        return instance;
                    },
                    OnGet,
                    OnCleanUp,
                    OnDestroy);
                pools.Add(prefab.GetInstanceID(), pool);
            }
            return pool.Get();
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
               pool.ReleaseAll();
            }
        }

        public static void ReleaseAll()
        {
            foreach (var data in pools)
            {
                data.Value.ReleaseAll();
            }
        }
        
        public static void Release(T instance)
        {
            if (pools.TryGetValue(instance.PrefabId, out var pool))
            {
                pool.Release(instance);
            }
        }

        private static void OnGet(T obj)
        {
            obj.gameObject.SetActive(true);
            obj.Reuse();
            obj.IsInUse = true;
        }

        private static void OnCleanUp(T obj)
        {
            obj.transform.SetParent(null);
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
            ReleaseAll();
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