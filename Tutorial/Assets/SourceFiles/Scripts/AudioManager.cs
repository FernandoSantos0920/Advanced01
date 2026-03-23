using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    
    public static AudioManager Instance;

    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClip;
    
    
    private void Awake()
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
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSound(int clipIndex)
    {
        audioSource.clip = audioClip[clipIndex];
        audioSource.Play();
        
    }



    public void StopSound(int clipIndex)
    {
        
        audioSource.clip = null;
        audioSource.Stop();
    }
}
