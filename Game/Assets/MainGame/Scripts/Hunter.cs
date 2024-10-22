using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunter : MonoBehaviour
{
    public static Vector3 HunterPosition;
    public static Transform HunterRotation;
    public static bool Moveable = false;
    public static bool Running = false;
    [SerializeField] AIManager aiManager;
    [SerializeField] BoxCollider Huntercollider;
    [SerializeField] Slider HPSlider;
    private Vector3 HPPosition;
    private Animator animator;
    private int keyInput;
    [SerializeField] GameObject[] AttackBox;

    void Awake()
    {
        
        HunterPosition = new Vector3(0, 0, 0);
       Huntercollider = GetComponent<BoxCollider>();
        HunterRotation = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        HPPosition = HPSlider.transform.position;
    }

    public void Move()
    {
        Moveable = true;
       
    }

    public void Attack()
    {
        Moveable = false;
        bool inputReceived = false;

        while (!inputReceived)  // �Է��� ���� ������ �ݺ�
        {
            // 1�� Ű�� ������ ��
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                keyInput = 1;
                inputReceived = true;  // �Է��� �޾Ҵٰ� ǥ��             
            }

            // 2�� Ű�� ������ ��
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                keyInput = 2;
                inputReceived = true;  // �Է��� �޾Ҵٰ� ǥ��
            }         
        }

        animator.SetTrigger("Attack" + keyInput);
        AttackBox[keyInput - 1].SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = HunterPosition;
       Huntercollider.enabled=!Moveable;
        if (Running)
        {
            Debug.Log("RUN");
            
        }
        //Vector3 lookDirection = HPPosition;
        //lookDirection.y = 0; // ���� ������ ����
        //HPSlider.transform.position = transform.position+HPPosition;
        //HPSlider.transform.rotation= Quaternion.Euler(45, 0, 0);
        //HPSlider.transform.LookAt(Camera.main.transform);
        animator.SetBool("Run", Running);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            other.transform.root.GetComponent<Animal>().Attack();
        Debug.Log(other.transform.root.name);
        }
    }
}
