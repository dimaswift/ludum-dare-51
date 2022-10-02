using System;
using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using UnityEngine;
using UnityEngine.Animations;

namespace MobRoulette.Core.Behaviours
{
    public class FlyingBehaviour : MonoBehaviour
    {
        public float SpeedMultiplier { get; set; } = 1f;
        
        [SerializeField] private float moveSpeed;

        [Range(0.01f, 1f)]
        [SerializeField] private float drag;
        
        private Rigidbody2D body;
        private Vector2 moveDirection;
        
        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }
        
        public void Move(Vector2 direction)
        {
            moveDirection = direction.normalized;
        }
        
        private void FixedUpdate()
        {
            body.velocity = Vector2.Lerp(body.velocity, moveDirection * (moveSpeed * SpeedMultiplier), Time.deltaTime / drag);
        }
    }
}