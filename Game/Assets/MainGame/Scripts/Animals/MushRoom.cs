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


    private Animator animator;
    private AudioSource audioSource;
    private Animation animationComponent;

    private void Awake()
    {
        bombPosition[0] = new Vector3(2, 0, 0);
        bombPosition[1] = new Vector3(-2, 0, 0);
        bombPosition[2] = new Vector3(-2, 0, 2);
        bombPosition[3] = new Vector3(0, 0, -2);
        bombPosition[4] = new Vector3(2, 0, -2);

       
        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
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

        base.AnimalAct(skillCount, attackable, false);
    }

    public override void Skill()
    {
        GameObject prefab;
        int randomNumber = Random.Range(0, 5);
        animationComponent.Play("BombAttack");
        prefab = Instantiate(BombPrefab);
        prefab.name = BombPrefab.name;
        prefab.GetComponent<Monster>().SetInitialSetting();
        Vector3 targetPosition = transform.position + bombPosition[randomNumber];
        StartCoroutine(JumpToPosition(prefab, targetPosition));
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

    IEnumerator JumpToPosition(GameObject prefab, Vector3 targetPosition)
    {
        base.JumpAnimaition();
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight; // Á¡ÇÁ °î¼±

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            prefab.transform.position = new Vector3(currentPosition.x, startPosition.y + height, currentPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        prefab.transform.position = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);
    }
    


    public override void Damaged(float dmg)
    {
        Health -= dmg;
        base.Damaged(dmg);
        if (Health <= 0) base.Die();
    }

 

  
}
