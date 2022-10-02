using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class Turbine : MonoBehaviour
    {
        public MobPart Part => mobPart;
        
        public bool Active => mobPart.Deactivated == false;
        
        private MobPart mobPart;
        private float targetDirection;
        private float speed;
        private bool instant;
        private void Awake()
        {
            mobPart = GetComponent<MobPart>();
        }

        public void SetRotation(float rotation)
        {
            targetDirection = rotation;
        }

        public void SetSpeed(float speed, bool instant)
        {
            this.instant = instant;
            this.speed = speed;
        }

        private void Update()
        {
            if (mobPart.Deactivated)
            {
                
                return;
            }
            
            if (instant)
            {
                mobPart.Body.MoveRotation(Mathf.LerpAngle(mobPart.Body.rotation, targetDirection, Time.deltaTime * 5));
                mobPart.Body.velocity = transform.up * speed;
            }
            else
            {
                mobPart.Body.AddForce(transform.up * speed * Time.deltaTime);
            }
           
        }
    }
}