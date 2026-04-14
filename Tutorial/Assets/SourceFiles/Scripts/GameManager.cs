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
        }
        else
        { 
            Destroy(gameObject);
        }
        
        changeState = State.Iniciando;
        Debug.Log(changeState);
    }
  
    #endregion
   
    
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

        switch (changeState)
        {
            case State.Iniciando:
               
                
              
                break;
            
            
            case State.MenuPrincipal:
              
                
                
                break;
          
            case State.Gameplay :
               
             
                
                break;
            
            
        }
    }

    private enum State
    {
        Iniciando,
        MenuPrincipal,
        Gameplay
    }


}
