using System;
using UnityEngine;

public static class PlayerOM 
{
  
    public static Action<PlayerIdentifier.Player> PlayerRegistered;
    
    public static Action<PlayerIdentifier.Player> CollectedCoin;
   
    public static Action<PlayerIdentifier.Player, int> ChangeCoins;

    public static Action<PlayerIdentifier.Player> PlayerVictory;
     
    public static Action<PlayerIdentifier.Player> CollectedStar;
     
    public static Action<PlayerIdentifier.Player, int> ChangeStars;
    
    public static Action<PlayerIdentifier.Player, float> ChangeAttackCooldown;
    
    public static Action<PlayerIdentifier.Player, PlayerIdentifier.Player> PlayerKnockedOut;
    
    public static Action<PlayerIdentifier.Player, int> ChangeKnockouts;
    
    public static void RegisterPlayer(PlayerIdentifier.Player player)
    {
        PlayerRegistered?.Invoke(player);
    }
    
    public static void CoinsAreChanged(PlayerIdentifier.Player player, int quantidade)
    {
       ChangeCoins?.Invoke(player, quantidade);
    }
    
    public static void CoinAreCollected(PlayerIdentifier.Player player)
    {
        CollectedCoin?.Invoke(player);
    }
    
    public static void StarsAreChanged(PlayerIdentifier.Player player, int quantidade)
    {
        ChangeStars?.Invoke(player, quantidade);
    }
    
    public static void StarAreCollected(PlayerIdentifier.Player player)
    {
        CollectedStar?.Invoke(player);
    }
    
    public static void PlayerWinned(PlayerIdentifier.Player player)
    {
        PlayerVictory?.Invoke(player);
    }
    
    public static void AttackCooldownChanged(PlayerIdentifier.Player player, float progresso)
    {
        ChangeAttackCooldown?.Invoke(player, progresso);
    }
    
    public static void PlayerWasKnockedOut(PlayerIdentifier.Player attacker, PlayerIdentifier.Player victim)
    {
        PlayerKnockedOut?.Invoke(attacker, victim);
    }
    
    public static void KnockoutsAreChanged(PlayerIdentifier.Player player, int quantidade)
    {
        ChangeKnockouts?.Invoke(player, quantidade);
    }
}
