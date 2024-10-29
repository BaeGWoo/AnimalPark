using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.SceneManagement;

public class AIManager : MonoBehaviour
{
    [SerializeField] GameObject[] Animals;
    //private List<Animal> animals = new List<Animal>();
    public static int[,] TileMap = new int[8, 8];
    [SerializeField] Hunter hunter;
    [SerializeField] GameObject animalImagePrefab;
    [SerializeField] Canvas animalCanvas;
    [SerializeField] int MaxAnimalHP = 3;
    int count = 5;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //private void Start()
    //{
    //    StartCoroutine(TurnManager());
    //    UpdateAnimalList();
    //}

    public void StartTurn()
    {
        StartCoroutine(ActiveAiManager());
        
    }

    IEnumerator ActiveAiManager()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject[] temp = GameObject.Find("AnimalManager").GetComponent<AnimalManager>().GetCurrentAnimals();
        Animals = new GameObject[temp.Length];
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i] = temp[i];
        }
        StartCoroutine(TurnManager());
        UpdateAnimalList();
    }

    public void AnimalMove()
    {
        Vector3[] movePoints=null;
        Vector3[] moveDirections=null;
        Animator animator = null;


        for (int i=0;i< Animals.Length; i++)
        {          
            Animals[i].GetComponent<Animal>().Move();
            Animals[i].GetComponent<BoxCollider>().enabled = true;
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
        while (Hunter.Health>=0) // 게임이 계속 진행되는 동안 반복
        {
            // 동물의 이동
            AnimalMove();
            yield return new WaitForSeconds(1.0f); // 필요에 따라 시간 조정
            AnimalAttack();
            yield return new WaitForSeconds(1.0f);
            
            if (CheckAniamlAttack())
            {    
                yield return new WaitForSeconds(3.0f); 
            }

            else
            {
                yield return new WaitForSeconds(1.0f); 
            }

            AnimalUnAttackBox();

           
            // Hunter의 이동
            HunterMove();

            while (Hunter.Moveable)
            {
                yield return null;
            }

            // Hunter의 공격
            HunterAttack();
            //hunter.Attack();

            // Hunter의 이동이 끝나기를 대기
            while (Hunter.Attackable)
            {
                yield return null;
            }

        }
    }

    // 현재 공격가능한 개체 탐색
    public bool CheckAniamlAttack()
    {
        bool check = true;

        for (int i = 0; i < Animals.Length; i++)
        {
            if (Animals[i].GetComponent<Animal>().GetAttackAble())
                check = false;
        }

        return check;
    }


    public void RemoveAnimal(GameObject animal)
    {
        // 배열을 리스트로 변환
        List<GameObject> animalList = new List<GameObject>(Animals);

        // 해당 동물 GameObject를 리스트에서 삭제
        if (animalList.Contains(animal))
        {
            animalList.Remove(animal);
        }

        // 리스트를 다시 배열로 변환
        Animals = animalList.ToArray();
    }


    public void UpdateAnimalList()
    {
        // 기존에 있던 썸네일 모두 삭제
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal"))
            {
                Destroy(child.gameObject);  
            }
        }

        // 동물 이미지 프리팹에서 기본 위치값 받아오기
        RectTransform prefabRectTransform = animalImagePrefab.GetComponent<RectTransform>();
        Vector3 initialPosition = prefabRectTransform.localPosition;
        float yOffset = -145f; 
        Vector3 currentPosition = new Vector3(0,-145.0f,0);

        // 새로운 리스트에 맞게 프리팹 생성
        for (int i = 0; i < Animals.Length; i++)
        {
            // 각 인덱스 번호에 맞는 동물 스프라이트 할당
            GameObject animalFace = Instantiate(animalImagePrefab, animalCanvas.transform);
            animalFace.GetComponent<Image>().sprite = Resources.Load<Sprite>(Animals[i].name);

            // 각 인덱스별 위치설정
            RectTransform rectTransform = animalFace.GetComponent<RectTransform>();
            rectTransform.localPosition += currentPosition*i;


            Slider hpSlider = animalFace.GetComponentInChildren<Slider>();
            hpSlider.value = Animals[i].GetComponent<Animal>().GetHP()/ MaxAnimalHP;
        }       
    }

    public void UpdateAnimalHp()
    {
        int index = 0;
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal"))
            {
                Slider hpSlider = child.gameObject.GetComponentInChildren<Slider>();
                hpSlider.value = Animals[index++].GetComponent<Animal>().GetHP()/ MaxAnimalHP;
            }
        }
    }

    public void HunterMove()
    {
        for(int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<BoxCollider>().enabled = false;
        }
        hunter.Move();
    }

    public void HunterAttack()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<BoxCollider>().enabled = true;
        }
        hunter.Attack();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("esc");
            hunter.PanelActive();
        }
    }
}
