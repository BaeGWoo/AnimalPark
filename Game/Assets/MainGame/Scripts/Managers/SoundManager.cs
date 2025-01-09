using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] bgm;
    public AudioClip[] audioClip;
    [SerializeField] private AudioSource backgroundAudioSource;  // 배경음악을 위한 AudioSource
    private AudioSource effectAudioSource;      // 효과음을 위한 AudioSource
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
        effectAudioSource=gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        BGMPlay();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 7)
        {
            BGMPlay();
            Destroy(gameObject);
        }
    }

    public void SoundPlay(string name)
    {
        effectAudioSource.clip = Resources.Load<AudioClip>("Sounds/"+name);
        effectAudioSource.Play();
    }

    public void SetBGMPlayer()
    {
        backgroundAudioSource = Camera.main.GetComponent<AudioSource>();
    }


    public void MoveStage()
    {
        backgroundAudioSource = Camera.main.GetComponent<AudioSource>();
        backgroundAudioSource.loop = false;
        backgroundAudioSource.clip= Resources.Load<AudioClip>("Sounds/UISound/MouseClick");
        backgroundAudioSource.Play();
    }

    public void BGMOff()
    {
        backgroundAudioSource.clip = null;
    }

    public void BGMPlay()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = bgm[index];
        //Debug.Log(index);
        backgroundAudioSource.Play();
    }

    public void EnterLobby()
    {
        backgroundAudioSource.clip = bgm[0];
        backgroundAudioSource.Play();
    }
}
