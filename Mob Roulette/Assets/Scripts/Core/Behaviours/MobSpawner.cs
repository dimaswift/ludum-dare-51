using System;
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

        [SerializeField] private bool randomizeOrder;
        [SerializeField] private Mob[] prefabs;
        [SerializeField] private float spawnRate = 10;

        private float lastSpawn;
        private float currentSpawnRate;

        private int wave;

        private int lastSpawnPointIndex;
        private Transform[] spawnPoints;
        private int lastMobIndex;
        
        private readonly HashSet<Mob> spawnedMobs = new ();

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
            Game.Instance.State.Subscribe(s =>
            {
                if (s == GameState.Playing || s == GameState.MainMenu)
                {
                    CleanUp();
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
               
                lastSpawn = Time.time;
                for (int i = 0; i < wave; i++)
                {
                    if (spawnedMobs.Count >= 3)
                    {
                        continue;
                    }

                    var point = spawnPoints[lastSpawnPointIndex];
                    
                    var index = randomizeOrder ? Random.Range(0, prefabs.Length) : lastMobIndex;
                    lastMobIndex++;
                    if(lastMobIndex >= prefabs.Length)
                    {
                        lastMobIndex = 0;
                    }
                    var mob = Instantiate(prefabs[index].gameObject).GetComponent<Mob>();
                    mob.transform.position = point.position;
                    mob.transform.rotation = Quaternion.identity;
                    mob.OnDestroyed += () =>
                    {
                        spawnedMobs.Remove(mob);
                    };
                    spawnedMobs.Add(mob);

                    lastSpawnPointIndex++;
                  
                    if(lastSpawnPointIndex >= spawnPoints.Length)
                    {
                        lastSpawnPointIndex = 0;
                    }
                    
                }

                wave = Mathf.Min(3, wave + 1);
            }
        }

        public void CleanUp()
        {
            wave = 1;
            lastSpawn = -spawnRate;
            currentSpawnRate = spawnRate;
            lastSpawnPointIndex = 0;
            foreach (Mob mob in spawnedMobs)
            {
                Destroy(mob.gameObject);
            }
            spawnedMobs.Clear();
        }
    }
}