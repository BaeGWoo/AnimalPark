using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public float jumpHeight = 2f; // 점프 높이
    public float jumpDuration = 1f; // 점프 애니메이션의 지속 시간
    public float molePoint = 0.5f;
    public bool moveable = false;
    protected AIManager aiManager;

   public virtual void AnimalAct() { }
    public virtual void ActiveBombCollider() { }

    
    public virtual void SetAttackAble(bool check) { }
    public virtual void Move() { }

    public virtual void Skill() { }
    public virtual void JumpAnimaition() { }
        
    public virtual void Attack() {  }

    public virtual void Damaged() { aiManager.UpdateAnimalList(); }

    public virtual float GetHP() { return 0; }
    public virtual float GetMaxHp() { return 0; }

    public virtual bool GetAttackAble() { return true; }

    public  virtual float AnimalDamage() { return 0; }

    public virtual void SetAnimalStatus(float attack, float health, int skillCount) { }


    public virtual void ColobusAttack() { }
    public virtual void ActiveAttackBox() { }
    public virtual void UnActiveAttackBox() { }

    private void Awake()
    {
        aiManager = FindObjectOfType<AIManager>();
    }

    public void AnimalAct(int skillcount, bool attackAble, bool moveAble)
    {
        if (skillcount == 0)
        {
            Debug.Log(gameObject.name + "Skill");
            Skill();
        }
        else if (attackAble)
        {
            Debug.Log(gameObject.name + "Attack");
            Attack();
        }

        else if (moveAble)
        {
            Debug.Log(gameObject.name + "Move");
            Move();
        }

       
    }

    public void Move(Vector3 curPosition,Quaternion curRotation, Vector3[] movePoint)
    {
        FindAnyObjectByType<TileManager>().GetComponent<TileManager>().insertTileMap((int)(curPosition.x / 2), (int)(curPosition.z / 2),0);
        Vector3 target = Hunter.HunterPosition;
        float distance = 20;
        int minDirection = -1;

        // 이동할 위치 중 target과 가장 인접한 위치 찾기
        for (int i = 0; i < movePoint.Length; i++)
        {
            float temp;
            temp = Mathf.Abs(movePoint[i].x - target.x) + Mathf.Abs(movePoint[i].z - target.z);
            if (movePoint[i].x >= 0 && movePoint[i].x <= 14 && movePoint[i].z >= 0 && movePoint[i].z <= 14)
            {
                if (FindAnyObjectByType<TileManager>().GetComponent<TileManager>().CheckTileMap((int)(curPosition.x / 2), (int)(curPosition.z / 2)))
                {
                    if (temp <= distance)
                    {
                        distance = temp;
                        minDirection = i;
                    }
                }
            }
        }
        FindAnyObjectByType<TileManager>().GetComponent<TileManager>().insertTileMap(
            ((int)movePoint[minDirection].x / 2), (int)(movePoint[minDirection].z) / 2, 1);
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
           float height = Mathf.Sin(t * Mathf.PI) * jumpHeight; // 점프 곡선
      
           Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
           transform.position = new Vector3(currentPosition.x, startPosition.y + height, currentPosition.z);
      
           elapsedTime += Time.deltaTime;
           yield return null; // 다음 프레임까지 대기
       }
      
        transform.position = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);
        
        transform.LookAt(Hunter.HunterPosition);


        float targetYRotation = (int)(transform.rotation.eulerAngles.y/90)*90.0f;
        transform.rotation = Quaternion.Euler(0, targetYRotation%360, 0);



        Vector3 eulerAngles = curRotation.eulerAngles;
    }


    // 공격 모션인 ParticleSystem 켜기
    public void Attack(GameObject[] attackMotion, float duration)
    {

        for (int i = 0; i < attackMotion.Length; i++)
        {
            attackMotion[i].SetActive(true);
            StartCoroutine(DeactivateAfterDuration(attackMotion, duration));
        }      
    }

   
    // 공격 모션인 ParticleSystem 끄기
    private IEnumerator DeactivateAfterDuration(GameObject[] attackMotion, float duration)
    {
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < attackMotion.Length; i++)
        {
            attackMotion[i].SetActive(false);
        }
        //UnActiveAttackBox();   
    }

    public void Die()
    { 
        FindAnyObjectByType<TileManager>().GetComponent<TileManager>().insertTileMap((int)transform.position.x / 2, (int)transform.position.z / 2, 0);
        aiManager.RemoveAnimal(gameObject);
        aiManager.UpdateAnimalList();
        Destroy(gameObject, 1.0f);

    }


}
