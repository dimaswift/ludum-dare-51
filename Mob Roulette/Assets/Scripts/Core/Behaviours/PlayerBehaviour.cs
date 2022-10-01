using System;
using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class PlayerBehaviour : MonoBehaviour
    {
        public GunBehaviour Gun => gunBehaviour;
        
        
        [SerializeField] private GunBehaviour gunBehaviour;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float width;
        [SerializeField] private float aimSpeed;
        [SerializeField] private Transform turret;
        
        [Range(0.01f, 1f)]
        [SerializeField] private float drag;
        
        private Rigidbody2D body;
        private float moveDirection;
        private Vector2 cameraBounds;
        private Camera cam;
        private Vector2 targetPoint;
        private float currentAimAngle;
        
        private void Start()
        {
            cam = Camera.main;
            body = GetComponent<Rigidbody2D>();
            EventBus.Subscribe<OnGunShot, (IProjectile projectile, IGun gun)>(OnGunShot);
        }

        private void OnGunShot((IProjectile projectile, IGun gun) arg0)
        {
            Debug.Log(arg0.projectile.PrefabId);
            EventBus.Clear<OnGunShot>();
        }
        
        public void MoveTowards(float direction)
        {
            moveDirection = Mathf.Lerp(moveDirection, direction, Time.deltaTime / drag);
        }

        public void Stop()
        {
            moveDirection = Mathf.Lerp(moveDirection, 0, Time.deltaTime / drag);
        }

        public void Aim(Vector2 target)
        {
            targetPoint = target;
        }
        
        private void FixedUpdate()
        {
            cameraBounds = cam.transform.position + new Vector3(cam.orthographicSize * cam.aspect * 2,
                cam.orthographicSize * 2, 0);
            var pos = body.position;
            pos.x += moveDirection * moveSpeed * Time.fixedDeltaTime;
            pos.x = Mathf.Clamp(pos.x, (-cameraBounds.x / 2) + width / 2, (cameraBounds.x / 2) - width / 2);
            body.MovePosition(pos);
            var turretPos = turret.position;
            var angle = Mathf.Atan2(Mathf.Max(0, targetPoint.y - turretPos.y), targetPoint.x - turretPos.x) * Mathf.Rad2Deg - 90;
            angle = Mathf.Clamp(angle, -90, 90);
            currentAimAngle = Mathf.LerpAngle(currentAimAngle, angle, Time.fixedDeltaTime * aimSpeed);
            turret.eulerAngles = new Vector3(0, 0, currentAimAngle);
        }
    }
}