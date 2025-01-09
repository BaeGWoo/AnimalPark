using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elegator : Monster
{
    private Vector3[] movePoint = new Vector3[12];
    private Vector3[] moveDirection = new Vector3[12];
    private Vector3[] attackBox = new Vector3[8];
    [SerializeField] Vector3 curAttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] GameObject SquidBomb;
    [SerializeField] float duration = 2.0f;
    [SerializeField] float Health = 3;
    [SerializeField] float MaxHealth = 3;
    public float AttackDamage = 0;
   
    public bool attackable = false;
    public bool hitable = false;
    [SerializeField] int skillCount = 2;
    [SerializeField] int totalSkillCount = 2;
   

    private void Awake()
    {
        moveDirection[0] = new Vector3(0, 0, 2);
        moveDirection[1] = new Vector3(0, 0, -2);
        moveDirection[2] = new Vector3(-2, 0, 0);
        moveDirection[3] = new Vector3(2, 0, 0);

        moveDirection[4] = new Vector3(-2, 0, 2);
        moveDirection[5] = new Vector3(-2, 0, -2);
        moveDirection[6] = new Vector3(2, 0, 2);
        moveDirection[7] = new Vector3(2, 0, -2);

        moveDirection[8] = new Vector3(-4, 0, 4);
        moveDirection[9] = new Vector3(4, 0, 4);
        moveDirection[10] = new Vector3(-4, 0, -4);
        moveDirection[11] = new Vector3(4, 0, -4);


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
        MaxHealth = Health;
        AttackDamage = Attack;

        this.skillCount = skillCount;
        totalSkillCount = skillCount;
    }


    public override void AnimalAct()
    {
        skillCount--;
        if (skillCount < 0) { skillCount = totalSkillCount; }

        base.AnimalAct(skillCount, attackable, true);
    }


    public override void Move()
    {
        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = moveDirection[i] + transform.position;
        }

        base.Move(transform.position, movePoint);
    }

    public override void Skill()
    {
        Move();
        GameObject prefab;
        int randomNumber = Random.Range(0, 5);
        prefab = Instantiate(SquidBomb);
        prefab.name = SquidBomb.name;
       prefab.transform.position = transform.position;
        FindAnyObjectByType<AIManager>().GetComponent<AIManager>().AddAnimal(prefab);
       
    }

    public override void Attack()
    {
        base.Attack();
        float attackDirX=curAttackBox.x-transform.position.x;
        float attackDirZ=curAttackBox.z-transform.position.z;

        if (attackDirZ > 0)
        {
            attackDirX = 1.0f;
            attackDirZ = 0;
        }

        else if (attackDirZ < 0)
        {
            attackDirX = -1.0f;
            attackDirZ = 0;
        }

        else if (attackDirZ == 0) 
        {
            if (attackDirX > 0)
            {
                attackDirZ = -1.0f;
                attackDirX = 0;
            }
            else
            {
                attackDirZ = 1.0f;
                attackDirX = 0;
            }
        }
        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().ElegatorAttack(attackDirX,attackDirZ);
    }

    public override bool GetAttackAble()
    {
        attackable = false;
        base.GetAttackAble(attackBox);
        return attackable;
    }


    public override void SetAttackAble(bool value,Vector3 cur)
    {
        curAttackBox = cur;
        attackable = value;
    }



    public override void Damaged(float dmg)
    {
        Health-=dmg;
       
        if (Health <= 0)
        {         
            base.Die();
        }
        base.Damaged(dmg);
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
