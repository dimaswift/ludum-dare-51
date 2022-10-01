using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private GunBehaviour gunBehaviour;

        private void Start()
        {
            EventBus.Subscribe<OnGunShot, (IProjectile projectile, IGun gun)>(OnGunShot);
        }

        private void OnGunShot((IProjectile projectile, IGun gun) arg0)
        {
            Debug.Log(arg0.projectile.PrefabId);
            EventBus.Clear<OnGunShot>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                gunBehaviour.TryShoot(out var projectile);
            }
        }
    }
}