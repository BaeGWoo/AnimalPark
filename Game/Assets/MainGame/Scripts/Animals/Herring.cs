                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herring : Animal
{
    private Vector3[] movePoint = new Vector3[5];
    private Vector3[] moveDirection = new Vector3[4];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float Health = 3;
    [SerializeField] float duration = 2.0f;
    private float MaxHealth = 3;
    public float AttackDamage = 0;
    public bool attackable = false;
    public bool hitable = false;

    private void Awake()
    {
        moveDirection[0] = new Vector3(4, 0, 4);
        moveDirection[1] = new Vector3(-4, 0, 4);
        moveDirection[2] = new Vector3(-4, 0, -4);
        moveDirection[3] = new Vector3(4, 0, -4);


       

        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();

    }

    public override void SetAnimalStatus(float Attack, float Health)
    {
        MaxHealth = Health;
        AttackDamage = Attack;
    }

    public override float AnimalDamage() { return AttackDamage; }

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
