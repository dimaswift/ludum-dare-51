﻿using System;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MobVisuals : MonoBehaviour
    {
        [SerializeField] private ParticleSystem destroyEffect;
        [SerializeField] private DamageDecal damageDecal;

        private void Start()
        {
            damageDecal.SetSize(transform.parent.localScale);
        }

        public void SetDamaged(float damagePercent)
        {
           
            damageDecal.SetDamaged(damagePercent);
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