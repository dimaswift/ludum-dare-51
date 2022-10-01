
using System.Collections.Generic;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class MobPartSpawner : MonoBehaviour
    {
        [SerializeField] private Vector2 spawnArea;
        [SerializeField] private MobPart[] prefabs;

        private MobPart lastPart;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var mobPart = Pool<MobPart>.GetFromPool(prefabs[Random.Range(0, prefabs.Length)]);
                var point = transform.position + new Vector3(Random.Range(-spawnArea.x / 2, spawnArea.x / 2), Random.Range(-spawnArea.y / 2, spawnArea.y / 2));
                mobPart.transform.position = point;
                mobPart.transform.rotation = Quaternion.identity;
                mobPart.Randomize();
                lastPart = mobPart;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, spawnArea);
        }
    }
}