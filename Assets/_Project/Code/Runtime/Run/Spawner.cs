using Assets._Project.Code.Runtime.Run;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnStage[] stages;
    [SerializeField] private Transform player;

    private float timer;
    private float stageTime;
    private int currentStage;
    private int currentAlive;
    private bool spawning = true;
    private bool missingWavesLogged;

    [SerializeField] private PlayerLight light;

    [Tooltip("Largura da faixa em que o inimigo aparece como silhueta, além do raio da luz.")]
    [SerializeField, Min(0f)] private float silhouetteBand = 3f;

    [Tooltip("Distância extra para o inimigo nascer completamente invisível.")]
    [SerializeField, Min(0f)] private float spawnMargin = 1f;

    [Tooltip("Intervalo de spawn enquanto os vivos estão abaixo do mínimo da onda.")]
    [SerializeField, Min(0.02f)] private float fillInterval = 0.1f;

    private void OnEnable()
    {
        GameEvents.PlayerDied += OnPlayerDied;
        GameEvents.MilestoneReached += OnMilestoneReached;
        GameEvents.BossDefeated += OnBossDefeated;
    }
    private void OnDisable()
    {
        GameEvents.PlayerDied -= OnPlayerDied;
        GameEvents.MilestoneReached -= OnMilestoneReached;
        GameEvents.BossDefeated -= OnBossDefeated;
    }
    void Awake()
    {
        light = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLight>();
    }

    void FixedUpdate()
    {
        currentAlive = GameObject.FindGameObjectsWithTag("Enemy").Count();

        if (!spawning) return;
        var stage = stages[currentStage];

        if (stage.waves == null || stage.waves.Length == 0)
        {
            if (!missingWavesLogged)
            {
                Debug.LogError($"O stage {currentStage} não tem ondas.", this);
                missingWavesLogged = true;
            }

            return;
        }

        stageTime += Time.deltaTime;

        if (currentAlive >= stage.maxAlive) return;

        SpawnWave wave = CurrentWave(stage);
        float interval = currentAlive < wave.minAlive ? fillInterval : wave.spawnInterval;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            GameObject enemy = PickEnemy(stage);

            if (enemy == null) return;

            Vector2 position = GetSpawnPosition();

            Instantiate(enemy, position, Quaternion.identity);

            timer -= interval;
            return;
        }

        return;
    }

    private void OnPlayerDied() => spawning = false;
    private void OnMilestoneReached(int milestoneIndex) 
    {
        spawning = false;

        OnStageCleared();
    }

    private void OnBossDefeated(int bossIndex)
    {
        currentStage = bossIndex + 1;
        spawning = currentStage < stages.Length;

        stageTime = 0f;

        missingWavesLogged = false;
    }

    private void OnStageCleared()
    {
        var enemiesCount = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemiesCount.Count() > 0)
        {
            foreach (GameObject go in enemiesCount) 
            {
                Destroy(go);
            }
        }
    }

    private SpawnWave CurrentWave(SpawnStage stage)
    {
        var elapsed = stageTime;

        foreach (var wave in stage.waves)
        {
            if (wave.duration <= 0) continue;

            if (elapsed < wave.duration) return wave;

            elapsed -= wave.duration;
        }

        return stage.waves[^1];
    }

    private GameObject PickEnemy(SpawnStage stage)
    {
        float weight = 0f;
        foreach (var e in stage.enemies) weight += e.weight;

        if (weight <= 0f) return null;

        float roll = Random.Range(0f, weight);

        foreach (var entry in stage.enemies)
        {
            roll -= entry.weight;
            if (roll <= 0f)
            {
                return entry.prefab;
            }
        }

        return stage.enemies[^1].prefab;
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;

        float radius = light.Radius + silhouetteBand + spawnMargin;
        Vector2 position = (Vector2)player.position + direction * radius;

        return position;
    }

 }
