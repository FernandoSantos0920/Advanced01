using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    
   

    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] List<AudioSource> activeSource;
    
     
    #region Singleton Logic
    
     public static AudioManager Instance;
         
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                audioSource = GetComponent<AudioSource>();
                activeSource = new List<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
   


    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 #endregion
  
    
    #region 2D Audio Logic

    

    public void Play(AudioClip clip)
       {
          audioSource.Stop();
          audioSource.clip = clip;
          audioSource.Play();
           
       }
   
   
   
       public void PlayOneShot(AudioClip clip)
       {
           audioSource.PlayOneShot(clip);
       }
   
       public void Stop()
       {
           audioSource.Stop();
       }
   
       public void Resume()
       {
           audioSource.UnPause();
       }
       
       public void Pause()
       {
           audioSource.Pause();
       }
       
    
    
    
    #endregion


    #region 3D Audio Logic
    
    
    public void Play(AudioClip clip, AudioSource source)
    {
        if (!activeSource.Contains(source))
            activeSource.Add(source);
        source.Stop();
        source.clip = clip;
        source.Play();
        
    }


    public void Stop(AudioSource source)
    {
        if(activeSource.Contains(source))
            activeSource.Remove(source);
        source.Stop();
    }

    public void Resume(AudioSource source)
    {
        source.UnPause();
    }
    
    public void Pause(AudioSource source)
    {
        source.Pause();
    }

    #endregion
}
