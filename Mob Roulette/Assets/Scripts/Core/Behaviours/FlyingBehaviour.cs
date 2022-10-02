using System;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class FlyingBehaviour : MonoBehaviour
    {
        public bool IsFalling => isFalling;
        public float SpeedMultiplier { get; set; } = 1f;
        
        [SerializeField] private float moveSpeed;

        [Range(0.01f, 1f)]
        [SerializeField] private float drag;
        
        private Rigidbody2D body;
        private Vector2 moveDirection;
        private bool isFalling;
     
        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void StartFalling()
        {
            if (isFalling)
            {
                return;
            }
            isFalling = true;
            body.angularDrag = 0;
            body.drag = 0;
            body.gravityScale = 1;
        }
        
        public void Move(Vector2 direction)
        {
            moveDirection = direction.normalized;
        }

        private void FixedUpdate()
        {
            if (isFalling)
            {
                return;
            }
            body.velocity = Vector2.Lerp(body.velocity, moveDirection * (moveSpeed * SpeedMultiplier), Time.deltaTime / drag);
        }
    }
}