using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : Monster
{
    private Vector3[] movePoint = new Vector3[1];
    private Vector3[] moveDirection = new Vector3[4];
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] GameObject magicSpell;
   
  
    [SerializeField] float duration = 1.5f;
    [SerializeField] float Health = 2;
    [SerializeField] float MaxHealth = 2;
    [SerializeField] int skillCount;
    [SerializeField] int totalSkillCount;
    [SerializeField] int TaipanCount=2;
    [SerializeField] GameObject[] Taipans;
    public float AttackDamage = 0;
    public bool attackable = false;
    public bool hitable = false;
    private Animation animationComponent;

    private void Awake()
    {
        moveDirection[0] = new Vector3(4, 0, 4);
        moveDirection[1] = new Vector3(4, 0, -4);
        moveDirection[2] = new Vector3(-4, 0, 4);
        moveDirection[3] = new Vector3(-4, 0, -4);

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
        TaipanCount--;
        if (TaipanCount == 0)
        {
            TaipanCount = 2;
            for (int i = 0; i < Taipans.Length; i++) 
            {
                Taipans[i].GetComponent<Taipan>().ActiveAttackBox();
            }
        }
        if (skillCount < 0) { skillCount = totalSkillCount; }
        base.AnimalAct(skillCount, false, true);
    }

    public override void Skill()
    {
        magicSpell.SetActive(true);
        StartCoroutine(UnActiveSkillBox());
    }

    private IEnumerator UnActiveSkillBox()
    {
        yield return new WaitForSeconds(duration);
        magicSpell.SetActive(false);
    }


    public override void Move()
    {
        int index = 0;
        Vector3 HunterPosition=Hunter.HunterPosition;
        float dir=0;

        for (int i = 0; i < moveDirection.Length; i++)
        {
            float temp = Mathf.Abs(HunterPosition.x - (moveDirection[i].x+transform.position.x))+
                  Mathf.Abs(HunterPosition.z - (moveDirection[i].z + transform.position.z));

            if (dir < temp)
            {
                dir = temp;
                index = i;
            }
        }
        movePoint[0] = moveDirection[index]+transform.position;

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

   

    public override float GetHP()
    {
        return Health;
    }

    public override float GetMaxHp()
    {
        return MaxHealth;
    }
}