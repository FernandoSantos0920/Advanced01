using System;
using UnityEngine;

public static class PlayerOM 
{
    public static Action CollectedCoin;
   
    public static Action<int> ChangeCoins;

    public static void CoinsAreChanged(int quantidade)
    {
       ChangeCoins?.Invoke(quantidade);
    }
    
    public static void CoinAreCollected()
    {
        CollectedCoin?.Invoke();
    }
    
}
