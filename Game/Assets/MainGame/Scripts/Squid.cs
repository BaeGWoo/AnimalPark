using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Squid : Animal
{
    private Vector3[] movePoint = new Vector3[9];
    private Vector3[] moveDirection = new Vector3[8];
    [SerializeField] GameObject AttackBox;
    [SerializeField] ParticleSystem AttackMotion;
    [SerializeField] float duration = 2.0f;
    private Animator animator;

    private void Awake()
    {
        moveDirection[0] = new Vector3(4, 0, 8);
        moveDirection[1] = new Vector3(-4, 0, 8);
        moveDirection[2] = new Vector3(-4, 0, -8);
        moveDirection[3] = new Vector3(4, 0, -8);
        moveDirection[4] = new Vector3(8, 0, 4);
        moveDirection[5] = new Vector3(8, 0, -4);
        moveDirection[6] = new Vector3(-8, 0, -4);
        moveDirection[7] = new Vector3(-8, 0, -4);

        

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

        base.Move(transform.position,transform.rotation,movePoint, moveDirection);
    }

    public override void JumpAnimaition(){animator.SetTrigger("Jump");}

    
    public void Attack()
    {
        animator.SetTrigger("Attack");
        Attack(AttackMotion, duration, AttackBox);
    }

    public override void ActiveAttackBox() {
        AttackBox.SetActive(true);
    }
}
