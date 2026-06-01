using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    private void Update()
    {
        if (Player.Instance != null)
        {
            healthText.text =
                "HP: " +
                Player.Instance.GetCurrentHealth() +
                " / " +
                Player.Instance.GetMaxHealth();
        }
    }
}