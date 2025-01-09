using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Animal
{
    private Vector3[] movePoint = new Vector3[8];
    private Vector3[] moveDirection = new Vector3[8];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health;
    public int totalSkillCount = -1;
    private float MaxHealth;
    public float AttackDamage = 0;
    public bool attackable = false;
    public bool hitable = false;
    private AudioSource audioSource;
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

        totalSkillCount = skillCount;
    }

    public override float AnimalDamage() { return AttackDamage; }


    public override void Move()
    {
        //movePoint[0] = transform.position;
        AttackBox.SetActive(true);
        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = new Vector3(moveDirection[i].x + transform.position.x, 0, moveDirection[i].z + transform.position.z);
        }
        base.Move(transform.position, transform.rotation, movePoint);
    }
    public override void AnimalAct()
    {
        base.AnimalAct(-1, false,true);
       
    }

    public override void JumpAnimaition()
    {
        //animationComponent.Play("Jump");
        animationComponent.Play("Run");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/Jump");
        audioSource.Play();
    }



    public override  void ActiveBombCollider()
    {
        AttackBox.SetActive(true);
    }
    public override void ActiveAttackBox()
    {
        attackable = true;
        AttackBox.SetActive(true);
    }

    public override void Attack()
    {
        AttackMotion[0].SetActive(true);
        FindAnyObjectByType<AIManager>().GetComponent<AIManager>().RemoveAnimal(gameObject);
        FindAnyObjectByType<AIManager>().GetComponent<AIManager>().UpdateAnimalList();
        Destroy(gameObject,0.5f);
    }


    public override void UnActiveAttackBox()
    {
        attackable = false;
        AttackBox.SetActive(false);
    }

    public override void Damaged()
    {
        Health--;
        animationComponent.Play("Damage");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/Damage");
        audioSource.Play();
        base.Damaged();
        base.Die();
        //FindAnyObjectByType<AIManager>().GetComponent<AIManager>().RemoveAnimal(gameObject);
        //FindAnyObjectByType<AIManager>().GetComponent<AIManager>().UpdateAnimalList();
        animationComponent.Play("Death");
            
    }

    public override float GetHP()
    {
        return Health;
    }

    public override float GetMaxHp()
    {
        return MaxHealth;
    }

    public override bool GetAttackAble()
    {
        return attackable;
    }

    

}
