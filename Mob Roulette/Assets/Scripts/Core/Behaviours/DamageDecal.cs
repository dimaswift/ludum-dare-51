using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class DamageDecal : MonoBehaviour
    {
        private int intensityId;
        private int sizeId;
        private Material material;

        private bool initialized;
        
        private void Awake()
        {
            Init();
        }
        
        private void Init()
        {
            if (initialized) return;
            
            initialized = true;
            material = GetComponent<MeshRenderer>().material;
            intensityId = Shader.PropertyToID("_Intensity");
            sizeId = Shader.PropertyToID("_Size");
        }

        public void SetSize(Vector2 size)
        {
            Init();
            material.SetVector(sizeId, size);
        }

        public void SetDamaged(float amount)
        {
            Init();
            material.SetFloat(intensityId, amount);
        }
    }
}