using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class mole : Animal
{
    private Vector3[] movePoint = new Vector3[12];
    private Vector3[] moveDirection = new Vector3[12];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health = 2;
    private float MaxHealth = 2;
    public bool attackable = false;
    public bool hitable = false;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();

        Vector3 target = Hunter.HunterPosition;
       
        int index = 0;
        // ¿ì»ó(2,0,2)
        for (int i = 1; i <= 3; i++)
        {
            moveDirection[index++] = new Vector3(2 * i, 0.5f, 2 * i);
        }
        //¿ìÇÏ(2,0,-2)
        for (int i = 1; i <= 3; i++)
        {
            moveDirection[index++] = new Vector3(2 * i, 0.5f, - 2 * i);
        }
        //ÁÂ»ó(-2,0,2)
        for (int i = 1; i <= 3; i++)
        {
            moveDirection[index++] = new Vector3(-2 * i, 0.5f,  2 * i);
        }
        //ÁÂÇÏ(-2,0,-2)
        for (int i = 1; i <= 3; i++)
        {
            moveDirection[index++] = new Vector3(- 2 * i, 0.5f,  - 2 * i);
        }

    }

    public override void Move()
    {
       for(int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = transform.position + moveDirection[i];
        }


        base.Move(transform.position, transform.rotation, movePoint);
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
        base.Attack();
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration);
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
