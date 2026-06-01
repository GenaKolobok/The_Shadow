using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Mode Selection")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject rulesPanel;

    [Header("Scene Names")]
    [SerializeField] private string campaignSceneName = "Game";
    [SerializeField] private string survivalSceneName = "SurvivalMode";

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameObject musicOnIcon;
    [SerializeField] private GameObject musicOffIcon;

    private bool isMusicOn = true;

    private void Start()
    {
        ShowMainMenu();
        UpdateMusicButton();
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(false);

        if (rulesPanel != null)
            rulesPanel.SetActive(false);
    }

    public void ShowModeSelection()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(true);

        if (rulesPanel != null)
            rulesPanel.SetActive(false);
    }

    public void ShowRules()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(false);

        if (rulesPanel != null)
            rulesPanel.SetActive(true);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (musicSource != null)
        {
            if (isMusicOn)
                musicSource.Play();
            else
                musicSource.Pause();
        }

        UpdateMusicButton();
    }

    private void UpdateMusicButton()
    {
        if (musicOnIcon != null)
            musicOnIcon.SetActive(isMusicOn);

        if (musicOffIcon != null)
            musicOffIcon.SetActive(!isMusicOn);
    }

    public void StartCampaign()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameMode(GameManager.GameMode.Campaign);
            GameManager.Instance.ResetPlayerStats();
            GameManager.Instance.ResetSurvivalStats();
        }

        // Включаем управление при старте новой игры
        if (GameInput.Instance != null)
        {
            GameInput.Instance.EnableMovement();
        }

        SceneManager.LoadScene(campaignSceneName);
    }

    public void StartSurvival()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameMode(GameManager.GameMode.Survival);
            GameManager.Instance.ResetPlayerStats();
            GameManager.Instance.ResetSurvivalStats();
        }

        // Включаем управление при старте новой игры
        if (GameInput.Instance != null)
        {
            GameInput.Instance.EnableMovement();
        }

        SceneManager.LoadScene(survivalSceneName);
    }

    public void StartGame()
    {
        ShowModeSelection();
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}