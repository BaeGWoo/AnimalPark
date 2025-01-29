using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] bool ice;
    [SerializeField] GameObject IceObject;
    public float jumpHeight = 2f; // 점프 높이
    public float jumpDuration = 1f; // 점프 애니메이션의 지속 시간
    [SerializeField] Animator animator;
    private AudioSource audioSource;

    public TileManager tileManager;
    public AIManager aiManager;
    public virtual void AnimalAct() { }

    public virtual void Skill() { }
    public virtual void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/" + gameObject.name + "Attack");
        audioSource.Play();
    }
    public virtual void Move() { }
    public virtual void Damaged(float dmg) 
    {
        if (animator != null)
            animator.SetTrigger("Damage");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/AnimalAttack/Damage");
        audioSource.Play();
       aiManager.UpdateAnimalList();
    }

   


    public virtual float GetHP() { return 0; }
    public virtual float GetMaxHp() { return 0; }
    public virtual bool GetAttackAble() { return true; }
    public virtual void SetAttackAble(bool attackable, Vector3 curAttackBox) { }

    public virtual void SetAnimalStatus(float attack, float health, int skillCount) { }



    private void Start()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        aiManager = FindAnyObjectByType<AIManager>().GetComponent<AIManager>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void SetInitialSetting()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        aiManager = FindAnyObjectByType<AIManager>().GetComponent<AIManager>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }



    public void AnimalAct(int skillcount, bool attackAble, bool moveAble)
    {
        if (ice)
            return;
        transform.LookAt(Hunter.HunterPosition);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y % 360, 0);
        if (skillcount == 0)
        {
            Skill();
        }

        else if (attackAble)
        {
            Attack();
        }

        else if (moveAble)
        {
            Move();
        }
    }

    public void Move(Vector3 curPosition, Vector3[] movePoint)
    {
        int movePositionX = (int)curPosition.x / 2;
        int movePositionZ = (int) curPosition.z / 2;

        tileManager.insertTileMap((int)(transform.position.x/2), (int)(transform.position.z/2), 0);
        Vector3 target = FindAnyObjectByType<Hunter>().GetComponent<Hunter>().GetHunterPosition();

        float distance = 20;
        int minDirection = -1;

        // 이동할 위치 중 target과 가장 인접한 위치 찾기
        for (int i = 0; i < movePoint.Length; i++)
        {
            float temp;
            temp = Mathf.Abs(movePoint[i].x - target.x) + Mathf.Abs(movePoint[i].z - target.z);
            if (movePoint[i].x >= 0 && movePoint[i].x <= 14 && movePoint[i].z >= 0 && movePoint[i].z <= 14)
            {
                if (tileManager.CheckTileMap((int)(movePoint[i].x/2), (int)(movePoint[i].z/2)))
                {
                    if (temp <= distance)
                    {
                        distance = temp;
                        minDirection = i;
                    }
                }
            }
        }
        if (minDirection >= 0)
        {
            //Debug.Log(gameObject.name + " : ( " + movePoint[minDirection].x / 2 + " , " + movePoint[minDirection].z / 2 + " ) ");
            tileManager.insertTileMap(
                ((int)(movePoint[minDirection].x / 2)), (int)(movePoint[minDirection].z / 2), 1);
            StartCoroutine(JumpToPosition(curPosition, new Vector3(movePoint[minDirection].x, 0, movePoint[minDirection].z)));
        }
        else if(minDirection==-1)
        {
            tileManager.insertTileMap(
                ((int)(transform.position.x / 2)), (int)(transform.position.z / 2), 1);
            StartCoroutine(JumpToPosition(curPosition, new Vector3(transform.position.x, 0, transform.position.z)));
        }
    }

    public void Move(Vector3 curPosition, Vector3[] AttackBox, Vector3[] movePoint)
    {
        tileManager.insertTileMap((int)(transform.position.x / 2), (int)(transform.position.z / 2), 0);
        Vector3 target = FindAnyObjectByType<Hunter>().GetComponent<Hunter>().GetHunterPosition();

        float distance = 20;
        int minDirection = -1;
        // 이동할 위치 중 target과 가장 인접한 위치 찾기
        for (int i = 0; i < movePoint.Length; i++)
        {

            bool tempattack = false;
            float temp;
            temp = Mathf.Abs(movePoint[i].x - target.x) + Mathf.Abs(movePoint[i].z - target.z);
            if (movePoint[i].x >= 0 && movePoint[i].x <= 14 && movePoint[i].z >= 0 && movePoint[i].z <= 14)
            {
                if (tileManager.CheckTileMap((int)(movePoint[i].x / 2), (int)(movePoint[i].z / 2)))
                {
                    if (temp <= distance)
                    {
                        distance = temp;
                        minDirection = i;
                    }
                }
            }

            for (int j = 0; j < AttackBox.Length; j++)
            {
                if (movePoint[i].x + AttackBox[j].x == Hunter.HunterPosition.x && movePoint[i].z==Hunter.HunterPosition.z)
                {
                    tempattack = true;
                    minDirection = i;
                }

            }

            if (tempattack)
                break;
            
        }
        if (minDirection >= 0)
        {
            //Debug.Log(gameObject.name + " : ( " + movePoint[minDirection].x / 2 + " , " + movePoint[minDirection].z / 2 + " ) ");
            tileManager.insertTileMap(
                ((int)(movePoint[minDirection].x / 2)), (int)(movePoint[minDirection].z / 2), 1);
            StartCoroutine(JumpToPosition(curPosition, new Vector3(movePoint[minDirection].x, 0, movePoint[minDirection].z)));
        }
        else if (minDirection == -1)
        {
            tileManager.insertTileMap(
                ((int)(transform.position.x / 2)), (int)(transform.position.z / 2), 1);
            StartCoroutine(JumpToPosition(curPosition, new Vector3(transform.position.x, 0, transform.position.z)));
        }
    }



    IEnumerator JumpToPosition(Vector3 curPosition, Vector3 targetPosition)
    {
        if (animator != null)
            animator.SetTrigger("Jump");
        audioSource.clip = Resources.Load<AudioClip>("Sounds/Jump");
        audioSource.Play();

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
        //float targetYRotation = (int)(transform.rotation.eulerAngles.y / 90) * 90.0f;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y % 360, 0);     
    }


    // 공격 모션인 ParticleSystem 켜기
    public void Attack(GameObject[] attackMotion, float duration,float dmg)
    {

        for (int i = 0; i < attackMotion.Length; i++)
        {
            attackMotion[i].SetActive(true);
            StartCoroutine(DeactivateAfterDuration(attackMotion, duration));
        }

        if (dmg != -1&&transform.name!="Colobus")
            FindAnyObjectByType<Hunter>().GetComponent<Hunter>().getDamaged(dmg);
    }


    // 공격 모션인 ParticleSystem 끄기
    private IEnumerator DeactivateAfterDuration(GameObject[] attackMotion, float duration)
    {
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < attackMotion.Length; i++)
        {
            attackMotion[i].SetActive(false);
        }  
    }

    public void Iced(bool value)
    {
        ice = value;
        if (ice)
        {
            if (IceObject != null)
                IceObject.SetActive(true);
            transform.tag = "IceAnimal";
        }

        else
        {
            if (IceObject != null)
                IceObject.SetActive(false);
            transform.tag = "Animal";
        }
    }

    public void Die()
    {
        if (animator != null)
            animator.SetTrigger("Dead");
        tileManager.insertTileMap((int)transform.position.x / 2, (int)transform.position.z / 2, 0);
        aiManager.RemoveAnimal(gameObject);
        aiManager.UpdateAnimalList();
        Destroy(gameObject, 1.0f);

    }

    public void GetAttackAble(Vector3[] attackBox)
    {
        Vector3 hunterPosition = FindAnyObjectByType<Hunter>().GetComponent<Hunter>().GetHunterPosition();    
        for (int i = 0; i < attackBox.Length; i++)
        {
            Vector3 attackDir = attackBox[i] + transform.position;
            if (attackDir.x == hunterPosition.x && attackDir.z == hunterPosition.z)
            {             
                SetAttackAble(true, attackDir);
            }
        }
       
    }

    public void UpdateAnimalPosition()
    {
        tileManager.insertTileMap
            (
               (int)transform.position.x / 2,
               (int)transform.position.z / 2, 
               0
            );
    }
}
