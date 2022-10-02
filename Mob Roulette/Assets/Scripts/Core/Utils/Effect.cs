using MobRoulette.Core.Domain;
using UnityEngine;

namespace MobRoulette.Core.Utils
{
    public class Effect : MonoBehaviour
    {
        [SerializeField] private SoundType soundType;
        [Range(0f,1f)]
        [SerializeField] private float volume = 1f;

        [SerializeField] private float positionDepth = -10;
        
        private ParticleSystem system;
        private Transform effectTransform;
        
        private void Awake()
        {
            effectTransform = transform;
            system = GetComponent<ParticleSystem>();
        }

        public void Play(Vector3 pos, Vector2 normal)
        {
            pos.z = positionDepth;
            effectTransform.position = pos;
            effectTransform.up = normal;
            system.Play();
            Sounds.PlaySound(soundType, effectTransform.position, volume);
        }

        public void Emit(Vector3 pos, Vector2 normal, int amount)
        {
            pos.z = positionDepth;
            effectTransform.position = pos;
            effectTransform.up = normal;
            system.Emit(amount);
            Sounds.PlaySound(soundType, effectTransform.position, volume);
        }
    }
}