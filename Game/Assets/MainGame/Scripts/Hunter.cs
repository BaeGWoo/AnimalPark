using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class Hunter : MonoBehaviour
{
    public static Vector3 HunterPosition;
    public static Transform HunterRotation;
    [SerializeField] public static int curLevel = 0;
    [SerializeField] public static bool Moveable = false;
    [SerializeField] public static bool Running = false;
    [SerializeField] public static bool Attackable = false;
    [SerializeField] public static bool fireball = false;
    [SerializeField] public static bool chooseDirection = false;

    [SerializeField] public static float Health;
    [SerializeField] public float MaxHealth;
    [SerializeField] GameObject HealthDebuff;

  
    [SerializeField] GameObject HunterCanvas;

    [SerializeField] Image HPSlider;
    [SerializeField] GameObject bananaMotion;
    [SerializeField] GameObject posionMotion;
    [SerializeField] GameObject magic;
    [SerializeField] GameObject[] AttackWeapon;
    [SerializeField] Button[] AttackButton;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] Camera mainCamera;

    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    private int WeaponNumber = 0;
    [SerializeField] GameObject[] AttackBox;
    [SerializeField] AudioClip[] AttackSound; 

    [SerializeField] public GameObject moveAbleBlock;
    [SerializeField] Vector3 attackAbleDirection;
    [SerializeField] float moveDebuff = 0;
    [SerializeField] bool attackDebuff = false;
    [SerializeField] int healthDebuffCount = 0;
    [SerializeField] float healthDebuffDMG = 0;

    [SerializeField] TileManager tileManager;
    private static Hunter instance;
    void Awake()
    {
        
       
        mainCamera = Camera.main;

        HunterPosition = new Vector3(8, 2, 0);
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();


      
        //HPPosition = HPSlider.transform.position;
        Health = 5;
        MaxHealth = 5;
    }

    private void Start()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
    }


    void Update()
    {     
        GetComponent<Rigidbody>().useGravity = false;
        HunterCanvas.SetActive(true);

        animator.SetBool("Run", Running);

        if (Moveable && moveAbleBlock == null)
        {
            ClickPosition();
        }

        if (Moveable && moveAbleBlock != null)
        {
            moveHunterPosition();
        }

        if(Attackable && moveAbleBlock == null)
        {
            ClickPosition();
        }


    }

    public int getCurLevel()
    {
        return curLevel;
    }

    public void setCamera(Camera camera)
    {
        mainCamera=camera;
    }

    public void getDamaged(float dmg)
    {
        animator.SetTrigger("Damage");
        Health -= dmg;
        HPSlider.fillAmount = Health / MaxHealth;
    }

    public Vector3 GetHunterPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("banana"))
        {
            animator.SetTrigger("Damage");
            getDamaged(0.5f);
            StartCoroutine(BananaMotion(bananaMotion));          
            FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().SoundPlay("Banana");
            Destroy(other.gameObject);
            HPSlider.fillAmount = Health / MaxHealth;
        }

        else if (other.CompareTag("Posion"))
        {
            animator.SetTrigger("Damage");
            getDamaged(0.5f);
            StartCoroutine(BananaMotion(posionMotion));
            Destroy(other.gameObject);
            HPSlider.fillAmount = Health / MaxHealth;
        }

        else if (other.CompareTag("Magic"))
        {
            moveDebuff = 6;
            magic.SetActive(true);
        }

        else if (other.CompareTag("FireWall"))
        {
            getDamaged(10);
        }
  
       

        if (Health <= 0)
        {
            Die();
            //HunterDeathSound
        }

    }

 


    public void ElegatorAttack(float x, float z)
    {
        animator.SetTrigger("Damage");
        Vector3 NuckBackDirection;
        float time = 6.0f;
        float timeGap = 0;
        NuckBackDirection = new Vector3(x, 0, z);

        //x = -1
        if (transform.position.x + 6.0f*x < 0)
        {
            timeGap = transform.position.x - 6.0f;
            if (timeGap <= 0)
                time += timeGap;
        }

        //x = 1
        else if (transform.position.x + 6.0f*x > 14)
        {
            timeGap = 14 - transform.position.x;
            if (timeGap < 6)
            {
                time -= timeGap;
            }
        }

        //z = -1
        else if (transform.position.z + 6.0f * z < 0)
        {
            timeGap = transform.position.z - 6.0f;
            if (timeGap <= 0)
                time += timeGap;
        }

        else
        {
            timeGap = 14 - transform.position.z;
            if (timeGap < 6)
            {
                time -= timeGap;
            }
        }
        HunterPosition = transform.position+time*NuckBackDirection;

        StartCoroutine(NuckBack(time, NuckBackDirection));

    }

    public void BossAttack(Vector3 dir)
    {
        animator.SetTrigger("Damage");
        float time = 6.0f;
        dir = (dir - transform.position).normalized;
        StartCoroutine(NuckBack(time, dir));
    }

    IEnumerator NuckBack(float time,Vector3 NuckBackDirection)
    {
        yield return new WaitForSeconds(0.5f);
        //transform.LookAt(NuckBackDirection);
        while (time > 0)
        {
            transform.position += NuckBackDirection * 6.0f * Time.deltaTime;
            time -= 6.0f * Time.deltaTime;
            yield return null;
        }
    }

    public void GetHealthDebuff(int count, float dmg)
    {
        HealthDebuff.SetActive(true);
        healthDebuffCount = count;
        healthDebuffDMG = dmg;
    }

    public void ClickPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                if (Moveable)
                {
                    if (clickedObject.CompareTag("MoveAbleBlock"))
                    {
                        moveAbleBlock = clickedObject;
                        tileManager.TileBlockReset();
                    }
                }

                if (Attackable)
                {
                    if (clickedObject.CompareTag("MoveAbleBlock"))
                    {
                        moveAbleBlock = clickedObject;
                        attackAbleDirection = new Vector3(clickedObject.transform.position.x,0, clickedObject.transform.position.z);
                        chooseDirection = true;
                    }
                }
            }
        }
    }


    IEnumerator BananaMotion(GameObject motion)
    {
        motion.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        motion.SetActive(false);
    }

    #region Hunter State
    public void Move()
    {
        if (healthDebuffCount > 0)
        {
            healthDebuffCount--;
            getDamaged(healthDebuffDMG);
            if(healthDebuffCount == 0)
            {
                HealthDebuff.SetActive(false);
                healthDebuffDMG = 0;
            }
        }

        if (moveDebuff >0)
        {
            moveDebuff=0;
            if (magic != null)
                magic.SetActive(false);
        }

        else
        {
            Moveable = true;
            tileManager.SetMoveableTile(transform.position, moveDebuff);
        }

       
    }

    public void Attack()
    {
        if (attackDebuff)
        {
            attackDebuff = false;
        }

        else
        {
            Attackable = true;
            for (int i = 0; i < AttackButton.Length; i++)
            {
                AttackButton[i].interactable = true;
            }
        }
        
    }

    public void Die()
    {
        animator.SetTrigger("Die");
       // soundManager.SoundPlay("GameOver");
        MenuPanel.SetActive(true);
    }

    public void SetInitialState()
    {
        Moveable = false;
        Running = false;
        Attackable = false;
 

        Health = MaxHealth;
        GetComponent<Rigidbody>().useGravity = false;
        HPSlider.fillAmount = Health / MaxHealth;
        transform.position = new Vector3(8,0,0);
    }

   

    #endregion


    #region HunterAttack
    public void AttackType(Button button)
    {
        if (!Attackable) return;
        
        int num = int.Parse(button.name.Substring("Attack".Length));
        WeaponNumber = num-1;

        for (int i = 0; i < AttackButton.Length; i++)
        {
            AttackButton[i].interactable = false;
        }
        StartCoroutine(HunterWeaponAttack());


    }

    IEnumerator HunterWeaponAttack()
    {
        if (WeaponNumber == 1)
        {
            if (magic==null||!magic.activeSelf)
            {
                chooseDirection = false;
                tileManager.SetAttackableTile(transform.position);
                while (!chooseDirection)
                {
                    yield return null;
                }
                tileManager.TileBlockReset();
            }
           
            ActiveHunterAttack();
        }

        else
            ActiveHunterAttack();
    }

    public void ActiveHunterAttack()
    {
        //FindAnyObjectByType<AIManager>().GetComponent<AIManager>().AnimalActiveCollider(true);
        Quaternion curRotation = transform.rotation;
        
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        float attackTime = 0.5f;

        animator.SetTrigger("Attack" + (WeaponNumber+1));
        if (WeaponNumber == 0)
        {
            FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().SoundPlay("HunterAttack");           
        }
        else if (WeaponNumber == 1)
        {
            attackTime = 0;
        }
        AttackWeapon[WeaponNumber].SetActive(true);
        StartCoroutine(HunterAttackMotion(attackTime));
        if (WeaponNumber == 1)
        {
            if (magic==null||!magic.activeSelf)
            {
                transform.LookAt(attackAbleDirection);
                StartCoroutine(IceMove(attackAbleDirection));
                if (magic != null)
                    magic.SetActive(false);
            }
            
        }
        StartCoroutine(HunterAttackEnd());
        
    }

    IEnumerator HunterAttackMotion(float time)
    {
        yield return new WaitForSeconds(time);
        AttackBox[WeaponNumber].SetActive(true);
    }

    IEnumerator IceMove(Vector3 attackDir)
    {
        transform.LookAt(attackDir);
        float step = 2.0f * Time.deltaTime;
        while (Vector3.Distance(transform.position, attackDir) > 0.1f)  // 0.1f는 허용 오차
        {
            // 목표 위치로 서서히 이동
            transform.position = Vector3.MoveTowards(transform.position, attackDir, step);

            // 한 프레임 대기
            yield return null;
        }

        transform.position = new Vector3(attackDir.x, transform.position.y, attackDir.z);
        HunterPosition= transform.position;
        moveAbleBlock = null;
    }

    IEnumerator HunterAttackEnd()
    {
        yield return new WaitForSeconds(2.5f);

        Attackable = false;
        fireball = false;
        chooseDirection = true;
        AttackBox[WeaponNumber].SetActive(false);
        AttackWeapon[WeaponNumber].SetActive(false);
    }
    #endregion


    #region HunterMove
    public void GetMoveDebuff(float value)
    {
        moveDebuff = value;
    }

    public void GetAttackDebuff()
    {
        attackDebuff = true;
    }

    public void moveHunterPosition()
    {
        if (moveAbleBlock != null)
        {
            // 새 위치 계산
            Vector3 newPosition = moveAbleBlock.transform.position;          
            StartCoroutine(Move(newPosition));
        }
    }

    IEnumerator Move(Vector3 target)
    {
        Running = true;
        Vector3 startPosition = transform.position;
        Vector3 currentPosition = startPosition;
        Vector3 targetPosition = new Vector3(target.x, startPosition.y, currentPosition.z);
        transform.LookAt(targetPosition);
        float speed = 3.0f;

        while (Mathf.Abs(currentPosition.x - target.x) > Mathf.Epsilon&&Moveable)
        {
            float newX = Mathf.MoveTowards(currentPosition.x, target.x, speed * Time.deltaTime);
            transform.position = new Vector3(newX, 0, startPosition.z);
            currentPosition = transform.position;
            

            yield return null;
        }

        startPosition = currentPosition;
        targetPosition = new Vector3(currentPosition.x, startPosition.y, target.z);
        transform.LookAt(targetPosition);

        while (Mathf.Abs(currentPosition.z - target.z) > Mathf.Epsilon && Moveable)
        {
            float newZ = Mathf.MoveTowards(currentPosition.z, target.z, speed * Time.deltaTime);
            transform.position = new Vector3(startPosition.x,0, newZ);
            currentPosition = transform.position;
            yield return null;
        }

        OnReachedDestination();
    }


    public void OnReachedDestination()
    {
        //currentStage = MoveStage.Done;
        Moveable = false;
        Running = false;
       
        HunterPosition = transform.position;
        moveAbleBlock = null;
        moveDebuff = 0;
    }

    public void StopPosition(Vector3 position)
    {
        Vector3 tempPosition=new Vector3
            (
            (int)((position.x+1)/2)*2,
            transform.position.y,
             (int)((position.z+1) / 2) * 2
            );

        transform.position = tempPosition;
        HunterPosition = transform.position;
    }

    #endregion


  

    public void LevelUp()
    {
        //levelManager.LevelUp();
        //ExitScene();
        transform.position = new Vector3(8, 0, 0);
        Health = MaxHealth;
        HPSlider.fillAmount = Health / MaxHealth;
        HunterPosition = transform.position;
        //levelManager.LinkMaps();
        //버튼 이벤트를 헌터에 주고 씬 이동전에 LevelUp호출


    }

    public void PanelOff()
    {
        MenuPanel.SetActive(false);
    }

    public void MoveEndingScene()
    {
        SceneManager.LoadScene("End");
        //Destroy(mainCamera);
        //Destroy(LevelManager);
        //Destroy(AIManager);
        //Destroy(TileManager);
        //Destroy(LoadManager);
        Destroy(gameObject);
    }
}
