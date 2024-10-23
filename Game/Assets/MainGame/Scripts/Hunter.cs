using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunter : MonoBehaviour
{
    public static Vector3 HunterPosition;
    public static Transform HunterRotation;

    public static bool Moveable = false;
    public static bool Running = false;
    public static bool Attackable = false;

    [SerializeField] AIManager aiManager;
    [SerializeField] BoxCollider Huntercollider;
    [SerializeField] Slider HPSlider;
    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    [SerializeField] GameObject[] AttackBox;

    void Awake()
    {
        
        HunterPosition = new Vector3(0, 0, 0);
       Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        HPPosition = HPSlider.transform.position;
    }

    public void Move()
    {
        Moveable = true;
       
    }

    public void Attack()
    {
        Attackable = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            other.transform.root.GetComponent<Animal>().Attack();
        Debug.Log(other.transform.root.name);
        }
    }

    public void AttackType(Button button)
    {
        if (!Attackable) return;
        string animatorTrigger = button.name;
        int num = int.Parse(animatorTrigger.Substring("Attack".Length));
        animator.SetTrigger("Attack" + num);

        StartCoroutine(HunterAttackMotion(num));
       
        StartCoroutine(HunterAttackEnd(num));
    }

    IEnumerator HunterAttackMotion(int num)
    {
        yield return new WaitForSeconds(0.5f);
        AttackBox[num - 1].SetActive(true);

    }

    IEnumerator HunterAttackEnd(int num)
    {
        yield return new WaitForSeconds(2.5f);

        Attackable = false;
        AttackBox[num - 1].SetActive(false);
    }
}
