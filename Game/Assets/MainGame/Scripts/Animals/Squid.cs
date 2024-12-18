using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Squid : Animal
{
    private Vector3[] movePoint = new Vector3[4];
    private Vector3[] moveDirection = new Vector3[4];
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 2.0f;
    [SerializeField] float Health = 3;
    private float MaxHealth = 3;
    public float AttackDamage = 0;
    private Animator animator;
    public bool attackable = false;
    public bool hitable = false;
    private AudioSource audioSource;

    private void Awake()
    {
        moveDirection[0] = new Vector3(0, 0, 4);
        moveDirection[1] = new Vector3(0, 0, -4);
        moveDirection[2] = new Vector3(-4, 0, 0);
        moveDirection[3] = new Vector3(4, 0, 0);

        

        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }


    public override void SetAnimalStatus(float Attack, float Health)
    {
        MaxHealth = Health;
        AttackDamage = Attack;
    }

    public override float AnimalDamage()
    {
       
        return AttackDamage;
    }

    public override void Move()
    {
        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = moveDirection[i] + transform.position;
        }

        base.Move(transform.position,transform.rotation,movePoint);
    }

    public override void JumpAnimaition(){animator.SetTrigger("Jump"); audioSource.clip = Resources.Load<AudioClip>("Sounds/Jump");
        audioSource.Play();
    }

    
    public override void Attack()
    {
        animator.SetTrigger("Attack");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/" + gameObject.name + "Attack");
        audioSource.Play();
        Attack(AttackMotion, duration);
    }

    public override void ActiveAttackBox() {
        attackable = true;
        AttackBox.SetActive(true);
    }

    public override void UnActiveAttackBox()
    {
        attackable = false;
        AttackBox.SetActive(false);
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
