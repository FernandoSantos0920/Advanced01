using UnityEngine;

public class PlayerCoins : MonoBehaviour
{

    private int coins;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    
    public void Collectcoins()
    {
            Debug.Log("Peguei uma moeda");
           
            coins += 1;
            PlayerOM.CoinsAreChanged(coins);
    }
}
