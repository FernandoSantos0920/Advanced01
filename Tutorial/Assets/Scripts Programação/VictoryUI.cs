using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryText;

    private void Awake()
    {
        victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerOM.PlayerVictory += ShowVictory;
    }

    private void OnDisable()
    {
        PlayerOM.PlayerVictory -= ShowVictory;
    }

    private void ShowVictory(PlayerIdentifier.Player player)
    {
        victoryPanel.SetActive(true);

        victoryText.text = player + " VENCEU!";
        
        Time.timeScale = 0;
    }
}
    

