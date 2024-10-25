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
    public static bool fireball = false;
    public static bool chooseDirection = false;

    [SerializeField] AIManager aiManager;
    [SerializeField] BoxCollider Huntercollider;
    [SerializeField] Slider HPSlider;
    [SerializeField] GameObject bananaMotion;
    [SerializeField] GameObject[] AttackWeapon;
    [SerializeField] GameObject MenuPanel;

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
        Health = 10;
    }

    public void Move()
    {
        Moveable = true;
       
    }

    public void Attack()
    {
        Attackable = true;
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        MenuPanel.SetActive(true);
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
            HPSlider.value = Health / 10;
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
        if (animatorTrigger == "Attack2")
        {
            fireball = true;
        }
        int num = int.Parse(animatorTrigger.Substring("Attack".Length));
       

        if (fireball)
        {
            chooseDirection = false;

            StartCoroutine(ChooseFireBallDirection(num));
        }

        else
        {
            animator.SetTrigger("Attack" + num);
            AttackWeapon[num - 1].SetActive(true);
            StartCoroutine(HunterAttackMotion(num));

            StartCoroutine(HunterAttackEnd(num));
        }


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

    IEnumerator ChooseFireBallDirection(int num)
    {
        while (!chooseDirection)
        {
            yield return null;
        }

        animator.SetTrigger("Attack" + num);
        AttackWeapon[num - 1].SetActive(true);
        
        StartCoroutine(HunterAttackMotion(num));
       // StartCoroutine(Fireball());
        StartCoroutine(HunterAttackEnd(num));
    }

   // IEnumerator Fireball()
   // {
   //     StartCoroutine(MoveTowardsTarget());
   //    
   //     yield return new WaitForSeconds(1.0f);
   // }
   //
   // private IEnumerator MoveTowardsTarget()
   // {
   //     Debug.Log("Fireballmove");
   //
   //     // Fireball과 Target의 월드 좌표를 저장
   //     Vector3 Fireball = AttackWeapon[1].transform.position;
   //     Vector3 targetPosition = AttackTarget.transform.position;
   //     Debug.Log("Fireball : " + Fireball);
   //     Debug.Log("Target : " + targetPosition);
   //     while (Vector3.Distance(AttackWeapon[1].transform.position, targetPosition) > 0.1f)
   //     {
   //         Vector3 cur = AttackWeapon[1].transform.position;
   //
   //         // 월드 좌표에서 이동
   //         AttackWeapon[1].transform.position = Vector3.MoveTowards(cur, targetPosition, 2.0f * Time.deltaTime);
   //
   //         yield return null; // 매 프레임마다 이동
   //     }
   //
   //     // 공격이 끝나면 Fireball을 비활성화하고 초기 위치로 되돌리기
   //     //AttackWeapon[1].SetActive(false);
   //     AttackWeapon[1].transform.position = Fireball;
   //
   //     yield return new WaitForSeconds(1.0f);
   // }

}
