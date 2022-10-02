using System.Collections.Generic;
using Core;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class MobSpawner : MonoBehaviour
    {
        public IEnumerable<Mob> SpawnedMobs => spawnedMobs;

        [SerializeField] private Mob[] prefabs;
        [SerializeField] private float spawnRate = 10;

        private float lastSpawn;
        private float currentSpawnRate;

        private int wave;

        private int lastSpawnPointIndex;
        private Transform[] spawnPoints;
        
        private readonly List<Mob> spawnedMobs = new ();

        public Mob FindClosestAliveMob(Vector2 point)
        {
            var closest = float.MaxValue;
            Mob closestMob = null;
            foreach (Mob mob in spawnedMobs)
            {
                if (mob.IsDead)
                {
                    continue;
                }
                var dist = Vector2.SqrMagnitude(mob.Position - point);
                if (dist < closest)
                {
                    closest = dist;
                    closestMob = mob;
                }
            }
            return closestMob;
        }

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

                if (spawnedMobs.Count > 3)
                {
                    return;
                }
                
                lastSpawn = Time.time;
                for (int i = 0; i < wave; i++)
                {
                   
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