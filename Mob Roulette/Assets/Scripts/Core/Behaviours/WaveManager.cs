using System.Collections;
using System.Collections.Generic;
using Core;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobRoulette.Core.Behaviours
{
    public class WaveManager : MonoBehaviour
    {
        public IObservableValue<int> CountDown => countdown;
        public IObservableValue<int> Wave => wave;
        public IObservableValue<int> MobsLeft => mobsLeft;
        public IObservableValue<int> WaveRecord => waveRecord;
        
        public IObservableValue<bool> IsSpawning => spawning;
        
        public IEnumerable<Mob> SpawnedMobs => spawnedMobs;

        [SerializeField] private bool randomizeOrder;
        [SerializeField] private Mob[] prefabs;
        [SerializeField] private int spawnRate = 10;
        
        private int lastSpawnPointIndex;
        private Transform[] spawnPoints;

        private float counter;
        private int lastMobIndex;

        private readonly Observable<int> countdown = new(0);
        private readonly Observable<int> mobsLeft = new(0);

        private readonly Observable<int> wave = new(0);
        private readonly Observable<int> waveRecord = new(0);
        private readonly Observable<bool> spawning = new(false);
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
            waveRecord.Set(Game.Instance.Save.RecordWave);
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

            if (spawning)
            {
                return;
            }

            counter += Time.deltaTime;
            
            if (counter >= 1)
            {
                counter = 0;
                countdown.Set(countdown.Value - 1);
            }

            if (countdown.Value <= 0)
            {
                StartCoroutine(SpawnWave());
                countdown.Set(spawnRate);
                wave.Set(wave.Value + 1);
                if (wave.Value > waveRecord.Value)
                {
                    waveRecord.Set(wave.Value);
                }
            }
        }

        private IEnumerator SpawnWave()
        {
            spawning.Set(true);
            var amount = wave.Value;
            for (int i = 0; i < amount; i++)
            {
                if (Game.Instance.State.Value != GameState.Playing)
                {
                    yield break;
                }
                var point = spawnPoints[lastSpawnPointIndex];
                var index = randomizeOrder ? Random.Range(0, prefabs.Length) : lastMobIndex;
                lastMobIndex++;
                if (lastMobIndex >= prefabs.Length)
                {
                    lastMobIndex = 0;
                }

                var mob = Instantiate(prefabs[index].gameObject).GetComponent<Mob>();
                mob.transform.position = point.position;
                mob.transform.rotation = Quaternion.identity;
                mob.OnDestroyed += () =>
                {
                    OnMobKilled(mob);
                    spawnedMobs.Remove(mob);
                };
                spawnedMobs.Add(mob);
                mobsLeft.Set(mobsLeft.Value + 1);
                lastSpawnPointIndex++;

                if (lastSpawnPointIndex >= spawnPoints.Length)
                {
                    lastSpawnPointIndex = 0;
                }

                yield return new WaitForSeconds(1);
            }
            spawning.Set(false);
        }
        
        private void OnMobKilled(Mob mob)
        {
            mobsLeft.Set(mobsLeft.Value - 1);
            Game.Instance.Player.TrackKilledMob(mob);
        }

        public void CleanUp()
        {
            spawning.Set(false);
            counter = 0;
            wave.Set(1);
            countdown.Set(spawnRate);
            lastSpawnPointIndex = 0;
            foreach (Mob mob in spawnedMobs)
            {
                Destroy(mob.gameObject);
            }
            spawnedMobs.Clear();
        }
    }
}