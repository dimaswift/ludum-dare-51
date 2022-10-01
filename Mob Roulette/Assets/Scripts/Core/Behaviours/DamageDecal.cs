using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class DamageDecal : MonoBehaviour
    {
        private int intensityId;
        private int sizeId;
        private Material material;
        
        private void Awake()
        {
            material = GetComponent<MeshRenderer>().material;
            intensityId = Shader.PropertyToID("_Intensity");
            sizeId = Shader.PropertyToID("_Size");
        }

        public void SetSize(Vector2 size)
        {
            material.SetVector(sizeId, size);
        }

        public void SetDamaged(float amount)
        {
            material.SetFloat(intensityId, amount);
        }
    }
}