using MobRoulette.Core.Domain;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class FadingDecal : Decal
    {
        [ColorUsage(true, true)]
        [SerializeField] private Color color;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
        private Color defaultColor;
        private float defaultIntensity;
        private int intensityId;
        private int glowColorId;
        private float secondsTillFade;
        private float fadeDuration;
        private Material material;

        public override void Init()
        {
            base.Init();
            material = GetComponent<MeshRenderer>().material;
            intensityId = Shader.PropertyToID("_Intensity");
            glowColorId = Shader.PropertyToID("_Glow");
            defaultColor = material.GetColor(glowColorId);
            defaultIntensity = material.GetFloat(intensityId);
        }

        public override void Place(Transform target, Vector3 point, DecalData data)
        {
            base.Place(target, point, data);

            if (data.Duration > 0)
            {
                secondsTillFade = data.Duration;
            }

            fadeDuration = data.Duration;

            if (color.maxColorComponent > 0)
            {
                material.SetColor(glowColorId, color);
            }

            material.SetFloat(intensityId, defaultIntensity);
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

        public override void Reuse()
        {
            base.Reuse();
            fadeDuration = 0;
            secondsTillFade = 0;
            material.SetColor(glowColorId, defaultColor);
            material.SetFloat(intensityId, defaultIntensity);
        }
    }
}