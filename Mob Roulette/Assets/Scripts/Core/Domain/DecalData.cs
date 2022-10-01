using UnityEngine;

namespace MobRoulette.Core.Domain
{
    public struct DecalData
    {
        public Vector3 Point { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public float ExtraIntensity { get; set; }
        public float Duration { get; set; }
    }
}