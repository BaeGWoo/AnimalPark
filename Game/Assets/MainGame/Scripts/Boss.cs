using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Monster
{
    private Vector3[] movePoint = new Vector3[12];
    private Vector3[] moveDirection = new Vector3[12];
    private Vector3[] attackBox = new Vector3[8];
    [SerializeField] Vector3 curAttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] GameObject PosionBomb;
    [SerializeField] float duration = 2.0f;
    [SerializeField] float Health = 3;
    [SerializeField] float MaxHealth = 3;
    public float AttackDamage = 0;

    public bool attackable = false;
    public bool hitable = false;
    [SerializeField] int posionCount = 2;
    [SerializeField] int posionStep = 0;
    [SerializeField] GameObject posionBomb;
    [SerializeField] Vector3[] posionPosition;
    GameObject[] TempPosionBomb;


    [SerializeField] int magicCount = 4;
    [SerializeField] int magicStep = 0;
    [SerializeField] GameObject magicSheld;
    [SerializeField] GameObject magic;
    [SerializeField] GameObject skill;

    [SerializeField] int totalPosionCount = 2;
    [SerializeField] int totalMagicCount = 4;


    private void Awake()
    {
        MaxHealth = 7;
        Health = MaxHealth;

        posionPosition[0] = new Vector3(2, 0, 0);
        posionPosition[1] = new Vector3(0, 0, 2);
        posionPosition[2] = new Vector3(-2, 0, 0);
        posionPosition[3] = new Vector3(0, 0, -2);


        attackBox[0] = new Vector3(2, 0, 0);
        attackBox[1] = new Vector3(0, 0, 2);
        attackBox[2] = new Vector3(-2, 0, 0);
        attackBox[3] = new Vector3(0, 0, -2);

        attackBox[4] = new Vector3(2, 0, 2);
        attackBox[5] = new Vector3(2, 0, -2);
        attackBox[6] = new Vector3(-2, 0, 2);
        attackBox[7] = new Vector3(-2, 0, -2);
    }


    public override void AnimalAct()
    {
        if (magicStep == 0)
        {
            if (posionCount == 0)
            {
                PosionAttack();
            }
            if (magicCount > 0)
            {
                magicCount--;
            }
        }

        else
            MagicAttack();


        if (posionStep == 0)
        {
            if (magicCount == 0)
            {
                MagicAttack();
            }
            if (posionCount > 0)
                posionCount--;
        }

        else
            PosionAttack();


        if (posionCount < 0)
            posionCount = totalPosionCount;
        if (magicCount < 0)
            magicCount = totalMagicCount;
       

        base.AnimalAct(-1, attackable, false);
    }


   

    public override void Attack()
    {
        base.Attack();
        Vector3 dir = transform.position;

        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().BossAttack(transform.position);
    }


    public void PosionAttack()
    {
        switch (posionStep)
        {
            case 0:
                posionStep++;
                CreatePosionBomb();
                break;
                
            case 1:
                posionStep = 0;
                GetComponent<Animator>().SetTrigger("Skill");
                ShootPosionBomb();
                break;
        }
    }

    public void CreatePosionBomb()
    {
        for(int i = 0; i < posionPosition.Length; i++)
        {
            TempPosionBomb[i] = Instantiate(posionBomb);
            TempPosionBomb[i].transform.position = posionPosition[i] + transform.position;
            TempPosionBomb[i].name = "PosionBomb";
        }
    }

    public void ShootPosionBomb()
    {
        for (int i = 0; i < TempPosionBomb.Length; i++)
        {
            TempPosionBomb[i].GetComponent<PosionBomb>().ShootPosionBomb(transform.position);
        }
    }


    public void MagicAttack()
    {
        switch (magicStep)
        {
            case 0:
                magicStep++;
                transform.GetComponent<BoxCollider>().enabled = false;
                magicSheld.SetActive(true);
                break;

            case 1:
                magicStep++;
                magicSheld.SetActive(false); 
                transform.GetComponent<BoxCollider>().enabled = true;
                magic.SetActive(true);
                StartCoroutine(UnActiveSkillBox());
                break;

            case 2:
                magicStep = 0;
                skill.SetActive(true);
                break;

        }
    }

    private IEnumerator UnActiveSkillBox()
    {
        yield return new WaitForSeconds(duration);
        magic.SetActive(false);
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



    public override void Damaged(float dmg)
    {
        Health -= dmg;

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
