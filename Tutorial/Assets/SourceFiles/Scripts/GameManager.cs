using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private PlayerInput playerInput;

    
    private State changeState;

    #region Singleton

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            changeState = State.Iniciando;
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
        
        Debug.Log("Boot finalizado");

        SceneManager.LoadScene(1);
    }

    void Update()
    {
        Debug.Log(changeState);
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
                SceneManager.LoadScene(2);
                DesativarInput();
                break;

            case State.Gameplay:
                SceneManager.LoadScene(3);
                AtivarInput();
                break;
        }
    }

    #region InputPlayer

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
        {
           Debug.Log("Input Valido");
        }
        else
        {
          Debug.Log("Input Invalido");
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
}