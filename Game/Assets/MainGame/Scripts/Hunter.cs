using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    public static Vector3 HunterPosition;
    public static Transform HunterRotation;
    public static bool Moveable = false;
    public static bool Running = false;
    [SerializeField] AIManager aiManager;
    [SerializeField] BoxCollider Huntercollider;
    private Animator animator;

    void Awake()
    {
        
        HunterPosition = new Vector3(0, 0, 0);
       Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    public void Move()
    {
        Moveable = true;
       
    }

    public void Attack()
    {
        Moveable = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = HunterPosition;
       Huntercollider.enabled=!Moveable;
        if (Running)
        {
            Debug.Log("RUN");
            
        }

        animator.SetBool("Run", Running);
    }
}
