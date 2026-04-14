using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  
    public static GameManager Instance;
    
    
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
        ChangeState(changeState);
        Debug.Log(changeState);

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            changeState = State.MenuPrincipal;
        }
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
                      
                        break;


                    case State.MenuPrincipal:

                        SceneManager.LoadScene(1);

                        break;

                    case State.Gameplay :

                        SceneManager.LoadScene(2);

                        break;


          }
    }
    
    
    
}
