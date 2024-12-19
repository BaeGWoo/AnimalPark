using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Hunter : MonoBehaviour
{
    public static Vector3 HunterPosition;
    public static Transform HunterRotation;

    [SerializeField] public static bool Moveable = false;
    [SerializeField] public static bool Running = false;
    [SerializeField] public static bool Attackable = false;
    [SerializeField] public static bool fireball = false;
    [SerializeField] public static bool chooseDirection = false;
    
   [SerializeField] public static float Health;
   [SerializeField] public float MaxHealth;


    public bool herringDebuff = false;
    public bool squidDebuff = false;

    //로비에서 움직임 제한을 위한 변수
    [SerializeField] Vector3 stagePosition;
    [SerializeField] GameObject[] cameraTriggers;
    [SerializeField] GameObject[] cameraPosition;
    [SerializeField] public bool onStage = true;
    [SerializeField] public bool onStair = false;
    
    private float curX;
    private float curY;
    private float curZ;
    private float stageSpeed = 3.0f;

    [SerializeField] GameObject AIManager;
    //[SerializeField] GameObject LevelManager;
    [SerializeField] GameObject TileManager;
    [SerializeField] GameObject LoadManager;
    [SerializeField] GameObject SoundManager;

    [SerializeField] BoxCollider Huntercollider;
    [SerializeField] Image HPSlider;
    [SerializeField] GameObject bananaMotion;
    [SerializeField] GameObject[] AttackWeapon;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] Camera mainCamera;

    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    private int WeaponNumber = 0;
    [SerializeField] GameObject[] AttackBox;
    [SerializeField] AudioClip[] AttackSound; 

    public static GameObject moveAbleBlock;
    public static Vector3 attackAbleDirection;

  

    private static Hunter instance;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            instance = this;
        }

        else if(instance != this)
        {
            Destroy(gameObject);
        }
        stagePosition = transform.position;
        mainCamera = Camera.main;

        HunterPosition = new Vector3(8, 0, 0);
        Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();


      
        //HPPosition = HPSlider.transform.position;
        Health = 5;
        MaxHealth = 5;
    }




    void Update()
    {
        


        if (onStage)
        {
            Vector3 moveDirection = Vector3.zero;

            curX = Input.GetAxisRaw("Horizontal");
            curZ = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(curX, 0f, curZ).normalized;

            Vector3 targetDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            transform.LookAt(transform.position + targetDirection);

            animator.SetBool("Run", moveDirection != Vector3.zero);
            //transform.Translate(-moveDirection * stageSpeed * Time.deltaTime);

            if (moveDirection != Vector3.zero)
                transform.Translate(Vector3.forward * stageSpeed * Time.deltaTime);


            stagePosition = transform.position;
        }


        else
        {
            Huntercollider.enabled = !Moveable;
            animator.SetBool("Run", Running);

            if (Moveable && moveAbleBlock != null)
            {
                moveHunterPosition();
            }
        }
       
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            other.transform.root.GetComponent<Animal>().Attack();
            //soundManager.SoundPlay(other.transform.root.name+ "Attack");
            animator.SetTrigger("Damage");
            Health--;
            HPSlider.fillAmount = Health / MaxHealth;
        }

        else if (other.CompareTag("colobusAttack"))
        {
            other.transform.root.GetComponent<Animal>().Attack();
            //soundManager.SoundPlay("ColobusAttack");
        }

        else if (other.CompareTag("banana"))
        {
            StartCoroutine(BananaMotion());
            //soundManager.SoundPlay("Banana");
            //Health -= aiManager.animalStatus["Colobus"][0];
            other.gameObject.SetActive(false);
            HPSlider.fillAmount = Health / MaxHealth;
        }

       

       

     
        if (other.name == "CameraMove")
        {
            other.gameObject.SetActive(false);
            for(int i = cameraTriggers.Length - 1; i >= 0; i--)
            {
                if (!cameraTriggers[i].gameObject.activeSelf)
                {
                    mainCamera.transform.position = cameraPosition[i].transform.localPosition;
                    break;
                }
            }
        }

        

        if (Health <= 0)
        {
            Die();
            //HunterDeathSound
        }

    }

    


    IEnumerator BananaMotion()
    {
        bananaMotion.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        bananaMotion.SetActive(false);
    }

    #region Hunter State
    public void Move()
    {
        Moveable = true;

    }

    public void Attack()
    {
        Attackable = true;
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
        HPSlider.fillAmount = Health / MaxHealth;
        HunterPosition = new Vector3(8, 0, 0);
    }

    public void SetLobbyState()
    {
        onStage = true;
        transform.position = stagePosition;
    }

    #endregion


    #region HunterAttack
    public void AttackType(Button button)
    {
        if (!Attackable) return;
        
        int num = int.Parse(button.name.Substring("Attack".Length));
        WeaponNumber = num-1;

        StartCoroutine(HunterWeaponAttack());


    }

    IEnumerator HunterWeaponAttack()
    {
        if (WeaponNumber == 1)
        {
            fireball = true;
            chooseDirection = false;
            while (!chooseDirection)
            {
                yield return null;
            }
            ActiveHunterAttack();
        }

        else
            ActiveHunterAttack();
    }

    public void ActiveHunterAttack()
    {
        //aiManager.AnimalActiveCollider(true);
        Quaternion curRotation = transform.rotation;
        transform.LookAt(attackAbleDirection);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);


        animator.SetTrigger("Attack" + (WeaponNumber+1));
        if (WeaponNumber == 0)
        {
            //soundManager.SoundPlay("HunterAttack");
        }
        else if (WeaponNumber == 1)
        {
            //soundManager.SoundPlay("FireBall");
        }
        AttackWeapon[WeaponNumber].SetActive(true);
        StartCoroutine(HunterAttackMotion());

        StartCoroutine(HunterAttackEnd());
        
    }

    IEnumerator HunterAttackMotion()
    {
        yield return new WaitForSeconds(0.5f);
        AttackBox[WeaponNumber].SetActive(true);
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

        while (Mathf.Abs(currentPosition.x - target.x) > Mathf.Epsilon)
        {
            float newX = Mathf.MoveTowards(currentPosition.x, target.x, speed * Time.deltaTime);
            transform.position = new Vector3(newX, startPosition.y, startPosition.z);
            currentPosition = transform.position;
            

            yield return null;
        }

        startPosition = currentPosition;
        targetPosition = new Vector3(currentPosition.x, startPosition.y, target.z);
        transform.LookAt(targetPosition);

        while (Mathf.Abs(currentPosition.z - target.z) > Mathf.Epsilon)
        {
            float newZ = Mathf.MoveTowards(currentPosition.z, target.z, speed * Time.deltaTime);
            transform.position = new Vector3(startPosition.x, startPosition.y, newZ);
            currentPosition = transform.position;
            yield return null;
        }

        OnReachedDestination();
    }


    private void OnReachedDestination()
    {
        //currentStage = MoveStage.Done;
        Moveable = false;
        Running = false;
        HunterPosition = transform.position;
        moveAbleBlock = null;
    }

    #endregion


    #region 씬 이동

    public void PanelActive()
    {
        if (!MenuPanel.activeSelf)
        {
            //soundManager.SoundPlay("PausePanel");
            MenuPanel.SetActive(true);
        }
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
        Destroy(AIManager);
        Destroy(TileManager);
        Destroy(LoadManager);
        Destroy(gameObject);
    }
}
