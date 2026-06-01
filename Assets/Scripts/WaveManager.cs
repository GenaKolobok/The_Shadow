using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private float timeBetweenSpawns = 1f;

    [Header("Wave Scaling")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private int enemiesIncreasePerWave = 2;
    [SerializeField] private float difficultyMultiplier = 1.1f;

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private int enemiesSpawnedThisWave = 0;
    private int enemiesToSpawnThisWave = 0;
    private bool waveInProgress = false;
    private bool isSpawning = false;

    public event EventHandler<OnWaveChangedEventArgs> OnWaveChanged;
    public event EventHandler<OnEnemyCountChangedEventArgs> OnEnemyCountChanged;
    public event EventHandler OnWaveCompleted;

    public class OnWaveChangedEventArgs : EventArgs
    {
        public int waveNumber;
    }

    public class OnEnemyCountChangedEventArgs : EventArgs
    {
        public int enemiesAlive;
        public int totalEnemies;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Auto-set to Survival mode if testing this scene directly
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Campaign)
        {
            Debug.Log("Auto-setting game mode to Survival for testing");
            GameManager.Instance.SetGameMode(GameManager.GameMode.Survival);
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Survival)
        {
            StartCoroutine(StartWaveSystem());
        }
        else
        {
            Debug.LogWarning("WaveManager: GameManager not found or not in Survival mode!");
        }
    }

    private IEnumerator StartWaveSystem()
    {
        yield return new WaitForSeconds(3f);
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (waveInProgress || isSpawning)
            return;

        currentWave++;
        GameManager.Instance.CurrentWave = currentWave;

        enemiesToSpawnThisWave = CalculateEnemiesForWave(currentWave);
        enemiesSpawnedThisWave = 0;
        enemiesAlive = 0;

        OnWaveChanged?.Invoke(this, new OnWaveChangedEventArgs { waveNumber = currentWave });

        waveInProgress = true;
        StartCoroutine(SpawnWave());
    }

    private int CalculateEnemiesForWave(int wave)
    {
        return baseEnemiesPerWave + (wave - 1) * enemiesIncreasePerWave;
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        while (enemiesSpawnedThisWave < enemiesToSpawnThisWave)
        {
            SpawnEnemy();
            enemiesSpawnedThisWave++;

            UpdateEnemyCount();

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs or spawn points assigned!");
            return;
        }

        GameObject enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemyEntity enemyEntity = enemy.GetComponent<EnemyEntity>();
        if (enemyEntity != null)
        {
            enemyEntity.OnEnemyDeath += EnemyEntity_OnEnemyDeath;

            float healthMultiplier = Mathf.Pow(difficultyMultiplier, currentWave - 1);
            enemyEntity.ScaleHealth(healthMultiplier);
        }

        enemiesAlive++;
    }

    private void EnemyEntity_OnEnemyDeath(object sender, EventArgs e)
    {
        EnemyEntity enemyEntity = sender as EnemyEntity;
        if (enemyEntity != null)
        {
            enemyEntity.OnEnemyDeath -= EnemyEntity_OnEnemyDeath;
        }

        enemiesAlive--;
        GameManager.Instance.TotalEnemiesKilled++;

        UpdateEnemyCount();

        if (enemiesAlive <= 0 && !isSpawning && waveInProgress)
        {
            CompleteWave();
        }
    }

    private void UpdateEnemyCount()
    {
        OnEnemyCountChanged?.Invoke(this, new OnEnemyCountChangedEventArgs
        {
            enemiesAlive = enemiesAlive,
            totalEnemies = enemiesToSpawnThisWave
        });
    }

    private void CompleteWave()
    {
        waveInProgress = false;

        if (currentWave > GameManager.Instance.HighestWave)
        {
            GameManager.Instance.HighestWave = currentWave;
        }

        OnWaveCompleted?.Invoke(this, EventArgs.Empty);

        StartCoroutine(WaitForNextWave());
    }

    private IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public int GetEnemiesAlive()
    {
        return enemiesAlive;
    }

    public float GetTimeUntilNextWave()
    {
        return timeBetweenWaves;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
