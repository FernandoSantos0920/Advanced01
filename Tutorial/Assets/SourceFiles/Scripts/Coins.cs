using UnityEngine;

public class Coins : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
           PlayerCoins Player = collision.GetComponent<PlayerCoins>();
           Player.Collectcoins();
           Destroy(gameObject);
        }
    }
    
    
}
