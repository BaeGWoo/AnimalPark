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

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
}
