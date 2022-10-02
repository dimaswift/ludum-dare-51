using System.Collections.Generic;
using Core;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobSpawner : MonoBehaviour
    {
        [SerializeField] private Mob[] prefabs;
        [SerializeField] private float spawnRate = 10;

        private float lastSpawn;
        private float currentSpawnRate;

        private int wave;

        private int lastSpawnPointIndex;
        private Transform[] spawnPoints;
        
        private readonly List<Mob> spawnedMobs = new ();

        private void Awake()
        {
            spawnPoints = GetComponentsInChildren<Transform>();
            currentSpawnRate = spawnRate;
            Game.Instance.State.Subscribe(s =>
            {
                if (s == GameState.Playing)
                {
                    foreach (Mob mob in spawnedMobs)
                    {
                        if (mob != null)
                        {
                            Destroy(mob.gameObject);
                        }
                    }
                    spawnedMobs.Clear();
                    lastSpawn = -currentSpawnRate;
                    currentSpawnRate = spawnRate;
                    wave = 1;
                }
            });
        }

        private void Update()
        {
            if (Game.Instance.State.Value != GameState.Playing) return;

            if (spawnPoints.Length == 0)
            {
                return;
            }
            
            if (Time.time - lastSpawn >= currentSpawnRate)
            {
                for (int i = 0; i < wave; i++)
                {
                    lastSpawn = Time.time;
                    var pointIndex = Random.Range(0, spawnPoints.Length);
                    while (pointIndex == lastSpawnPointIndex)
                    {
                        pointIndex = Random.Range(0, spawnPoints.Length);
                    }
                    lastSpawnPointIndex = pointIndex;
                    var point = spawnPoints[pointIndex];
                    var mob = Instantiate(prefabs[Random.Range(0, prefabs.Length)].gameObject).GetComponent<Mob>();
                    mob.transform.position = point.position;
                    mob.transform.rotation = Quaternion.identity;
                    mob.OnDestroyed += () => spawnedMobs.Remove(mob);
                    spawnedMobs.Add(mob);
                }

                wave = Mathf.Min(3, wave + 1);
            }
        }
    }
}