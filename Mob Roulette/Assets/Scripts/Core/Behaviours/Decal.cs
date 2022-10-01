using System;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Decal : MonoBehaviour, IPooled
    {
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
        
        
        public int PrefabId { get; set; }

        private MeshRenderer meshRenderer;
        private int intensityId;
        private int glowColorId;
        
        private float secondsTillFade;
        private float fadeDuration;
        private Material material;

        private Color defaultColor;
        private float defaultIntensity;

        private DecalData data;
        
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            intensityId = Shader.PropertyToID("_Intensity");
            glowColorId = Shader.PropertyToID("_Glow");
            material = meshRenderer.material;
            defaultColor = material.GetColor(glowColorId);
            defaultIntensity = material.GetFloat(intensityId);
        }
        
        public void Prepare()
        {
            fadeDuration = 0;
            secondsTillFade = 0;
            material.SetColor(glowColorId, defaultColor);
            material.SetFloat(intensityId, defaultIntensity);
        }

        private void Attach(Transform target, Vector3 point)
        {
            var localPoint = target.InverseTransformPoint(point);
            localPoint.z = -0.5f;
            Transform t = transform;
            t.localScale = Vector3.one * data.Size;
            t.SetParent(target, true);
            t.localPosition = localPoint;
        }
        

        public void Place(Transform target, Vector3 point, DecalData data)
        {
            this.data = data;
            
            if (data.Duration > 0)
            {
                secondsTillFade = data.Duration;
            }
            
            fadeDuration = data.Duration;

            if (data.Color.maxColorComponent > 0)
            {
                material.SetColor(glowColorId, data.Color);
            }

            Attach(target, point);
        }

        private void Update()
        {
            if (secondsTillFade > 0)
            {
                secondsTillFade = Mathf.Max(0, secondsTillFade - Time.deltaTime);
                var intensity = (defaultIntensity + data.ExtraIntensity) *
                                fadeCurve.Evaluate(Mathf.Clamp(secondsTillFade / fadeDuration, 0, 1));
                material.SetFloat(intensityId, Mathf.Clamp(intensity, 0, 5));
                if (secondsTillFade <= 0)
                {
                    Pool<Decal>.Release(this);
                }
            }
        }

        public void CleanUp()
        {
           
        }

        public void Init()
        {
            
        }
    }
}