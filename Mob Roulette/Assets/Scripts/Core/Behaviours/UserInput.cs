using System;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using UnityEngine.Animations;

namespace MobRoulette.Core.Behaviours
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private FlyingBehaviour flyingBehaviour;
        [SerializeField] private Axis moveAxis = Axis.X;
        private Camera cam;
        private int gunIndex;
        private IGun[] guns;

        private readonly KeyCode[] gunKeys =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
        };
        
        private void Awake()
        {
            cam = Camera.main;
            guns = GetComponentsInChildren<IGun>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PoolDisposal.ReleaseAll();
            }
            
            Vector2 dir = Vector2.zero;

            for (int i = 0; i < guns.Length; i++)
            {
                if (Input.GetKeyDown(gunKeys[i]))
                {
                    if (gunIndex != i)
                    {
                        guns[gunIndex].SetEquipped(false);
                        gunIndex = i;
                        guns[gunIndex].SetEquipped(true);
                    }
                }
            }
            
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

            if(gunIndex < guns.Length)
            {
                var currentGun = guns[gunIndex];

                currentGun.Aim(cam.ScreenToWorldPoint(Input.mousePosition));

                if (Input.GetMouseButton(0))
                {
                    currentGun.TryShoot();
                }
            }
        }
    }
}