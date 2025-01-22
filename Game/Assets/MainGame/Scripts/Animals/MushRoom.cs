using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushRoom : Monster
{
    [SerializeField] float Health;
    [SerializeField] int skillCount;
    [SerializeField] GameObject BombPrefab;


    public float duration = 3.5f;
    private float MaxHealth;
    public int totalSkillCount;
    public float AttackDamage;
    public bool attackable = false;

    private Vector3[] bombPosition = new Vector3[5];
    private Animation animationComponent;

    private void Awake()
    {
        bombPosition[0] = new Vector3(2, 0, 0);
        bombPosition[1] = new Vector3(-2, 0, 0);
        bombPosition[2] = new Vector3(-2, 0, 2);
        bombPosition[3] = new Vector3(0, 0, -2);
        bombPosition[4] = new Vector3(2, 0, -2);

       
        aiManager = FindObjectOfType<AIManager>();
        animationComponent = GetComponent<Animation>();
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

        base.AnimalAct(skillCount, false, false);
    }

    public override void Skill()
    {
        GameObject prefab;
        animationComponent.Play("BombAttack");
        prefab = Instantiate(BombPrefab);
        prefab.name = BombPrefab.name;
        prefab.transform.position = transform.position;
        prefab.GetComponent<Monster>().SetInitialSetting();
        //prefab.GetComponent<Monster>().AnimalAct();
        FindAnyObjectByType<AIManager>().GetComponent<AIManager>().AddAnimal(prefab);
    }

    public override float GetHP()
    {
        return Health;
    }

    public override float GetMaxHp()
    {
        return MaxHealth;
    }

  
    


    public override void Damaged(float dmg)
    {
        Health -= dmg;
        animationComponent.Play("Damage");
        base.Damaged(dmg);
        if (Health <= 0) base.Die();
    }

 

  
}
