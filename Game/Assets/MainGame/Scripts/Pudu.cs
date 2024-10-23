using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Pudu : Animal
{
    private Vector3[] movePoint = new Vector3[3];
    private Vector3[] moveDirection = new Vector3[2];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] GameObject AttackMotion;
    [SerializeField] float duration = 2.0f;
    [SerializeField] float Health = 3;

  
    private void Awake()
    {
        animator = GetComponent<Animator>();

        Vector3 target = Hunter.HunterPosition;

        moveDirection[0] = new Vector3(transform.position.x, 0, target.z);
        moveDirection[1] = new Vector3(target.x, 0, transform.position.z);
        aiManager = FindObjectOfType<AIManager>();

    }

    public override void Move()
    {
        movePoint[0] = transform.position;

        movePoint[1] = new Vector3(Hunter.HunterPosition.x, 0, transform.position.z);
        movePoint[2] = new Vector3(transform.position.x, 0, Hunter.HunterPosition.z);
        base.Move(transform.position, transform.rotation, movePoint);
    }

    public override void JumpAnimaition(){animator.SetTrigger("Jump");}



    public override void ActiveAttackBox()
    {
        AttackBox.SetActive(true);
    }

    public override void UnActiveAttackBox()
    {
        AttackBox.SetActive(false);
    }

    public override void Attack()
    {
        //  AttackBox.SetActive(true);
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration);
    }

    public override void Damaged()
    {
        Health--;
        if (Health <= 0)
        {
            aiManager.RemoveAnimal(gameObject);
            animator.SetTrigger("Die");
            base.Die();
        }
    }

    public override float GetHP()
    {
        return Health;
    }
}
