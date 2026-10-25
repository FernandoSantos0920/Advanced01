using System;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [Header("Rotação")]
    [SerializeField] private float velocidadeRotacao = 90f;

    private void Update()
    {
        transform.Rotate(
            0f,
            velocidadeRotacao * Time.deltaTime,
            0f
        );
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerIdentifier playerID =
            collision.GetComponent<PlayerIdentifier>();

        if (playerID == null)
            return;

        PlayerOM.CoinAreCollected(playerID.ID);

        Destroy(gameObject);
    }
    
}
