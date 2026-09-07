using TMPro;
using UnityEngine;

public class UiGUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] TextMeshProUGUI coins;
   private IDdaUI iddaUI;
    void Awake()
    {
        iddaUI = GetComponent<IDdaUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PlayerOM.ChangeCoins += UpdateCoinsText;
    }
    
    private void OnDisable()
    {
        PlayerOM.ChangeCoins -= UpdateCoinsText;
    }


    private void UpdateCoinsText(PlayerIdentifier.Player player, int quantidade)
    {

        if (player != iddaUI.ID)
        {
            return;
        }
       
        coins.text = quantidade.ToString();
    }
}
