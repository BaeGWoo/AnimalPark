using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public float jumpHeight = 2f; // ���� ����
    public float jumpDuration = 1f; // ���� �ִϸ��̼��� ���� �ð�
    public float molePoint = 0.5f;
    public bool moveable = false;
    protected AIManager aiManager;

    public virtual void Move() { }
    public virtual void JumpAnimaition() { }

        
    public virtual void Attack() {  }

    public virtual void Damaged() { aiManager.UpdateAnimalHp(); }

    public virtual float GetHP() { return 0; }
    public virtual float GetMaxHp() { return 0; }

    public virtual bool GetAttackAble() { return true; }

    public  virtual void SparrowAttack() { }

    public virtual void ColobusAttack() { }
    public virtual void ActiveAttackBox() { }
    public virtual void UnActiveAttackBox() { }

    private void Awake()
    {
        aiManager = FindObjectOfType<AIManager>();
    }

    public void Move(Vector3 curPosition,Quaternion curRotation, Vector3[] movePoint)
    {
        moveable = true;
        AIManager.TileMap[(int)(curPosition.x / 2), (int)(curPosition.z / 2)] = 0;
        Vector3 target = Hunter.HunterPosition;
        

        float distance = 100;
        int minDirection = 0;

      

        // �̵��� ��ġ �� target�� ���� ������ ��ġ ã��
        for (int i = 0; i < movePoint.Length; i++)
        {
            float temp;
            temp = Mathf.Abs((movePoint[i].x - target.x) + (movePoint[i].z - target.z));

            if (movePoint[i].x >= -0.01f && movePoint[i].x <= 14 && movePoint[i].z >= -0.01f && movePoint[i].z <= 14)
            {
                if (AIManager.TileMap[(int)(movePoint[i].x / 2), (int)(movePoint[i].z / 2)] != 1)
                {
                    if (temp <= distance)
                    {
                        distance = temp;
                        minDirection = i;
                    }
                }
            }
        }
        AIManager.TileMap[(int)(movePoint[minDirection].x / 2), (int)(movePoint[minDirection].z) / 2] = 1;
        StartCoroutine(JumpToPosition(curPosition, curRotation, new Vector3(movePoint[minDirection].x, 0, movePoint[minDirection].z)));
    }

    IEnumerator JumpToPosition(Vector3 curPosition,Quaternion curRotation, Vector3 targetPosition)
    {
        JumpAnimaition();
        Vector3 startPosition = curPosition;
        float elapsedTime = 0f;

       while (elapsedTime < jumpDuration)
       {
           float t = elapsedTime / jumpDuration;
           float height = Mathf.Sin(t * Mathf.PI) * jumpHeight; // ���� �
      
           Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
           transform.position = new Vector3(currentPosition.x, startPosition.y + height, currentPosition.z);
      
           elapsedTime += Time.deltaTime;
           yield return null; // ���� �����ӱ��� ���
       }
      
        transform.position = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);
        
        transform.LookAt(Hunter.HunterPosition);


        float targetYRotation = transform.rotation.eulerAngles.y;

        // ���ο� ȸ���� ���� (x, z�� 0���� ����, y�� 45�� ����� ����)
        transform.rotation = Quaternion.Euler(0, targetYRotation%360, 0);



        Vector3 eulerAngles = curRotation.eulerAngles;
    }


    // ���� ����� ParticleSystem �ѱ�
    public void Attack(GameObject[] attackMotion, float duration)
    {

        for (int i = 0; i < attackMotion.Length; i++)
        {
            attackMotion[i].SetActive(true);
            StartCoroutine(DeactivateAfterDuration(attackMotion, duration));
        }

       
        if (gameObject.name == "Sparrow")
        {
            SparrowAttack();
        }

        if (gameObject.name == "Colobus")
        {
            ColobusAttack();
        }
    }

   
    // ���� ����� ParticleSystem ����
    private IEnumerator DeactivateAfterDuration(GameObject[] attackMotion, float duration)
    {
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < attackMotion.Length; i++)
        {
            attackMotion[i].SetActive(false);
        }
        UnActiveAttackBox();

       
    }

    public void Die()
    { 
        Destroy(gameObject, 1.0f);
        aiManager.UpdateAnimalList();
        if (aiManager.GetAnimalsCount() <= 0)
        {
            aiManager.ShowNext();

        }
    }
}
