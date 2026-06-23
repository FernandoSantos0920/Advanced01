using System;
using UnityEngine;

public class Coins : MonoBehaviour
{
   private Transform coinTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        coinTransform = GetComponent<Transform>();
    }
    
    public void Update()
    {
        coinTransform.rotation = Quaternion.Euler(coinTransform.rotation.eulerAngles.x, coinTransform.rotation.eulerAngles.y + 0.6f, 0);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerOM.CollectedCoin?.Invoke();
            Destroy(gameObject);
        }
    }
    
    
}
