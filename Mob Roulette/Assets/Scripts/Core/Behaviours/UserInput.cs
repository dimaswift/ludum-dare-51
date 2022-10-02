using System;
using Core;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

namespace MobRoulette.Core.Behaviours
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private FlyingBehaviour flyingBehaviour;
        [SerializeField] private Axis moveAxis = Axis.X;
        private Camera cam;
        
        private void Awake()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            Vector2 dir = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Game.Instance.GoToMenu();
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
            
            IGun currentGun = Game.Instance.Player.CurrentGun;
            
            currentGun.Aim(cam.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                
                if (currentGun.TryShoot(out var projectile))
                {
                    if (projectile.CurrentGun.Config.ProjectileAutoAimSpeed > 0)
                    {
                        var closest = Game.Instance.WaveManager.FindClosestAliveMob(transform.position);
                        if (closest != null)
                        {
                            projectile.SetFollowTarget(closest.MainPart);
                        }
                    }
                }
            }
        }
    }
}