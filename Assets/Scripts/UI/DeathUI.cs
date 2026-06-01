using System.Collections;
using UnityEngine;
using TMPro;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private float deathPanelDelay = 2f;

    [Header("Survival Mode Statistics")]
    [SerializeField] private GameObject survivalStatsPanel;
    [SerializeField] private TextMeshProUGUI finalWaveText;
    [SerializeField] private TextMeshProUGUI totalKillsText;

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        StartCoroutine(ShowDeathPanelAfterDelay());
    }

    private IEnumerator ShowDeathPanelAfterDelay()
    {
        // Wait for death animation to play
        yield return new WaitForSecondsRealtime(deathPanelDelay);

        Time.timeScale = 0f;
        deathPanel.SetActive(true);

        // Show survival stats only in Survival mode
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Survival)
        {
            ShowSurvivalStats();
        }
        else
        {
            HideSurvivalStats();
        }
    }

    private void ShowSurvivalStats()
    {
        if (survivalStatsPanel != null)
            survivalStatsPanel.SetActive(true);

        if (finalWaveText != null)
        {
            finalWaveText.text = $"Достигнута волна: {GameManager.Instance.CurrentWave}";
        }

        if (totalKillsText != null)
        {
            totalKillsText.text = $"Всего убито: {GameManager.Instance.TotalEnemiesKilled}";
        }

        // Update highest wave record in background
        if (GameManager.Instance.CurrentWave > GameManager.Instance.HighestWave)
        {
            GameManager.Instance.HighestWave = GameManager.Instance.CurrentWave;
        }
    }

    private void HideSurvivalStats()
    {
        if (survivalStatsPanel != null)
            survivalStatsPanel.SetActive(false);
    }
}