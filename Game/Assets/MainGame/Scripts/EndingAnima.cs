using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingAnima : MonoBehaviour
{
    private float speed = 2.0f;
    public Animator animator;
    [SerializeField] GameObject train;
   
    [SerializeField] GameObject trainCamera;
    [SerializeField] GameObject Smoke;
    private AudioSource backgroundAudioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        DestroyAll();
       backgroundAudioSource =Camera.main.GetComponent<AudioSource>();
    }

   public void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

   public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        backgroundAudioSource.clip = Resources.Load<AudioClip>("Sounds/BGM/Ending");
        StartCoroutine(EndingStart());
    }

    private void Start()
    {
        StartCoroutine(EndingStart());
    }


    IEnumerator startEndingTrigger()
    {
        animator.SetTrigger("Walk");
        yield return null;
    }

    IEnumerator EndingStart()
    {
        StartCoroutine(startEndingTrigger());
        while (true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z+ (speed * Time.deltaTime));
            yield return null;
        }

    }

   


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            trainCamera.SetActive(true);
            Smoke.SetActive(true);
            train.GetComponent<Train>().TrainStart();
            Destroy(gameObject);
            
        }
    }


    public void DestroyAll()
    {
        // �� ��ȯ ���Ŀ��� ��Ƴ��� ��� ��ü�� ã�Ƽ� ����
        GameObject[] dontDestroyObjects = GameObject.FindGameObjectsWithTag("DontDestroyObject");

        foreach (GameObject obj in dontDestroyObjects)
        {
            Destroy(obj);
        }
    }
}
