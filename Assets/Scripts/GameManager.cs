using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public enum GameMode
    {
        Campaign,
        Survival
    }

    public GameMode CurrentGameMode { get; private set; }

    public int CurrentHealth;
    public int MaxHealth;

    public float BonusSpeed;
    public int BonusDamage;

    // Survival mode stats
    public int CurrentWave { get; set; }
    public int TotalEnemiesKilled { get; set; }
    public int HighestWave { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetGameMode(GameMode mode)
    {
        CurrentGameMode = mode;
    }

    public void ResetSurvivalStats()
    {
        CurrentWave = 0;
        TotalEnemiesKilled = 0;
    }

    public void ResetPlayerStats()
    {
        CurrentHealth = 0;
        MaxHealth = 0;
        BonusSpeed = 0;
        BonusDamage = 0;
    }
}