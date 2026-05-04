using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  
    public static GameManager Instance;
    private PlayerInput playerInput;
    
    private State changeState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        }
        
        
       
    }
  
    #endregion
   
    
    void Start()
    {
      
    }
    // Update is called once per frame
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
                        AtivarInput();
                        SceneManager.LoadScene(2);
                        
                        break;

                    case State.Gameplay :
                        AtivarInput();
                        SceneManager.LoadScene(3);

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
            Debug.Log("Input Achado");
            
        }
        else
        {
            Debug.Log("Input Perdido?");
        }
    }
    
    
    
    
    public void DesativarInput()
    {
        if (playerInput != null)
        {
             playerInput.DeactivateInput();
        }
          
    }

    public void AtivarInput()
    {
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
    }
    
    #endregion
}
