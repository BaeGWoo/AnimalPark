using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparrow : Animal
{
    private Vector3[] movePoint = new Vector3[9];
    private Vector3[] moveDirection = new Vector3[8];
    private Animator animator;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject AttackTargetBox;
    [SerializeField] float duration = 3.0f;
    [SerializeField] float Health = 3;
    public bool attackable = false;
    public bool hitable = false;

    private void Awake()
    {
        moveDirection[0] = new Vector3(-2f, 0, 4);
        moveDirection[1] = new Vector3(-4f, 0, 2);
        moveDirection[2] = new Vector3(2f, 0, 4);
        moveDirection[3] = new Vector3(4f, 0, 2);
        moveDirection[4] = new Vector3(-4f, 0, -2);
        moveDirection[5] = new Vector3(-2f, 0, -4);
        moveDirection[6] = new Vector3(2f, 0, -4);
        moveDirection[7] = new Vector3(4f, 0, -2);
        

        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
    }

    public override void Move()
    {
        movePoint[0] = transform.position;

        for (int i = 1; i < movePoint.Length; i++)
        {
            movePoint[i] = new Vector3(moveDirection[i - 1].x + transform.position.x, 0, moveDirection[i - 1].z + transform.position.z);
        }
        base.Move(transform.position, transform.rotation, movePoint);
    }
    public override void JumpAnimaition()
    {
        animator.SetTrigger("Jump");
    }




    public override void ActiveAttackBox()
    {
        attackable = true;
        AttackBox.SetActive(true);
    }

    public override void UnActiveAttackBox()
    {
        attackable = false;
        AttackBox.SetActive(false);
    }

    public override void Attack()
    {
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration);
    }

    public override void SparrowAttack()
    {
        StartCoroutine(MoveTowardsTarget());
    }

    private IEnumerator MoveTowardsTarget()
    {
        Vector3 LeftWing= AttackMotion[0].transform.position;
        Vector3 RightWing = AttackMotion[1].transform.position;


        while (Vector3.Distance(AttackMotion[1].transform.position, AttackTargetBox.transform.position) > 0.1f)
        {
            Vector3 Lcur = AttackMotion[0].transform.position;
            Vector3 Rcur = AttackMotion[1].transform.position;
            AttackMotion[0].transform.position = Vector3.MoveTowards(Lcur, AttackTargetBox.transform.position, 2.0f * Time.deltaTime);
            AttackMotion[1].transform.position = Vector3.MoveTowards(Rcur, AttackTargetBox.transform.position, 2.0f * Time.deltaTime);

            // 매 프레임마다 한 번씩 이동
            yield return null;
        }
        AttackMotion[0].transform.position = LeftWing;
        AttackMotion[1].transform.position = RightWing;

        for(int i = 0; i < AttackMotion.Length; i++)
        {
            AttackMotion[i].SetActive(false);
        }
    }

    public override void Damaged()
    {
        Health--;
        if (Health <= 0)
        {
            aiManager.RemoveAnimal(gameObject);
            animator.SetTrigger("Die");
            base.Die();
        }
    }

    public override float GetHP()
    {
        return Health;
    }

    public override bool GetAttackAble()
    {
        return attackable;
    }

}
