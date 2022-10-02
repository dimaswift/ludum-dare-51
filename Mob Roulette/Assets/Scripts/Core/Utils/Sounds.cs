using System;
using System.Collections.Generic;
using MobRoulette.Core.Domain;
using UnityEngine;
using UnityEngine.Pool;

namespace MobRoulette.Core.Utils
{
    public static class Sounds
    {
        private static Dictionary<SoundType, AudioClip> sounds = new ();
        private static ObjectPool<AudioSource> audioSources;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            sounds = new();
            audioSources = new ObjectPool<AudioSource>(() =>
            {
                var audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                return audioSource;
            });
        }
        
        public static void PlaySound(SoundType soundType, Vector2 position, float volume = 1f)
        {
            if (soundType == SoundType.None)
            {
                return;
            }
            
            if (!sounds.TryGetValue(soundType, out var clip))
            {
               clip = Resources.Load<AudioClip>($"Sounds/{soundType}");
               if (clip == null)
               {
                   throw new Exception($"Sound not found in Resources/Sounds/{soundType}");
               }
               sounds.Add(soundType, clip);
            }
            var audioSource = audioSources.Get();
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            TimerUtils.Delay(clip.length, () => audioSources.Release(audioSource));
        }

    }

}