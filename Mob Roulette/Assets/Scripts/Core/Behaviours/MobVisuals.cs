using System;
using System.Collections;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MobVisuals : MonoBehaviour
    {
        private MeshRenderer meshRenderer;

        private Color color;
        private Color blinkColor;

        private float blinkTimer;
        
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            color = meshRenderer.material.color;
        }
        
        public void Blink(Color color)
        {
            blinkTimer = 1;
            blinkColor = color;
        }

        private void Update()
        {
            if (blinkTimer > 0)
            {
                blinkTimer -= Time.deltaTime * 10;
                meshRenderer.material.color = Color.Lerp(color, blinkColor, Mathf.Sin(Mathf.PI * blinkTimer));
            }
        }
    }
}