using LastWick.Combat;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Code.Runtime.Bosses
{
    // Faz o boss do marco atual nascer quando a arena fica limpa.
    public class BossSpawner : MonoBehaviour
    {
        [Tooltip("Prefabs dos bosses, na ordem dos marcos.")]
        [SerializeField] private GameObject[] bosses;

        [SerializeField] private Transform player;

        private void OnEnable()
        {
            GameEvents.MilestoneReached += OnMilestoneReached;
        }

        private void OnDisable()
        {
            GameEvents.MilestoneReached -= OnMilestoneReached;
        }

        private void OnMilestoneReached(int milestoneIndex)
        {
            SpawnBoss(milestoneIndex);
        }

        private void SpawnBoss(int index)
        {
            Console.WriteLine($"index: {index} | Boss count: {bosses.Count().ToString()} |");

            var boss = bosses[index];

            GameObject instance = Instantiate(boss, GetSpawnPosition(), Quaternion.identity);

            GameEvents.RaiseBossSpawned(instance.GetComponent<Health>());
        }

        private Vector2 GetSpawnPosition()
        {
            PlayerLight light = player.GetComponentInChildren<PlayerLight>();

            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;

            float radius = light.Radius + 1 + 1;
            Vector2 position = (Vector2)player.position + direction * radius;

            return position;
        }
    }
}
