using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MobVisuals : MonoBehaviour, IReusable, IScalable
    {
        [SerializeField] private DamageDecal damageDecalPrefab;
        private MeshRenderer meshRenderer;

        private Color color;
        private Color blinkColor;

        private float blinkTimer;

        private DamageDecal damageDecal;
        private ParticleSystem destroyEffect;
        
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            color = meshRenderer.material.color;
            destroyEffect = transform.parent.GetComponentInChildren<ParticleSystem>();
            damageDecal = Instantiate(damageDecalPrefab.gameObject).GetComponent<DamageDecal>();
            var damageDecalTransform = damageDecal.transform;
            damageDecalTransform.SetParent(transform);
            damageDecalTransform.localPosition = new Vector3(0, 0, -0.5f);
            damageDecalTransform.localRotation = Quaternion.identity;
            damageDecalTransform.localScale = Vector3.one;
            ApplyNewScale();
        }
        
        public void Blink(Color color)
        {
            blinkTimer = 1;
            blinkColor = color;
        }

        public void SetDamaged(float damagePercent)
        {
            damageDecal.SetDamaged(damagePercent);
            
        }
        

        public void Explode()
        {
            gameObject.SetActive(false);
            destroyEffect.transform.SetParent(null);
            destroyEffect.Play();
        }
        
        private void Update()
        {
            if (blinkTimer > 0)
            {
                blinkTimer -= Time.deltaTime * 10;
                meshRenderer.material.color = Color.Lerp(color, blinkColor, Mathf.Sin(Mathf.PI * blinkTimer));
            }
        }

        public void Reuse()
        {
            destroyEffect.transform.SetParent(transform.parent);
            gameObject.SetActive(true);
            
        }

        public void ApplyNewScale()
        {
            var scale = transform.localScale;
            var shape = destroyEffect.shape;
            shape.scale = scale;
            damageDecal.SetSize(scale * 0.1f);
        }
    }
}