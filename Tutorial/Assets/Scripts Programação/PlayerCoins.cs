using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    private PlayerIdentifier playerID;
    private int coins;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        playerID = GetComponent<PlayerIdentifier>();
    }
    
    private void OnEnable()
    {
         PlayerOM.CollectedCoin += Collectcoins;
    }
    
    private void OnDisable()
    {
         PlayerOM.CollectedCoin -= Collectcoins;
    }
    
    public void Collectcoins(PlayerIdentifier.Player player)
    {
            Debug.Log("Peguei uma moeda");
          
            if (player != playerID.ID)
            {
                return;
            }
                
            coins += 1;
            PlayerOM.CoinsAreChanged(playerID.ID, coins);
    }
}
