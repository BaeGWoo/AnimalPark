using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flower : Animal
{
    private Vector3[] movePoint = new Vector3[1];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health = 3;
    public bool attackable = false;
    public bool hitable = false;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
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

    public override bool GetAttackAble()
    {
        return attackable;
    }
}
