﻿using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class BaseDestructible : MonoBehaviour, IHitTarget
    {
        public void OnHit(IProjectile projectile, HitPoint hitPoint)
        {
            Destroy(gameObject);
        }
    }
}