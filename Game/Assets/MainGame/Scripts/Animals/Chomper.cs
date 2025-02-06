using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chomper : Monster
{
    private Vector3[] movePoint = new Vector3[4];
    private Vector3[] moveDirection = new Vector3[4];
    [SerializeField] GameObject[] AttackMotion;
    private Vector3[] attackBox = new Vector3[12];

    [SerializeField] GameObject Posion;
    [SerializeField] float duration = 1.5f;
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
        moveDirection[0] = new Vector3(2, 0, 2);
        moveDirection[1] = new Vector3(2, 0, -2);
        moveDirection[2] = new Vector3(-2, 0, 2);
        moveDirection[3] = new Vector3(-2, 0, -2);

        attackBox[0] = new Vector3(4, 0, 0);
        attackBox[1] = new Vector3(2, 0,0);
        attackBox[2] = new Vector3(2, 0, 2);
        attackBox[3] = new Vector3(2, 0, -2);
        attackBox[4] = new Vector3(0, 0, 2);
        attackBox[5] = new Vector3(0, 0, 4);
        attackBox[6] = new Vector3(-2, 0, 0);
        attackBox[7] = new Vector3(-2, 0, 2);
        attackBox[8] = new Vector3(-2, 0, -2);
        attackBox[9] = new Vector3(-4, 0, 0);
        attackBox[10] = new Vector3(0, 0, -2);
        attackBox[11] = new Vector3(0, 0, -4);

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
       
        base.AnimalAct(-1, attackable, true);
    }

    public override void Attack()
    {
        base.Attack();
        StartCoroutine(PosionMove());
        //Attack(AttackMotion, duration, AttackDamage);
        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().GetMoveDebuff(4);
    }

    IEnumerator PosionMove()
    {
        GameObject posion = Instantiate(Posion);
        posion.transform.position = transform.position;
        posion.tag = "Posion";

        bool reached = false;
        float speed = 3.0f * Time.deltaTime;
        while (!reached)  // 0.1f는 허용 오차
        {
            //posion null check
            // 목표 위치로 서서히 이동
            posion.transform.position = Vector3.MoveTowards(posion.transform.position, Hunter.HunterPosition, speed);
            if (posion.transform.position.x == Hunter.HunterPosition.x && posion.transform.position.z == Hunter.HunterPosition.z)
            {
                reached = true;
                break;
            }
            // 한 프레임 대기
            yield return null;
        }

    }

    public override void Move()
    {
        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = moveDirection[i] + transform.position;
        }
        //animationComponent.Play("Run");
        base.Move(transform.position, movePoint);
    }





    public override void Damaged(float dmg)
    {
        Health -= dmg;
        //animationComponent.Play("Damage");
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
