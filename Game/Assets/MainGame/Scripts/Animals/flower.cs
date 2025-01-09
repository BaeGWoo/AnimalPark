using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class flower : Monster
{
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] Vector3[] attackBox=new Vector3[8];
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health = 2;
    [SerializeField] float MaxHealth = 2;
    [SerializeField] int skillCount;
    [SerializeField] int totalSkillCount;

    public bool attackable = false;
    public float AttackDamage = 0;
    private Vector3 startSize;
  
    private void Awake()
    {    
        startSize = transform.localScale;
        attackBox[0] = new Vector3(0, 0, 2);
        attackBox[1] = new Vector3(0, 0, -2);
        attackBox[2] = new Vector3(-2, 0, 0);
        attackBox[3] = new Vector3(2, 0, 0);
        attackBox[4] = new Vector3(2, 0, 2);
        attackBox[5] = new Vector3(2, 0, -2);
        attackBox[6] = new Vector3(-2, 0, 2);
        attackBox[7] = new Vector3(-2, 0, -2);
    }

    public override void SetAnimalStatus(float Attack, float Health, int skillCount)
    {
        this.Health = Health;
        MaxHealth = Health;
        AttackDamage = Attack;

        this.skillCount = skillCount;
        totalSkillCount = skillCount;
    }

    public override void AnimalAct()
    {
        skillCount--;
        if (skillCount < 0) { skillCount = totalSkillCount; }

        base.AnimalAct(-1, true, false);
    }



    public override void Attack()
    {
        base.Attack();
        if(attackable)
            Attack(AttackMotion, duration, AttackDamage);
        else
            Attack(AttackMotion, duration, -1);

        SizeUp();
    }

    public override void Damaged(float dmg)
    {
        Health-=dmg;
        base.Damaged(dmg);
        if (Health <= 0) base.Die();
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
        attackable = false;
        base.GetAttackAble(attackBox);
        return attackable;
    }


    public override void SetAttackAble(bool value, Vector3 cur)
    {      
        attackable = value;
    }


    public void SizeUp()
    {
        Vector3 targetSize = startSize * 3;
        transform.localScale = targetSize;
        StartCoroutine(ResetSize());


    }

    IEnumerator ResetSize()
    {
        yield return new WaitForSeconds(1.5f);
        transform.localScale = startSize;
    }
}
