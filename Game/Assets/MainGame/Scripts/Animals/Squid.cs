using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Squid : Monster
{
    [SerializeField] GameObject[] AttackMotion;
    private Vector3[] attackBox = new Vector3[8];
    [SerializeField] bool attackable = false;

    [SerializeField] float duration = 2.0f;

    public float AttackDamage = 0;
 

    [SerializeField] int skillCount;
    public int totalSkillCount;
    


    public override void SetAnimalStatus(float Attack, float Health,int skillCount)
    {
       
        AttackDamage = Attack;

        this.skillCount = skillCount;
        totalSkillCount = skillCount;
    }

    public override void AnimalAct()
    {
        skillCount--;
        if (skillCount < 0) { skillCount = totalSkillCount; }

        base.AnimalAct(skillCount, false, false);
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

    public override void Skill()
    {
        base.Attack();

        if (attackable)
            Attack(AttackMotion, duration, AttackDamage);
        else
            Attack(AttackMotion, duration, -1);
        base.Die();
    }

    public override float GetHP()
    {
        return 1;
    }

    public override float GetMaxHp()
    {
        return 1;
    }
   
}
