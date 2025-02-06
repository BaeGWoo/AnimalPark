using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : Monster
{
    private Vector3[] movePoint = new Vector3[1];
    private Vector3[] moveDirection = new Vector3[4];
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] GameObject magicSpell;

    [SerializeField] float taipanCount=3;
    [SerializeField] GameObject[] Taipans;
  
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
        taipanCount--;
        if (taipanCount <= 0) 
        {
            for (int i = 0; i < Taipans.Length; i++) 
            {
                Taipans[i].GetComponent<Taipan>().ActiveAttackBox();
            }
            taipanCount = 2;
        }
        if (skillCount < 0) { skillCount = totalSkillCount-1; }
        base.AnimalAct(skillCount, false, true);
    }

    public override void Skill()
    {
        magicSpell.SetActive(true);
        StartCoroutine(UnActiveSkillBox());
        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().GetMoveDebuff(6);
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


            if (dir < temp&&
                (moveDirection[i].x + transform.position.x)>=0 && (moveDirection[i].x + transform.position.x) <= 14&&
                (moveDirection[i].z + transform.position.z) >= 0 && (moveDirection[i].z + transform.position.z) <= 14
                )
            {
                dir = temp;
                index = i;
            }
        }
        movePoint[0] = moveDirection[index]+transform.position;

        //animationComponent.Play("Jump");
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
