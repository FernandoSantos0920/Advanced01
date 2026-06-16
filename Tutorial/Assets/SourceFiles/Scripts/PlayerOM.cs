using System;
using UnityEngine;

public static class PlayerOM 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Action<int> ChangeCoins;

    public static void CoinsAreChanged(int quantidade)
    {
       ChangeCoins?.Invoke(quantidade);
    }
    
    public static event Action<bool> Colidiu;

    public static void ColidiuIsChanged(bool value)
    {
        Colidiu?.Invoke(value);
    }
}
