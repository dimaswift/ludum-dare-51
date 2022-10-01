using System;
using MobRoulette.Core.Utils;
using UnityEngine;
using UnityEngine.Animations;

namespace MobRoulette.Core.Behaviours
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private GunBehaviour gunBehaviour;
        [SerializeField] private FlyingBehaviour flyingBehaviour;
        [SerializeField] private Axis moveAxis = Axis.X;
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PoolDisposal.ReleaseAll();
            }
            
            Vector2 dir = Vector2.zero;
            if (moveAxis.HasFlag(Axis.X))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    dir.x = -1;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    dir.x = 1;
                }
            }

            if (moveAxis.HasFlag(Axis.Y))
            {

                if (Input.GetKey(KeyCode.W))
                {
                    dir.y = 1;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    dir.y = -1;
                }
            }
            
            flyingBehaviour.Move(dir);

            gunBehaviour.Aim(cam.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetMouseButton(0))
            {
                gunBehaviour.TryShoot();
            }
        }
    }
}