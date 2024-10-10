                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herring : Animal
{
    private Vector3[] movePoint = new Vector3[9];
    private Vector3[] moveDirection = new Vector3[8];
    private Animator animator;
    [SerializeField] GameObject AttackBox;

    private void Awake()
    {
        moveDirection[0] = new Vector3(2, 0, 2);
        moveDirection[1] = new Vector3(-2, 0, 2);
        moveDirection[2] = new Vector3(-2, 0, -2);
        moveDirection[3] = new Vector3(2, 0, -2);
        moveDirection[4] = new Vector3(0, 0, -2);
        moveDirection[5] = new Vector3(0, 0, 2);
        moveDirection[6] = new Vector3(2, 0, 0);
        moveDirection[7] = new Vector3(-2, 0, 0);


       

        animator = GetComponent<Animator>();
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

    public override void JumpAnimaition()
    {
        animator.SetTrigger("Jump");
    }



    public void Attack()
    {
        StartCoroutine(ActiveAttackBox());
    }

    private IEnumerator ActiveAttackBox()
    {
        animator.SetTrigger("Attack");

        // GameObject를 활성화합니다.
        AttackBox.SetActive(true);

        // 지정한 시간 동안 대기합니다.
        yield return new WaitForSeconds(1.0f);

        // GameObject를 비활성화합니다.
        AttackBox.SetActive(false);
    }
}
