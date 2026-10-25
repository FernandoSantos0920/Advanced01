using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerInput playerInput;

    private State changeState;

    [Header("Estrelas")]
    [SerializeField] private int totalStars;

    private int starsCollected;
    private int Player1Stars;
    private int Player2Stars;

    private bool gameEnded;

    [Header("Contagem de Derrubadas")]
    [SerializeField] private int derrubadasParaVencer = 3;

    private int player1Derrubadas;
    private int player2Derrubadas;

    [Header("Seleção de Robôs")]
    [SerializeField] private RoboData roboEscolhidoPlayer1;
    [SerializeField] private RoboData roboEscolhidoPlayer2;

    public RoboData RoboEscolhidoPlayer1 => roboEscolhidoPlayer1;
    public RoboData RoboEscolhidoPlayer2 => roboEscolhidoPlayer2;

    #region Singleton

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            changeState = State.Iniciando;
            Debug.Log(changeState);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    private void Start()
    {
        StartCoroutine(IniciarDepoisDoBoot());
    }

    private IEnumerator IniciarDepoisDoBoot()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }

        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        // Atualizações gerais do jogo, se necessárias.
    }

    public enum State
    {
        Iniciando,
        MenuPrincipal,
        Gameplay
    }

    public void ChangeState(State newState)
    {
        if (changeState == newState)
            return;

        changeState = newState;

        switch (changeState)
        {
            case State.Iniciando:
                DesativarInput();
                break;

            case State.MenuPrincipal:
                Debug.Log(changeState);
                DesativarInput();
                break;

            case State.Gameplay:
                Debug.Log(changeState);
                AtivarInput();
                break;
        }
    }

    public void TrocaDeCena(string NomeCena, string NomeCena2)
    {
        SceneManager.LoadScene(NomeCena);

        if (!string.IsNullOrEmpty(NomeCena2))
        {
            SceneManager.LoadScene(
                NomeCena2,
                LoadSceneMode.Additive
            );
        }

        if (NomeCena == "Menu")
        {
            ChangeState(State.MenuPrincipal);
        }
        else if (NomeCena == "GetStarted_Scene")
        {
            ChangeState(State.Gameplay);
        }
    }

    #region Seleção de Robôs

    public void DefinirRobo(
        PlayerIdentifier.Player jogador,
        RoboData dados)
    {
        if (dados == null)
        {
            Debug.LogWarning(
                "Não é possível selecionar um RoboData nulo."
            );

            return;
        }

        if (jogador == PlayerIdentifier.Player.Player1)
        {
            roboEscolhidoPlayer1 = dados;
        }
        else if (jogador == PlayerIdentifier.Player.Player2)
        {
            roboEscolhidoPlayer2 = dados;
        }

        Debug.Log(
            $"{jogador} escolheu o robô: {dados.roboName}"
        );
    }

    private void AplicarRobosEscolhidos()
    {
        // Se as escolhas não estiverem completas, não altera os robôs.
        if (roboEscolhidoPlayer1 == null ||
            roboEscolhidoPlayer2 == null)
        {
            Debug.LogWarning(
                "As escolhas dos dois jogadores ainda não foram definidas."
            );

            return;
        }

        // Verifica se os dois selecionaram o mesmo asset RoboData.
        bool mesmoRobo =
            roboEscolhidoPlayer1 == roboEscolhidoPlayer2;

        StarterAssets.ThirdPersonController[] jogadores =
            FindObjectsByType<StarterAssets.ThirdPersonController>(
                FindObjectsSortMode.None
            );

        foreach (StarterAssets.ThirdPersonController jogador in jogadores)
        {
            PlayerIdentifier identificador =
                jogador.GetComponent<PlayerIdentifier>();

            if (identificador == null)
            {
                Debug.LogWarning(
                    $"{jogador.name} não possui PlayerIdentifier."
                );

                continue;
            }

            if (identificador.ID == PlayerIdentifier.Player.Player1)
            {
                jogador.ConfigurarRobo(roboEscolhidoPlayer1, false);
            }
            else if (identificador.ID == PlayerIdentifier.Player.Player2)
            {
                jogador.ConfigurarRobo(roboEscolhidoPlayer2, mesmoRobo);
            }
        }

        if (mesmoRobo)
        {
            Debug.Log("Os dois jogadores escolheram o mesmo robô. " +
                      "O Jogador 2 usará a cor secundária.");
        }
    }

    #endregion

    #region InputPlayer

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayerOM.ChangeStars += OnStarsChanged;
        PlayerOM.PlayerKnockedOut += OnPlayerKnockedOut;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        PlayerOM.ChangeStars -= OnStarsChanged;
        PlayerOM.PlayerKnockedOut -= OnPlayerKnockedOut;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerInput = FindFirstObjectByType<PlayerInput>();

        totalStars = FindObjectsByType<Stars>(
            FindObjectsSortMode.None
        ).Length;

        // Aplica as escolhas somente quando a gameplay é carregada.
        if (scene.name == "GetStarted_Scene")
        {
            AplicarRobosEscolhidos();
        }
    }

    public void DesativarInput()
    {
        if (playerInput != null)
            playerInput.DeactivateInput();
    }

    public void AtivarInput()
    {
        if (playerInput != null)
            playerInput.ActivateInput();
    }

    #endregion

    #region Estrelas

    private void OnStarsChanged(
        PlayerIdentifier.Player player,
        int quantidade)
    {
        if (gameEnded)
            return;

        if (player == PlayerIdentifier.Player.Player1)
        {
            Player1Stars = quantidade;
        }
        else if (player == PlayerIdentifier.Player.Player2)
        {
            Player2Stars = quantidade;
        }

        starsCollected += 1;

        if (starsCollected < totalStars)
            return;

        MatchEnded();
    }

    private void MatchEnded()
    {
        gameEnded = true;

        if (Player1Stars > Player2Stars)
        {
            PlayerOM.PlayerWinned(
                PlayerIdentifier.Player.Player1
            );
        }
        else if (Player2Stars > Player1Stars)
        {
            PlayerOM.PlayerWinned(
                PlayerIdentifier.Player.Player2
            );
        }
    }

    #endregion

    #region Derrubadas

    private void OnPlayerKnockedOut(
        PlayerIdentifier.Player attacker,
        PlayerIdentifier.Player victim)
    {
        if (gameEnded)
            return;

        if (attacker == victim)
            return;

        if (attacker == PlayerIdentifier.Player.Player1)
        {
            player1Derrubadas++;

            PlayerOM.KnockoutsAreChanged(
                PlayerIdentifier.Player.Player1,
                player1Derrubadas
            );

            Debug.Log(
                $"Player 1: {player1Derrubadas} derrubadas"
            );
        }
        else if (attacker == PlayerIdentifier.Player.Player2)
        {
            player2Derrubadas++;

            PlayerOM.KnockoutsAreChanged(
                PlayerIdentifier.Player.Player2,
                player2Derrubadas
            );

            Debug.Log(
                $"Player 2: {player2Derrubadas} derrubadas"
            );
        }
        else
        {
            return;
        }

        if (player1Derrubadas >= derrubadasParaVencer)
        {
            MatchEndedByKnockouts(
                PlayerIdentifier.Player.Player1
            );
        }
        else if (player2Derrubadas >= derrubadasParaVencer)
        {
            MatchEndedByKnockouts(
                PlayerIdentifier.Player.Player2
            );
        }
    }

    private void MatchEndedByKnockouts(
        PlayerIdentifier.Player vencedor)
    {
        if (gameEnded)
            return;

        gameEnded = true;

        PlayerOM.PlayerWinned(vencedor);

        Debug.Log($"{vencedor} venceu por derrubadas!");
    }

    #endregion
}