using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MobVisuals : MonoBehaviour
    {
        [SerializeField] private ParticleSystem destroyEffect;
        
        
        private DamageDecal damageDecal;

        private void Awake()
        {
            damageDecal = GetComponentInChildren<DamageDecal>();
            if (damageDecal != null)
            {
                damageDecal.SetSize(transform.localScale / 10);
            }
        }

        public void SetDamaged(float damagePercent)
        {
            if (damageDecal != null)
            {
                damageDecal.SetDamaged(damagePercent);
            }
        }
        
        public void Explode()
        {
            if (destroyEffect != null)
            {
                destroyEffect.transform.SetParent(null);
                destroyEffect.Play();
            }

            gameObject.SetActive(false);
        }
    }
}