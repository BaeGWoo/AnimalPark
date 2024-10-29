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
        while (Hunter.Health>=0) // ������ ��� ����Ǵ� ���� �ݺ�
        {
            // ������ �̵�
            AnimalMove();
            yield return new WaitForSeconds(1.0f); // �ʿ信 ���� �ð� ����
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

           
            // Hunter�� �̵�
            HunterMove();

            while (Hunter.Moveable)
            {
                yield return null;
            }

            // Hunter�� ����
            HunterAttack();
            //hunter.Attack();

            // Hunter�� �̵��� �����⸦ ���
            while (Hunter.Attackable)
            {
                yield return null;
            }

        }
    }

    // ���� ���ݰ����� ��ü Ž��
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
        // �迭�� ����Ʈ�� ��ȯ
        List<GameObject> animalList = new List<GameObject>(Animals);

        // �ش� ���� GameObject�� ����Ʈ���� ����
        if (animalList.Contains(animal))
        {
            animalList.Remove(animal);
        }

        // ����Ʈ�� �ٽ� �迭�� ��ȯ
        Animals = animalList.ToArray();
    }


    public void UpdateAnimalList()
    {
        // ������ �ִ� ����� ��� ����
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal"))
            {
                Destroy(child.gameObject);  
            }
        }

        // ���� �̹��� �����տ��� �⺻ ��ġ�� �޾ƿ���
        RectTransform prefabRectTransform = animalImagePrefab.GetComponent<RectTransform>();
        Vector3 initialPosition = prefabRectTransform.localPosition;
        float yOffset = -145f; 
        Vector3 currentPosition = new Vector3(0,-145.0f,0);

        // ���ο� ����Ʈ�� �°� ������ ����
        for (int i = 0; i < Animals.Length; i++)
        {
            // �� �ε��� ��ȣ�� �´� ���� ��������Ʈ �Ҵ�
            GameObject animalFace = Instantiate(animalImagePrefab, animalCanvas.transform);
            animalFace.GetComponent<Image>().sprite = Resources.Load<Sprite>(Animals[i].name);

            // �� �ε����� ��ġ����
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
