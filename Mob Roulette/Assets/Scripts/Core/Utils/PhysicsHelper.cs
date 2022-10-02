using UnityEngine;

namespace MobRoulette.Core.Utils
{
    public static class PhysicsHelper
    {
        public static Vector2 GetThrowVelocity(Vector2 source, Vector2 target)
        {
            var dist = target - source;
            var height = Mathf.Max(0, dist.y);
            var half = new Vector2(dist.x, 0);
            float vY = Mathf.Sqrt(-2 * Physics2D.gravity.y * height);
            var vxZ = -(half * Physics.gravity.y) / vY;
            return new Vector2(vxZ.x, vY);
        }
    }
}