using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobRoulette.Core.Utils
{
    public sealed class TimerUtils : MonoBehaviour
    {
        private static TimerUtils instance;
        private static TimerUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("Timer").AddComponent<TimerUtils>();
                }
                return instance;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            instance = null;
        }
        
        public static void Delay(float delay, Action callback)
        {
            Instance.StartCoroutine(Instance.DelayCoroutine(delay, callback));
        }

        public static void DelayRealtime(float delay, Action callback)
        {
            Instance.StartCoroutine(Instance.DelayRealtimeCoroutine(delay, callback));
        }

        private IEnumerator DelayCoroutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        private IEnumerator DelayRealtimeCoroutine(float delay, Action callback)
        {
            yield return new WaitForSecondsRealtime(delay);
            callback();
        }
    }
}