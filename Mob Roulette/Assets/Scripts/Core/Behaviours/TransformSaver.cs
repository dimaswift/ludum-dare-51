using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class TransformSaver : MonoBehaviour, IDefaultStateSaver
    {
        private Transform defaultParent;

        private Vector3 pos;
        private Quaternion rot;
        
        public void Save()
        {
            var t = transform;
            defaultParent = t.parent;
            pos = t.localPosition;
            rot = t.localRotation;
        }

        public void Restore()
        {
            var t = transform;
            t.SetParent(defaultParent);
            t.localPosition = pos;
            t.localRotation = rot;
        }

        public void OnParentDestroyed()
        {
           
        }
    }
}