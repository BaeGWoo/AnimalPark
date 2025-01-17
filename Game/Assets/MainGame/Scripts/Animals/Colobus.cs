using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Colobus :Monster
{
    private Vector3[] movePoint = new Vector3[1];
    private Vector3[] moveDirection = new Vector3[4];
    [SerializeField] GameObject[] AttackMotion;



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
        moveDirection[0] = new Vector3(2, 0, 4);
        moveDirection[1] = new Vector3(2, 0, -4);
        moveDirection[2] = new Vector3(-2, 0,4);
        moveDirection[3] = new Vector3(-2, 0, -4);
        moveDirection[4] = new Vector3(-4, 0, 0);
        moveDirection[5] = new Vector3(4, 0, 0);

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
        base.AnimalAct(-1, true, true);
    }

    public override void Attack()
    {
        base.Attack();
        BananaMove();
        Attack(AttackMotion, duration, AttackDamage);
        Move();
    }

    IEnumerator BananaMove()
    {
        float speed = 2.0f * Time.deltaTime;
        float distance = Mathf.Abs(AttackMotion[0].transform.position.x - Hunter.HunterPosition.x) +
            Mathf.Abs(AttackMotion[0].transform.position.z - Hunter.HunterPosition.z);
        while (distance > 0.1f)  // 0.1f는 허용 오차
        {
            // 목표 위치로 서서히 이동
            AttackMotion[0].transform.position = Vector3.MoveTowards(transform.position, Hunter.HunterPosition, speed);

            // 한 프레임 대기
            yield return null;
        }

        AttackMotion[0].transform.position = Hunter.HunterPosition;

    }

    public override void Move()
    {
        int index = 0;
        Vector3 HunterPosition = Hunter.HunterPosition;
        float dir = 0;

        for (int i = 0; i < moveDirection.Length; i++)
        {
            float temp = Mathf.Abs(HunterPosition.x - (moveDirection[i].x + transform.position.x)) +
                  Mathf.Abs(HunterPosition.z - (moveDirection[i].z + transform.position.z));

            if (dir < temp)
            {
                dir = temp;
                index = i;
            }
        }
        movePoint[0] = moveDirection[index] + transform.position;

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



    public override float GetHP()
    {
        return Health;
    }

    public override float GetMaxHp()
    {
        return MaxHealth;
    }


}
