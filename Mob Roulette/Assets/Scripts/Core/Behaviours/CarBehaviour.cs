using System;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class CarBehaviour : MonoBehaviour
    {
        [SerializeField] private HingeJoint2D[] wheels;
        [SerializeField] private float maxSpeed = 100;
        
        public void Move(float dir, float speed)
        {
            foreach (HingeJoint2D wheel in wheels)
            {
                if (wheel == null)
                {
                    continue;
                }
                var motor = wheel.motor;
                motor.motorSpeed = dir * maxSpeed;
                motor.maxMotorTorque = speed;
                wheel.useMotor = speed > 0;
                wheel.motor = motor;
            }
        }
    }
}