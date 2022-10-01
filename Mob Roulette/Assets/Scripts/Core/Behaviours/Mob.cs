using System.Collections.Generic;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class Mob : MonoBehaviour, IMob
    {
        private readonly List<MobPart> parts = new();

        public void Reuse()
        {
            foreach (MobPart mobPart in parts)
            {
                Pool<MobPart>.Release(mobPart);
            }
            parts.Clear();
        }

        public int PrefabId { get; set; }
        public bool IsInUse { get; set; }

        public void OnCleanUp()
        {
            
        }

        public void OnDestroy()
        {
            
        }


        public void Init()
        {
           
            
        }

        public void Build(List<MobPart> partPrefabs)
        {
            foreach (MobPart part in partPrefabs)
            {
                var partInstance = Pool<MobPart>.GetFromPool(part);
                partInstance.transform.SetParent(transform);
                parts.Add(partInstance);
            }
        }
    }
}