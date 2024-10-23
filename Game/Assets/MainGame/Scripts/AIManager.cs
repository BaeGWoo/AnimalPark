using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AIManager : MonoBehaviour
{
    [SerializeField] GameObject[] Animals;
    //private List<Animal> animals = new List<Animal>();
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
        while (count>=0) // 게임이 계속 진행되는 동안 반복
        {
            count--;
            // 동물의 이동
            AnimalMove();
            yield return new WaitForSeconds(1.0f); // 필요에 따라 시간 조정
            AnimalAttack();

            // 동물의 이동이 끝나기를 대기
            yield return new WaitForSeconds(1.0f); // 필요에 따라 시간 조정
            AnimalUnAttackBox();

            // Hunter의 이동
            hunter.Move();

            while (Hunter.Moveable)
            {
                yield return null;

            }

            // Hunter의 공격
            hunter.Attack();

            // Hunter의 이동이 끝나기를 대기
           

            while (Hunter.Attackable)
            {
                yield return null;
            }

        }
    }

    public void RemoveAnimal(GameObject animal)
    {
        // 배열을 리스트로 변환
        List<GameObject> animalList = new List<GameObject>(Animals);

        // 해당 동물 GameObject를 리스트에서 삭제
        if (animalList.Contains(animal))
        {
            animalList.Remove(animal);
            //Debug.Log(animal.name + "가 동물 배열에서 제거되었습니다.");
        }

        // 리스트를 다시 배열로 변환
        Animals = animalList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
