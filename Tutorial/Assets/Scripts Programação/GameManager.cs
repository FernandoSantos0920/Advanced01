using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private PlayerInput playerInput;

    
    private State changeState;

    [SerializeField] private int totalcoins;
    private int coinsCollected;
    private int Player1Coins;    
    private int Player2Coins;


    private bool gameEnded;

    #region Singleton

    void Awake()
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

    void Start()
    {
       
        StartCoroutine(IniciarDepoisDoBoot());
    }

    IEnumerator IniciarDepoisDoBoot()
    {
       for(int i = 0; i < 5; i++)
        {
            yield return null;
        }
        
      //  Debug.Log("Boot finalizado");

        SceneManager.LoadScene(1);
    }

    void Update()
    {
       // Debug.Log(changeState);
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
        {
              return;
        }
          

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

        if (NomeCena2 != null)
        {
             SceneManager.LoadScene(NomeCena2, LoadSceneMode.Additive);
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
    
    

    #region InputPlayer

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayerOM.ChangeCoins += OnCoinsChanged;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerOM.ChangeCoins -= OnCoinsChanged;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
        {
          // Debug.Log("Input Valido");
        }
        else
        {
         // Debug.Log("Input Invalido");
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

    private void OnCoinsChanged(PlayerIdentifier.Player player, int quantidade)
    {
        if (gameEnded)
        {
            return;
        }

        if (player == PlayerIdentifier.Player.Player1)
        {
            Player1Coins = quantidade;
        }
        else if (player == PlayerIdentifier.Player.Player2)
        {
            Player2Coins = quantidade;
        }
        
        coinsCollected += 1;

        if (coinsCollected < totalcoins)
        {
            return;
        }

        MatchEnded();
    }


    private void MatchEnded()
    {
        gameEnded = true;

        if (Player1Coins > Player2Coins)
        {
            PlayerOM.PlayerWinned(PlayerIdentifier.Player.Player1);
        }
        else if (Player2Coins > Player1Coins)
        {
            PlayerOM.PlayerWinned(PlayerIdentifier.Player.Player2);
        }
    
    }
    #endregion
}