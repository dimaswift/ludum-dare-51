using System;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private PlayerBehaviour playerBehaviour;

        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                playerBehaviour.MoveTowards(-1);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                playerBehaviour.MoveTowards(1);
            }
            else
            {
                playerBehaviour.Stop();
            }
            playerBehaviour.Aim(cam.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetMouseButton(0))
            {
                playerBehaviour.Gun.TryShoot(out var projectile);
            }
        }
    }
}