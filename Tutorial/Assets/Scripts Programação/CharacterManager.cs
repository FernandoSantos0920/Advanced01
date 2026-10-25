using UnityEngine;
using TMPro;
public class CharacterManager : MonoBehaviour
{
   [Header("Opções de Robôs")]
    [SerializeField] private RoboData[] opcoesRobo = new RoboData[5];

    [Header("Interface")]
    [SerializeField] private TMP_Text textoInstrucao;

    [Header("Cenas")]
    [SerializeField] private string gameplaySceneName = "GetStarted_Scene";
    [SerializeField] private string sceneUI = "GUI";
    private int jogadorAtual = 1;
    private bool selecaoConcluida;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager não foi encontrado.");
            enabled = false;
            return;
        }

        if (opcoesRobo == null || opcoesRobo.Length != 5)
        {
            Debug.LogError("Configure exatamente cinco opções de RoboData.");
            enabled = false;
            return;
        }

        for (int i = 0; i < opcoesRobo.Length; i++)
        {
            if (opcoesRobo[i] == null)
            {
                Debug.LogError($"A opção {i + 1} não possui RoboData.");
                enabled = false;
                return;
            }
        }

        AtualizarInstrucao();
    }

    public void SelecionarRobo(int indice)
    {
        if (selecaoConcluida)
            return;

        if (indice < 0 || indice >= opcoesRobo.Length)
        {
            Debug.LogWarning("Índice de robô inválido.");
            return;
        }

        RoboData escolhido = opcoesRobo[indice];

        PlayerIdentifier.Player jogador = jogadorAtual == 1 ? PlayerIdentifier.Player.Player1 : PlayerIdentifier.Player.Player2;

        GameManager.Instance.DefinirRobo(jogador, escolhido);

        Debug.Log($"Jogador {jogadorAtual} escolheu {escolhido.roboName}.");

        if (jogadorAtual == 1)
        {
            jogadorAtual = 2;
            AtualizarInstrucao();
        }
        else
        {
            selecaoConcluida = true;
            GameManager.Instance.TrocaDeCena(gameplaySceneName, sceneUI);
        }
    }

    private void AtualizarInstrucao()
    {
        if (textoInstrucao == null)
            return;

        textoInstrucao.text = jogadorAtual == 1 ? "Jogador 1: escolha seu robô" : "Jogador 2: escolha seu robô";
    }
}

