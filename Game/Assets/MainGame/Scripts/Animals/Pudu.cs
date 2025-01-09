using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Pudu : Animal
{
    private Vector3[] movePoint = new Vector3[2];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] float duration = 2.0f;
    [SerializeField] float Health = 3;
    private float MaxHealth = 3;
    public float AttackDamage = 0;
    public bool attackable = false;
    public bool hitable = false;
    private AudioSource audioSource;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        Vector3 target = Hunter.HunterPosition;
        aiManager = FindObjectOfType<AIManager>();
        audioSource=gameObject.AddComponent<AudioSource>();
    }

   

    public override float AnimalDamage() { return AttackDamage; }


    public override void Move()
    {
        movePoint[0] = new Vector3(Hunter.HunterPosition.x, 0, transform.position.z);
        movePoint[1] = new Vector3(transform.position.x, 0, Hunter.HunterPosition.z);
        base.Move(transform.position, transform.rotation, movePoint);
    }

    public override void JumpAnimaition(){animator.SetTrigger("Jump"); audioSource.clip = Resources.Load<AudioClip>("Sounds/Jump");
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
        audioSource.clip= Resources.Load<AudioClip>("Sounds/AnimalAttack/"+gameObject.name+"Attack");
        audioSource.Play();
        animator.SetTrigger("Attack");
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
