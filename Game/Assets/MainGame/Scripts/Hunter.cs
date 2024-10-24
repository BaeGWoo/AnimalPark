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
    [SerializeField] GameObject bananaMotion;
    [SerializeField] GameObject[] AttackWeapon;

    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    [SerializeField] GameObject[] AttackBox;
    public float Health;

    void Awake()
    {
        
        HunterPosition = new Vector3(0, 0, 0);
       Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        HPPosition = HPSlider.transform.position;
        Health = 5;
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
       
        animator.SetBool("Run", Running);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            other.transform.root.GetComponent<Animal>().Attack();
            Health--;
            HPSlider.value = Health / 5;
        }
        else if (other.CompareTag("banana"))
        {
            StartCoroutine(BananaMotion());
            Health--;
            HPSlider.value = Health / 5;
        }

    }

    IEnumerator BananaMotion()
    {
        bananaMotion.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        bananaMotion.SetActive(false);
    }

    public void AttackType(Button button)
    {
        if (!Attackable) return;
        string animatorTrigger = button.name;
        int num = int.Parse(animatorTrigger.Substring("Attack".Length));
        animator.SetTrigger("Attack" + num);
        AttackWeapon[num - 1].SetActive(true);
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
        AttackWeapon[num - 1].SetActive(false);
    }
}
