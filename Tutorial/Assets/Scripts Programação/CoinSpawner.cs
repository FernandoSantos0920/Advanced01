using System.Collections;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private GameObject moedaPrefab;
    [SerializeField] private Transform[] pontosDeSpawn;

    [Header("Respawn")]
    [SerializeField] private float tempoRespawn = 3f;

    private GameObject moedaAtual;
    private Transform ultimoPonto;
    private bool aguardandoRespawn;

    private void OnEnable()
    {
        PlayerOM.CollectedCoin += AoColetarMoeda;
    }

    private void OnDisable()
    {
        PlayerOM.CollectedCoin -= AoColetarMoeda;
    }

    private void Start()
    {
        SpawnarMoeda();
    }

    private void AoColetarMoeda(PlayerIdentifier.Player player)
    {
        if (aguardandoRespawn || moedaAtual == null)
            return;

        moedaAtual = null;

        StartCoroutine(RespawnarMoeda());
    }

    private IEnumerator RespawnarMoeda()
    {
        aguardandoRespawn = true;

        yield return new WaitForSeconds(tempoRespawn);

        SpawnarMoeda();

        aguardandoRespawn = false;
    }

    private void SpawnarMoeda()
    {
        if (moedaPrefab == null ||
            pontosDeSpawn == null ||
            pontosDeSpawn.Length == 0)
        {
            Debug.LogWarning(
                "Configure o prefab e os pontos de spawn.",
                this
            );

            return;
        }

        Transform pontoEscolhido;

        if (pontosDeSpawn.Length == 1)
        {
            pontoEscolhido = pontosDeSpawn[0];
        }
        else
        {
            do
            {
                pontoEscolhido = pontosDeSpawn[
                    Random.Range(0, pontosDeSpawn.Length)
                ];
            }
            while (pontoEscolhido == ultimoPonto);
        }

        ultimoPonto = pontoEscolhido;

        // Usa a posição do ponto e a rotação do prefab.
        moedaAtual = Instantiate(
            moedaPrefab,
            pontoEscolhido.position,
            moedaPrefab.transform.rotation
        );
    }
}