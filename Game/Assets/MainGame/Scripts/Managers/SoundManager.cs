using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClip;
    private AudioSource backgroundAudioSource;  // ��������� ���� AudioSource
    private AudioSource effectAudioSource;      // ȿ������ ���� AudioSource
    private static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        backgroundAudioSource = Camera.main.GetComponent<AudioSource>();
    }
    

    public void SoundPlay(string name)
    {
        for(int i=0;i<audioClip.Length; i++)
        {
            audioClip[i].name = name;
            
            return;
        }
    }
}
