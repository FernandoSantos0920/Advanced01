using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    private PlayerIdentifier playerID;
    private int coins;

    private int stars;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        playerID = GetComponent<PlayerIdentifier>();
    }
    
    private void OnEnable()
    {
         PlayerOM.CollectedCoin += Collectcoins;
         PlayerOM.CollectedStar += Collectstars;
    }
    
    private void OnDisable()
    {
         PlayerOM.CollectedCoin -= Collectcoins;
         PlayerOM.CollectedStar -= Collectstars;
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
    
    public void Collectstars(PlayerIdentifier.Player player)
    {
        Debug.Log("Peguei uma Estrela");
          
        if (player != playerID.ID)
        {
            return;
        }
                
        stars += 1;
        PlayerOM.StarsAreChanged(playerID.ID, stars);
    }
    
}
