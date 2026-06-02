using UnityEngine;

public class NextScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameManager.Instance.TrocaDeCena("Menu", null);
    }
   
   
}
