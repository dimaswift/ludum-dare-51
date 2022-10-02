using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class Decal : MonoBehaviour, IPooled
    {
        public int PrefabId { get; set; }
        public bool IsInUse { get; set; }
        protected DecalData data;
        private ParticleSystem effect;
        private Vector3 effectPos;
        private float width;
        
      

        private void Attach(Transform target, Vector3 point)
        {
            Transform t = transform;
            t.localScale = new Vector3(data.Size, data.Size, width);
            t.SetParent(target, true);
            t.position = point;
        }
        
        public virtual void Place(Transform target, Vector3 point, DecalData data)
        {
            this.data = data;
            Attach(target, point);
        }

        public void OnCleanUp()
        {
            if (effect != null)
            {
                effect.transform.SetParent(null, true);
                effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public void OnDestroy()
        {
            
        }

        public virtual void Init()
        {
            width = transform.localScale.z;
            effect = GetComponentInChildren<ParticleSystem>();
            if (effect != null)
            {
                effectPos = effect.transform.localPosition;
            }
        }

        public virtual void Reuse()
        {
            if (effect != null)
            {
                var effectTransform = effect.transform;
                effectTransform.SetParent(transform);
                effectTransform.localPosition = effectPos;
                effect.Play();
            }
        }
    }
}