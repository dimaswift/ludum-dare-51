using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class Wall : MonoBehaviour, IHitTarget
    {
        [SerializeField] private Transform decalPoint;
        
        public void OnHit(IProjectile projectile, HitPoint hitPoint)
        {
            Vector3 decalPos = transform.InverseTransformPoint(hitPoint.Point);
            decalPos.z = decalPoint.localPosition.z;
            DecalsPool.AddDecal(projectile.CurrentGun.Config.DecalType, transform.TransformPoint(decalPos), transform,
                new DecalData()
                {
                    Duration = Random.Range(1f, 3f) + hitPoint.ExtraDecalDuration,
                    Size = Random.Range(2f, 4f) + hitPoint.ExtraDecalSize
                });
        }

        public void OnExplode(float maxRadius, int damage, Vector2 center)
        {
            
        }
    }
}
