using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coins;
    private IDdaUI iddaUI;

    [SerializeField] private TextMeshProUGUI stars;
    [SerializeField] private TextMeshProUGUI derrubadas;

    [Header("Barra de Recarga")]
    [SerializeField] private Slider barraRecarga;

    private void Awake()
    {
        iddaUI = GetComponent<IDdaUI>();
    }

    private void OnEnable()
    {
        PlayerOM.ChangeCoins += UpdateCoinsText;
        PlayerOM.ChangeStars += UpdateStarsText;
        PlayerOM.ChangeAttackCooldown += UpdateAttackCooldown;
        PlayerOM.ChangeKnockouts += UpdateKnockoutsText;
    }

    private void OnDisable()
    {
        PlayerOM.ChangeCoins -= UpdateCoinsText;
        PlayerOM.ChangeStars -= UpdateStarsText;
        PlayerOM.ChangeAttackCooldown -= UpdateAttackCooldown;
        PlayerOM.ChangeKnockouts -= UpdateKnockoutsText;
    }

    private void UpdateCoinsText(
        PlayerIdentifier.Player player,
        int quantidade)
    {
        if (player != iddaUI.ID)
            return;

        coins.text = quantidade.ToString();
    }

    private void UpdateStarsText(
        PlayerIdentifier.Player player,
        int quantidade)
    {
        if (player != iddaUI.ID)
            return;

        stars.text = quantidade.ToString();
    }

    private void UpdateAttackCooldown(
        PlayerIdentifier.Player player,
        float progresso)
    {
        if (player != iddaUI.ID)
            return;

        if (barraRecarga == null)
            return;

        barraRecarga.value = Mathf.Clamp01(progresso);
    }

    private void UpdateKnockoutsText(
        PlayerIdentifier.Player player,
        int quantidade)
    {
        if (player != iddaUI.ID)
            return;

        if (derrubadas == null)
            return;

        derrubadas.text = quantidade.ToString();
    }
}