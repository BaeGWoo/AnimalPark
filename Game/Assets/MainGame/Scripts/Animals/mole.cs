using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mole : Animal
{
    private Vector3[] movePoint = new Vector3[65];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health = 3;
    public bool attackable = false;
    public bool hitable = false;



    private void Awake()
    {
       
        int index = 0;
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                movePoint[index++] = new Vector3(i * 2, 0, j * 2);
            }
        }


        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
    }

    public override void Move()
    {
        AIManager.TileMap[(int)(transform.position.x / 2), (int)(transform.position.z / 2)] = 0;
        Vector3 target = Hunter.HunterPosition;


        float distance = 100;
        int minDirection = 0;



        // 이동할 위치 중 target과 가장 인접한 위치 찾기
        for (int i = 0; i < movePoint.Length; i++)
        {
            float temp;
            temp = Mathf.Abs((movePoint[i].x - target.x) + (movePoint[i].z - target.z));

            if (movePoint[i].x >= -0.01f && movePoint[i].x <= 14 && movePoint[i].z >= -0.01f && movePoint[i].z <= 14)
            {
                if (AIManager.TileMap[(int)(movePoint[i].x / 2), (int)(movePoint[i].z / 2)] != 1)
                {
                    if (temp < distance)
                    {
                        distance = temp;
                        minDirection = i;
                    }
                }
            }
        }
        AIManager.TileMap[(int)(movePoint[minDirection].x / 2), (int)(movePoint[minDirection].z) / 2] = 1;
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
