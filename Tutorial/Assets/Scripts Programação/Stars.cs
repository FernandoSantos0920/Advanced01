using UnityEngine;

public class Stars : MonoBehaviour
{
  
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
       
    }
    
    public void Update()
    {
       
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
          
            PlayerIdentifier playerID = collision.GetComponent<PlayerIdentifier>();

            if (playerID != null)
            {
                PlayerOM.StarAreCollected(playerID.ID);
                Destroy(gameObject);
            }
          
        }
    }

}
