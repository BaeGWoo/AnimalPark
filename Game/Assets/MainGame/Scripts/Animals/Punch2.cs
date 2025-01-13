using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch2 : Monster
{
    private Vector3[] movePoint = new Vector3[8];
    private Vector3[] moveDirection = new Vector3[8];
    [SerializeField] GameObject[] AttackMotion;
    private Vector3[] attackBox = new Vector3[4];
    [SerializeField] Vector3 curAttackBox;
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health = 2;
    [SerializeField] float MaxHealth = 2;
    [SerializeField] int skillCount;
    [SerializeField] int totalSkillCount;

    public float AttackDamage = 0;
    public bool attackable = false;
    public bool hitable = false;
    private Animation animationComponent;

    private void Awake()
    {
        moveDirection[0] = new Vector3(4, 0,2);
        moveDirection[1] = new Vector3(4, 0,-2);
       
        moveDirection[2] = new Vector3(-4, 0, 2);
        moveDirection[3] = new Vector3(-4, 0, 2);

        moveDirection[4] = new Vector3(-2, 0, 4);
        moveDirection[5] = new Vector3(-2, 0, -4);

        moveDirection[6] = new Vector3(2, 0, 4);
        moveDirection[7] = new Vector3(2, 0, -4);
        



        attackBox[0] = new Vector3(2, 0, 2);
        attackBox[1] = new Vector3(-2, 0, 2);
        attackBox[2] = new Vector3(2, 0, -2);
        attackBox[3] = new Vector3(-2, 0, -2);


        aiManager = FindObjectOfType<AIManager>();
        animationComponent = GetComponent<Animation>();
    }

    public override void SetAnimalStatus(float Attack, float Health, int skillCount)
    {
        this.Health = Health;
        MaxHealth = Health;
        AttackDamage = Attack;

        totalSkillCount = skillCount;
    }

    public override void AnimalAct()
    {
        skillCount--;
        if (skillCount < 0) { skillCount = totalSkillCount; }
        base.AnimalAct(-1, false, true);
    }


    public override void Attack()
    {
        base.Attack();
        AttackMotion[0].transform.position = Hunter.HunterPosition;
        Attack(AttackMotion, duration, AttackDamage);
        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().GetHealthDebuff(3, 0.5f);
    }


    public override void Move()
    {
        int index = 0;
        for (int i = 0; i < movePoint.Length; i++)
        {
            for (int j = 0; j < attackBox.Length; j++)
            {
                movePoint[index++] = moveDirection[i] + transform.position + attackBox[j];
            }

        }
        animationComponent.Play("Run");
        base.Move(transform.position, movePoint);
    }


    public override void Damaged(float dmg)
    {
        Health -= dmg;
        animationComponent.Play("Damage");
        base.Damaged(dmg);
        if (Health <= 0) base.Die();
    }

    public override bool GetAttackAble()
    {
        attackable = false;
        base.GetAttackAble(attackBox);
        return attackable;
    }


    public override void SetAttackAble(bool value, Vector3 cur)
    {
        curAttackBox = cur;
        attackable = value;
    }

    public override float GetHP()
    {
        return Health;
    }

    public override float GetMaxHp()
    {
        return MaxHealth;
    }
}
