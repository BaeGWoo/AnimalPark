using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AIManager : MonoBehaviour
{
    [SerializeField] GameObject[] Animals;
    private List<Animal> animals = new List<Animal>();
    public static int[,] TileMap = new int[8, 8];
    [SerializeField] Hunter hunter;
    int count = 5;

    private void Start()
    {
        StartCoroutine(TurnManager());
    }

    public void AnimalMove()
    {
        Vector3[] movePoints=null;
        Vector3[] moveDirections=null;
        Animator animator = null;


        for (int i=0;i< Animals.Length; i++)
        {          
            Animals[i].GetComponent<Animal>().Move();     
        }
    }

    public void AnimalAttack()
    {

        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<Animal>().ActiveAttackBox();
        }
    }

    public void AnimalUnAttackBox()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<Animal>().UnActiveAttackBox();
        }
    }

    private IEnumerator TurnManager()
    {
        while (count>=0) // ������ ��� ����Ǵ� ���� �ݺ�
        {
            count--;
            // ������ �̵�
            AnimalMove();
            yield return new WaitForSeconds(2.0f); // �ʿ信 ���� �ð� ����
            AnimalAttack();

            // ������ �̵��� �����⸦ ���
            yield return new WaitForSeconds(2.0f); // �ʿ信 ���� �ð� ����
            AnimalUnAttackBox();

            // Hunter�� �̵�
            hunter.Move();

            // Hunter�� �̵��� �����⸦ ���
            while (Hunter.Moveable)
            {
                hunter.Attack();
                yield return new WaitForSeconds(1.5f); // �� ������ ���
            }
            //Hunter.Moveable = false;

            // Hunter�� �̵��� �����⸦ ���

            

            // ���� ���� ���� ��� ��� (���ϴ� ���)
            yield return null;

        }
    }



    // Update is called once per frame
    void Update()
    {
     
    }
}
