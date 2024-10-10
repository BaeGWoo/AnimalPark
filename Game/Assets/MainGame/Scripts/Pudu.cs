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

    private void Awake()
    {
        animator = GetComponent<Animator>();

        Vector3 target = Hunter.HunterPosition;

        moveDirection[0] = new Vector3(transform.position.x, 0, target.z);
        moveDirection[1] = new Vector3(target.x, 0, transform.position.z);
        
    }

    public override void Move()
    {
        movePoint[0] = transform.position;

        movePoint[1] = new Vector3(Hunter.HunterPosition.x, 0, transform.position.z);
        movePoint[2] = new Vector3(transform.position.x, 0, Hunter.HunterPosition.z);
        base.Move(transform.position, transform.rotation, movePoint, moveDirection);
    }

    public override void JumpAnimaition(){animator.SetTrigger("Jump");}



    public void Attack()
    {
        
    }

    private IEnumerator ActiveAttackBox()
    {
        animator.SetTrigger("Jump");

        // GameObject를 활성화합니다.
        AttackBox.SetActive(true);

        // 지정한 시간 동안 대기합니다.
        yield return new WaitForSeconds(1.0f);

        // GameObject를 비활성화합니다.
        AttackBox.SetActive(false);
    }
}
