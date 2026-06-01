using UnityEngine;

public class WinObject : MonoBehaviour
{
    private bool playerInside;

    [SerializeField] private GameObject winPanel;

    private void Start()
    {
        GameInput.Instance.OnInteract += GameInput_OnInteract;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= GameInput_OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            playerInside = false;
        }
    }

    private void GameInput_OnInteract(object sender, System.EventArgs e)
    {
        if (playerInside)
        {
            Win();
        }
    }

    private void Win()
    {
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }
}