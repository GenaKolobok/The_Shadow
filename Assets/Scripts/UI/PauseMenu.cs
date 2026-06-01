using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameObject musicOnIcon;
    [SerializeField] private GameObject musicOffIcon;

    private bool isPaused;
    private bool isMusicOn = true;

    private void Start()
    {
        UpdateMusicButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (rulesPanel != null)
            rulesPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (rulesPanel != null)
            rulesPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowRules()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (rulesPanel != null)
            rulesPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        if (rulesPanel != null)
            rulesPanel.SetActive(false);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (musicSource != null)
        {
            if (isMusicOn)
                musicSource.UnPause();
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

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}