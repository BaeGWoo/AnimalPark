using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Colobus :Animal
{
    private Vector3[] movePoint= new Vector3[9];
    private Vector3[] moveDirection = new Vector3[8];
    private Animator animator;
    [SerializeField] float duration = 2.0f;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;

    [SerializeField] float Health = 3;
    private float MaxHealth = 3;

    public bool attackable = false;
    public bool hitable = false;
    private void Awake()
    {
        moveDirection[0] = new Vector3(-2, 0, 4);
        moveDirection[1] = new Vector3(-4, 0, 2);
        moveDirection[2] = new Vector3(-4, 0, -2);
        moveDirection[3] = new Vector3(-2, 0, -4);

        moveDirection[4] = new Vector3(2, 0, 4);
        moveDirection[5] = new Vector3(4, 0, 2);
        moveDirection[6] = new Vector3(4, 0, -2);
        moveDirection[7] = new Vector3(2, 0, -4);


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

    public override void JumpAnimaition(){animator.SetTrigger("Jump");}

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

    public override void ColobusAttack()
    {
        StartCoroutine(MoveTowardsTarget());
    }

    private IEnumerator MoveTowardsTarget()
    {
        Vector3 banana = AttackMotion[0].transform.position;      
        Vector3 Target = Hunter.HunterPosition;

        while (Vector3.Distance(AttackMotion[0].transform.position, Target) > 0.1f)
        {
            Vector3 cur = AttackMotion[0].transform.position;
           
            AttackMotion[0].transform.position = Vector3.MoveTowards(cur, Target, 5.0f * Time.deltaTime);
          
            // 매 프레임마다 한 번씩 이동
            yield return null;
        }
        AttackMotion[0].SetActive(false);
        AttackMotion[0].transform.position = banana;
        
    }

    public override void Damaged()
    {
        Health--;
        animator.SetTrigger("Damage");
        if (Health <= 0)
        {
            aiManager.RemoveAnimal(gameObject);
            animator.SetTrigger("Die");
            base.Die();
        }

        base.Damaged();
    }

    public override float GetHP()
    {
        return Health;
    }

    public override float GetMaxHp()
    {
        return MaxHealth;
    }

    public override bool GetAttackAble()
    {
        return attackable;
    }
}
