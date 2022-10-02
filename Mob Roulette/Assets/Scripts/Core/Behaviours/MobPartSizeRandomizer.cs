using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobPartSizeRandomizer : MobPartRandomizer
    {
        [SerializeField] private Transform mesh;
        [SerializeField] private Collider2D collider2d;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 1.5f;
        [SerializeField] private bool saveAspectRatio = true;
        
        public override void Randomize()
        {
            var x = Random.Range(minScale, maxScale);
            var y = saveAspectRatio ? x : Random.Range(minScale, maxScale);
            mesh.localScale = new Vector3(x, y, mesh.localScale.z);
            switch (collider2d)
            {
                case BoxCollider2D box:
                    box.size = mesh.localScale;
                    break;
                case CircleCollider2D circle:
                    circle.radius = mesh.localScale.x;
                    break;
            }
        }
    }
}