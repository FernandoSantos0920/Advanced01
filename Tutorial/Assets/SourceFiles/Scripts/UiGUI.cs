using TMPro;
using UnityEngine;

public class UiGUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] TextMeshProUGUI coins;
    
    void Start()
    {
        
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
        PlayerOM.ChangeCoins += UpdateCoinsText;
    }


    private void UpdateCoinsText(int quantidade)
    {
        coins.text = quantidade.ToString();
    }
}
