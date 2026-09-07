using System;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    

    [SerializeField] private Player id;

    public Player ID => id;
    void Start()
    {
        PlayerOM.RegisterPlayer(id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public enum Player
    {
        Player1, Player2
    }
}
