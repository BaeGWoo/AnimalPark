using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taipan : Animal
{
    private Vector3[] movePoint = new Vector3[5];
    private Vector3[] moveDirection = new Vector3[4];
    private Animator animator;
    [SerializeField] GameObject AttackBox;
    [SerializeField] ParticleSystem AttackMotion;
    [SerializeField] float duration = 2.0f;

    private void Awake()
    {
        moveDirection[0] = new Vector3(4f, 0, 4);
        moveDirection[1] = new Vector3(-4, 0, 4);
        moveDirection[2] = new Vector3(-4, 0, -4);
        moveDirection[3] = new Vector3(4, 0, -4);
       

        animator = GetComponent<Animator>();
        if (AttackMotion != null)
        {
            // 파티클을 비활성화 상태로 설정
            AttackMotion.Stop();
        }
    }

    public override void Move()
    {
        movePoint[0] = transform.position;

        for (int i = 1; i < movePoint.Length; i++)
        {
            movePoint[i] = new Vector3(moveDirection[i - 1].x + transform.position.x, 0, moveDirection[i - 1].z + transform.position.z);
        }
        base.Move(transform.position, transform.rotation, movePoint, moveDirection);
    }
    public override void JumpAnimaition(){animator.SetTrigger("Jump");}




    public override void ActiveAttackBox()
    {
        AttackBox.SetActive(true);
    }

    public override void Attack()
    {
        //  AttackBox.SetActive(true);
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration, AttackBox);
    }
}
