using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Hunter : MonoBehaviour
{
    public static Vector3 HunterPosition;
    public static Transform HunterRotation;

    public static bool Moveable = false;
    public static bool Running = false;
    public static bool Attackable = false;
    public static bool fireball = false;
    public static bool chooseDirection = false;
    public static float Health;
    //private float speed = 1.0f;

    [SerializeField] GameObject AIManager;
    [SerializeField] GameObject LevelManager;
    [SerializeField] GameObject TileManager;


    [SerializeField] BoxCollider Huntercollider;
    [SerializeField] Slider HPSlider;
    [SerializeField] GameObject bananaMotion;
    [SerializeField] GameObject[] AttackWeapon;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] Camera mainCamera;

    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    private int WeaponNumber = 0;
    [SerializeField] GameObject[] AttackBox;

    private AIManager aiManager;
    private LevelManager levelManager;
    private TileManager tileManager;


    public static GameObject moveAbleBlock;
    public static Vector3 attackAbleDirection;

    
    private static Hunter instance;
    //private TileManager tileManager;
    //private LevelManager levelManager;
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

        HunterPosition = new Vector3(0, 0, 0);
        Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();


        aiManager =AIManager.GetComponent<AIManager>();
        levelManager =LevelManager.GetComponent<LevelManager>();
        tileManager =TileManager.GetComponent<TileManager>();

      
        HPPosition = HPSlider.transform.position;
        Health = 10;
    }

   


    void Update()
    {
        Huntercollider.enabled = !Moveable;

        animator.SetBool("Run", Running);

        

        if (Moveable && moveAbleBlock != null)
        {
            moveHunterPosition();
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            other.transform.root.GetComponent<Animal>().Attack();
            Health--;
            HPSlider.value = Health / 10;
        }
        else if (other.CompareTag("banana"))
        {
            StartCoroutine(BananaMotion());
            Health--;
            HPSlider.value = Health / 5;
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
        MenuPanel.SetActive(true);
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
        aiManager.AnimalActiveCollider(true);
        Quaternion curRotation = transform.rotation;
        transform.LookAt(attackAbleDirection);

        animator.SetTrigger("Attack" + (WeaponNumber+1));
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
        //attackAbleDirection = new Vector3(0, 0, 0);
    }
    #endregion


    #region HunterMove
    public void moveHunterPosition()
    {
        if (moveAbleBlock != null)
        {
            // �� ��ġ ���
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


    #region �� �̵�

    public void PanelActive()
    {
        MenuPanel.SetActive(true);
    }

    public void ReLoadScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ExitScene();

        MenuPanel.SetActive(false);
        gameObject.SetActive(true);

        StartCoroutine(LoadScene(sceneName));
        transform.position = Vector3.zero;
        HunterPosition=transform.position;
    }
    IEnumerator LoadScene(string name)
    {
        yield return null;
        AIManager.SetActive(true);


        LevelManager.SetActive(false);
        levelManager.SetSceneName(name);


        SceneManager.LoadScene(name);
        aiManager.StartTurn();
        
        TileManager.SetActive(true);
        tileManager.CreateTileMap();
    }

    public void ExitScene()
    {
        MenuPanel.SetActive(false);
        Moveable = false;
        
        aiManager.ResetAnimalList();
        AIManager.SetActive(false);
       
        TileManager.SetActive(false);
        
        LevelManager.SetActive(true);
        levelManager.LinkMaps();
        
        gameObject.SetActive(false);
        SceneManager.LoadScene("Lobby");
        
    }
    #endregion
}