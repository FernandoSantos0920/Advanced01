using UnityEngine;

public class AudioTocar : MonoBehaviour
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
        AudioManager.Instance.StartSound(0);
    }

    public void Stop()
    {
        AudioManager.Instance.StopSound(0);
    }
}
