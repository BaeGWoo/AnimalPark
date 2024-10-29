using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private float speed = 3.0f;

    [SerializeField] AIManager aiManager;
    [SerializeField] BoxCollider Huntercollider;
    [SerializeField] Slider HPSlider;
    [SerializeField] GameObject bananaMotion;
    [SerializeField] GameObject[] AttackWeapon;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] Camera mainCamera;

    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    [SerializeField] GameObject[] AttackBox;

    public static GameObject moveAbleBlock;
    public static GameObject attackAbleDirection;

    private enum MoveStage { MovingX, MovingZ, Done }
    private MoveStage currentStage = MoveStage.MovingX;

    void Awake()
    {
        HunterPosition = new Vector3(0, 0, 0);
        Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();
       
        HPPosition = HPSlider.transform.position;
        Health = 10;
        DontDestroyOnLoad(gameObject);
    }

    public void Move()
    {
        Moveable = true;
       
    }

    public void moveHunterPosition()
    {
        if (moveAbleBlock != null)
        {
            // 새 위치 계산
            Vector3 newPosition = moveAbleBlock.transform.position;
            StartCoroutine(MoveTo(newPosition));
        }
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

    // Update is called once per frame
    void Update()
    {
        //HunterPosition= transform.position;
        Huntercollider.enabled = !Moveable;

        animator.SetBool("Run", Running);

        if (Moveable || Attackable)
            ClickPosition();

        if (Moveable && moveAbleBlock != null)
        {
            moveHunterPosition();
        }

    }

    public void ClickPosition()
    {
        Debug.Log("CLICK");
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                Debug.Log(clickedObject);
                if (Hunter.Moveable)
                {
                    if (clickedObject.CompareTag("MoveAbleBlock"))
                    {
                        Hunter.moveAbleBlock = clickedObject;
                    }
                }

                if (Hunter.Attackable)
                {
                    if (clickedObject.CompareTag("AttackAbleDirection"))
                    {
                        Hunter.attackAbleDirection = clickedObject;
                    }
                }


            }
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

    public void AttackType(Button button)
    {
        Debug.Log("ATTACK");
        if (!Attackable) return;
        string animatorTrigger = button.name;
        if (animatorTrigger == "Attack2")
        {
            fireball = true;
        }
        int num = int.Parse(animatorTrigger.Substring("Attack".Length));


        if (fireball)
        {
            chooseDirection = false;

            StartCoroutine(ChooseFireBallDirection(num));
        }

        else
        {
            animator.SetTrigger("Attack" + num);
            AttackWeapon[num - 1].SetActive(true);
            StartCoroutine(HunterAttackMotion(num));

            StartCoroutine(HunterAttackEnd(num));
        }


    }

    IEnumerator HunterAttackMotion(int num)
    {
        yield return new WaitForSeconds(0.5f);
        AttackBox[num - 1].SetActive(true);


    }

    IEnumerator HunterAttackEnd(int num)
    {
        yield return new WaitForSeconds(2.5f);

        Attackable = false;
        AttackBox[num - 1].SetActive(false);
        AttackWeapon[num - 1].SetActive(false);
    }

    IEnumerator ChooseFireBallDirection(int num)
    {
        while (!chooseDirection)
        {
            yield return null;
        }

        animator.SetTrigger("Attack" + num);
        AttackWeapon[num - 1].SetActive(true);

        StartCoroutine(HunterAttackMotion(num));
        // StartCoroutine(Fireball());
        StartCoroutine(HunterAttackEnd(num));
    }

    public void PanelActive()
    {
        MenuPanel.SetActive(true);
    }

    public void ReLoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitScene()
    {
        SceneManager.LoadScene("Lobby");
        gameObject.SetActive(false);
    }

    

   

   

    private IEnumerator MoveTo(Vector3 target)
    {
        Running = true;
        Vector3 startPosition = transform.position;
        Vector3 currentPosition=transform.position;
        currentStage = MoveStage.MovingX;
        Vector3 targetPosition = new Vector3(target.x, startPosition.y, currentPosition.z);
        transform.LookAt(targetPosition);

        float yRotation = transform.eulerAngles.y;
        float step = speed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        while (Mathf.Abs(currentPosition.x - target.x) > Mathf.Epsilon)
        {
            float newX = Mathf.MoveTowards(transform.position.x, target.x, step);
            transform.position = new Vector3(newX, startPosition.y, startPosition.z);
            currentPosition = transform.position;
            yield return null; 
        }


        startPosition = transform.position;
        currentStage = MoveStage.MovingZ;
        targetPosition = new Vector3(currentPosition.x, startPosition.y, target.z);
        transform.LookAt(targetPosition);
        while (Mathf.Abs(currentPosition.z - target.z) > Mathf.Epsilon)
        {
            float newZ = Mathf.MoveTowards(transform.position.z, target.z, step);
            transform.position = new Vector3(startPosition.x, startPosition.y, newZ);
            currentPosition = transform.position;
            yield return null;
        }

        OnReachedDestination();
    }


 


    private void OnReachedDestination()
    {
        currentStage = MoveStage.Done;
        Moveable = false;
        Running = false;
        HunterPosition = transform.position;
    }



}
