using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Monster
{
    private Vector3[] movePoint = new Vector3[8];
    private Vector3[] moveDirection = new Vector3[8];
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
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
        moveDirection[0] = new Vector3(2, 0, 0);
        moveDirection[1] = new Vector3(-2, 0, 0);
        moveDirection[2] = new Vector3(0, 0, -2);
        moveDirection[3] = new Vector3(0, 0, 2);
        moveDirection[4] = new Vector3(4, 0, 0);
        moveDirection[5] = new Vector3(-4, 0, 0);
        moveDirection[6] = new Vector3(0, 0, -4);
        moveDirection[7] = new Vector3(0, 0, 4);

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
        AttackBox.SetActive(false);
        base.AnimalAct(-1, false, true);
    }


    public override void Attack()
    {
        base.Attack();
        Attack(AttackMotion, duration, AttackDamage);
        base.Die();
    }


    public override void Move()
    {
        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = moveDirection[i] + transform.position;
        }
        animationComponent.Play("Run");
        base.Move(transform.position, movePoint);
        AttackBox.SetActive(true);
    }


    public override void Damaged(float dmg)
    {
        Health -= dmg;
        animationComponent.Play("Damage");
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hunter")
        {
            Attack();
            other.GetComponent<Hunter>().OnReachedDestination();
            other.GetComponent<Hunter>().StopPosition(new Vector3(transform.position.x,0,transform.position.z));
        }
    }



}