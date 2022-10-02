using UnityEngine;

namespace MobRoulette.Core.Utils
{
    public static class Utils
    {
        public static float NormalizeAngle(float a)
        {
            return a - 180f * Mathf.Floor((a + 180f) / 180f);
        }
    }
}