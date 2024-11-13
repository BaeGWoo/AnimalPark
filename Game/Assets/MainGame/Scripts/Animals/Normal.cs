using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal : Animal
{
    private Vector3[] movePoint = new Vector3[8];
    private Vector3[] moveDirection = new Vector3[8];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 3.5f;
    [SerializeField] float Health = 2;
    private float MaxHealth=2;
    public float AttackDamage = 0;
    public bool attackable = false;
    public bool hitable = false;
    private AudioSource audioSource;


    private void Awake()
    {
        moveDirection[0] = new Vector3(2, 0, 0);
        moveDirection[1] = new Vector3(-2, 0, 0);
        moveDirection[2] = new Vector3(0, 0, -2);
        moveDirection[3] = new Vector3(0, 0, 2);

        moveDirection[4] = new Vector3(2, 0, 2);
        moveDirection[5] = new Vector3(-2, 0, 2);
        moveDirection[6] = new Vector3(2, 0, -2);
        moveDirection[7] = new Vector3(-2, 0, -2);


        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void SetAnimalStatus(float Attack, float Health)
    {
        MaxHealth = Health;
        AttackDamage = Attack;
    }

    public override float AnimalDamage() { return AttackDamage; }


    public override void Move()
    {
        movePoint[0] = transform.position;

        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = new Vector3(moveDirection[i].x + transform.position.x, 0, moveDirection[i].z + transform.position.z);
        }
        base.Move(transform.position, transform.rotation, movePoint);
    }
    public override void JumpAnimaition() { animator.SetTrigger("Jump"); audioSource.clip = Resources.Load<AudioClip>("Sounds/Jump");
        audioSource.Play();
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
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/" + gameObject.name + "Attack");
        audioSource.Play();
        Attack(AttackMotion, duration);
    }

    public override void Damaged()
    {
        Health--;
        animator.SetTrigger("Damage");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/Damage");
        audioSource.Play();

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

    public override float GetMaxHp()
    {
        return MaxHealth;
    }

public override bool GetAttackAble()
    {
        return attackable;
    }

   
}
