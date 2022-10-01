using UnityEngine;

namespace MobRoulette.Core.Domain
{
    public struct HitPoint
    {
        public Vector2 Point { get; set; }
        public Vector2 Normal { get; set; }
        public float Impulse { get; set; }
    }
}