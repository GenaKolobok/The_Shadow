using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private bool playerInside;

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
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}