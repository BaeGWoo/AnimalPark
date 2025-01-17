using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taipan : MonoBehaviour
{  
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    //[SerializeField] GameObject AttackMotion;
    [SerializeField] GameObject FireWall;
    [SerializeField] float duration = 2.0f;
   
    private AudioSource audioSource;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }



    public  void ActiveAttackBox()
    {
        animator.SetTrigger("Attack");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/" + gameObject.name + "Attack");
        audioSource.Play();

        AttackBox.SetActive(true);
        //AttackMotion.SetActive(true);
        FireWall.SetActive(true);

        StartCoroutine(UnActiveAttackBox());
    }



    private IEnumerator UnActiveAttackBox()
    {
        yield return new WaitForSeconds(duration);

        AttackBox.SetActive(false);
        //AttackMotion.SetActive(false);
        FireWall.SetActive(false);
    }

}
