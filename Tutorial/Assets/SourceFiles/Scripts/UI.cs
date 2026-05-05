using UnityEngine;

public class UI : MonoBehaviour
{
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {   
        Debug.Log("Mudando para estado: Gameplay");
        GameManager.Instance.ChangeState(GameManager.State.Gameplay);
    }
    
    public void Quit()
    {   
       Application.Quit();
    }
}
