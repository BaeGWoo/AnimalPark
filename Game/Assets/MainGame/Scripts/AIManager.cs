using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AIManager : MonoBehaviour
{
    [SerializeField] GameObject[] Animals;
    //private List<Animal> animals = new List<Animal>();
    public static int[,] TileMap = new int[8, 8];
    [SerializeField] Hunter hunter;
    [SerializeField] GameObject animalImagePrefab;
    [SerializeField] Canvas animalCanvas;
    int count = 5;
   

    private void Start()
    {
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
            //hunter.Move();
            HunterMove();

            while (Hunter.Moveable)
            {

                yield return null;

            }

            // Hunter�� ����
            //hunter.Attack();
            HunterAttack();
            // Hunter�� �̵��� �����⸦ ���


            while (Hunter.Attackable)
            {
                yield return null;
            }

        }
    }
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
            //Debug.Log(animal.name + "�� ���� �迭���� ���ŵǾ����ϴ�.");
        }

        // ����Ʈ�� �ٽ� �迭�� ��ȯ
        Animals = animalList.ToArray();
    }


    public void UpdateAnimalList()
    {
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal"))
            {
                Destroy(child.gameObject);  // �±װ� "AnimalImage"�� �ڽ� ������Ʈ�� ����
            }
        }

        RectTransform prefabRectTransform = animalImagePrefab.GetComponent<RectTransform>();
        Vector3 initialPosition = prefabRectTransform.localPosition;// �ʱ� ��ġ
        float yOffset = -145f; // Y �������� ����
        Vector3 currentPosition = new Vector3(0,-145.0f,0);

        for (int i = 0; i < Animals.Length; i++)
        {
            GameObject animalFace = Instantiate(animalImagePrefab, animalCanvas.transform);

            //Image childImage = animalFace.GetComponentInChildren<Image>();
            animalFace.GetComponent<Image>().sprite = Resources.Load<Sprite>(Animals[i].name);

            RectTransform rectTransform = animalFace.GetComponent<RectTransform>();
            rectTransform.localPosition += currentPosition*i;


            Slider hpSlider = animalFace.GetComponentInChildren<Slider>();
            hpSlider.value = Animals[i].GetComponent<Animal>().GetHP()/3;
            // ���� ������Ʈ�� ��ġ�� ���� y �� ����
            //currentPosition.y += yOffset;
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
                hpSlider.value = Animals[index++].GetComponent<Animal>().GetHP()/3;
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


    // Update is called once per frame
    void Update()
    {
     
    }
}
