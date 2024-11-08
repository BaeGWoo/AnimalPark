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
    [SerializeField] float Health = 2;
    private float MaxHealth = 2;
    public bool attackable = false;
    public float AttackDamage = 0;
    public bool hitable = false;
    private Vector3 startSize;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
        startSize = transform.localScale;
    }

    public override void SetAnimalStatus(float Attack, float Health)
    {
        MaxHealth = Health;
        AttackDamage = Attack;
    }

    public override float AnimalDamage() { return AttackDamage; }





    public override void ActiveAttackBox()
    {
        attackable = true;
        AttackBox.SetActive(true);
        Attack();
    }

    public override void UnActiveAttackBox()
    {
        ResetSize();
        attackable = false;
        AttackBox.SetActive(false);
    }

    public override void Attack()
    {
        base.Attack();
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration);
        SizeUp();
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

    public void SizeUp()
    {
      
        Vector3 targetSize = startSize * 3;

        transform.localScale = targetSize;
    }

    public void ResetSize()
    {
        transform.localScale = startSize;
    }
}
