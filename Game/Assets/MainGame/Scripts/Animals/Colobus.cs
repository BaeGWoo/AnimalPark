using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Colobus :Animal
{
    private Vector3[] movePoint= new Vector3[4];
    private Vector3[] moveDirection = new Vector3[4];
    private Animator animator;
    [SerializeField] float duration = 2.0f;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject[] AttackMotion;
    [SerializeField] GameObject Banana;
    

    [SerializeField] float Health = 3;
    private float MaxHealth = 3;
    private Vector3 bananaPosition;
    public bool attackable = false;
    public bool hitable = false;
    public float AttackDamage = 0;
    

    private void Awake()
    {
        moveDirection[0] = new Vector3(-2, 0, 0);
        moveDirection[1] = new Vector3(2, 0, 0);
        moveDirection[2] = new Vector3(0, 0, -2);
        moveDirection[3] = new Vector3(0, 0, 2);

      
        bananaPosition=Banana.transform.position;

        animator = GetComponent<Animator>();
        aiManager = FindObjectOfType<AIManager>();
    }

    public override void SetAnimalStatus(float Attack, float Health)
    {
        MaxHealth= Health;
        AttackDamage= Attack;
    }

    public override float  AnimalDamage() { return AttackDamage; }

    public override void Move()
    {
        movePoint[0] = transform.position;

        for (int i = 1; i < movePoint.Length; i++)
        {
            movePoint[i] = new Vector3(moveDirection[i - 1].x + transform.position.x, 0, moveDirection[i - 1].z + transform.position.z);
        }
        base.Move(transform.position, transform.rotation, movePoint);
    }

    public override void JumpAnimaition(){animator.SetTrigger("Jump");}

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
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration);
    }

    public override void ColobusAttack()
    {
        Banana.transform.position = gameObject.transform.position;
        Banana.SetActive(true);
        StartCoroutine(MoveTowardsTarget());
    }

    private IEnumerator MoveTowardsTarget()
    {
        Vector3 Target = Hunter.HunterPosition;

        while (Vector3.Distance(Banana.transform.position, Target) > 0.1f)
        {
            Vector3 cur = Banana.transform.position;

            Banana.transform.position = Vector3.MoveTowards(cur, Target, 5.0f * Time.deltaTime);
          
            // 매 프레임마다 한 번씩 이동
            yield return null;
        }
        
    }

    public override void Damaged()
    {
        Health--;
        animator.SetTrigger("Damage");
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
